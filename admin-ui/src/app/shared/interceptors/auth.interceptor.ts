import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { TokenStorageService } from "../services/token-storage.services";
import { RefreshTokenState } from "./refresh-token.service";
import { addTokenHeader } from "./interceptor.add-token";
import { catchError, throwError } from "rxjs";
import { handle401Error, handleErrorStatus } from "./interceptor.error-handler";
import { ToastService } from "../services/alert.services";
import { Router } from "@angular/router";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(TokenStorageService);
    const refreshState = inject(RefreshTokenState);
    const alertService = inject(ToastService);
    const router = inject(Router);
    // skip refresh api
    if (req.url.includes('/refresh')) {
        return next(req);
    }

    const token = authService.getToken();
    if (!token) {
        return next(req);
    }
    const authReq = addTokenHeader(req, token);

    return next(authReq).pipe(
        catchError((error: HttpErrorResponse) => {

            if (error.status === 401) {
                return handle401Error(
                    req,
                    next,
                    authService,
                    refreshState,
                    router
                );
            }
            handleErrorStatus(error, authService, alertService);
            return throwError(() => error);
        })
    );
};