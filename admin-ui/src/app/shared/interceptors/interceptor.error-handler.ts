import { HttpErrorResponse, HttpHandler, HttpHandlerFn, HttpRequest } from "@angular/common/http";
import { RefreshTokenState } from "./refresh-token.service";
import { TokenStorageService } from "../services/token-storage.services";
import { catchError, filter, of, switchMap, take, throwError } from "rxjs";
import { ToastService } from "../services/alert.services";
import { Router } from "@angular/router";
import { UrlConstants } from "../constants/url.constants";


export function handle401Error(
    req: HttpRequest<any>,
    next: HttpHandlerFn,
    authService: TokenStorageService,
    refreshState: RefreshTokenState,
    router: Router
) {
    if (!refreshState.isRefreshing) {
        refreshState.isRefreshing = true;
        refreshState.refreshTokenSubject.next(null);
        const token = authService.getToken();
        if (!token) {
            throw new Error("Token is null");
        }
        const refreshToken = authService.getRefreshToken();
        if (!refreshToken) {
            throw new Error("Refresh token is null");
        }
        return of(refreshToken).pipe(
            switchMap((newToken: string) => {
                refreshState.isRefreshing = false;
                authService.saveToken(newToken);
                refreshState.refreshTokenSubject.next(newToken);
                const retryReq = req.clone({
                    setHeaders: {
                        Authorization: `Bearer ${newToken}`,
                    }
                });

                return next(retryReq);
            }),
            catchError((err) => {
                refreshState.isRefreshing = false;
                authService.signOut();
                router.navigate([UrlConstants.LOGIN]);
                return throwError(() => err);
            })
        );
    }

    return refreshState.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap((token) => {
            const retryReq = req.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`,
                }
            });
            console.warn('Retry request token')
            return next(retryReq);
        })
    );
}

export function handleErrorStatus(
    error: HttpErrorResponse,
    authService: TokenStorageService,
    alertService: ToastService
) {

    if (error.status === 403) {
        authService.signOut();
    }
    if (error.status === 500) {
        alertService.error('An error has occurred. Please contact the administrator.');
    }
}