import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { TokenStorageService } from './services/token-storage.services';
@Injectable()
export class AuthGuard {
    constructor(private router: Router, private tokenService: TokenStorageService) { }
    canActivate(activatedRoute: ActivatedRouteSnapshot, routerState: RouterStateSnapshot): boolean {
        let requiredPolicy = activatedRoute.data['requiredPolicy'] as string;
        const loggedUser = this.tokenService.getUser();
        if (loggedUser) {
            const listPermissions = JSON.parse(loggedUser.permissions);
            if (listPermissions != null && listPermissions != '' 
                && listPermissions.filter((x: string) => x == requiredPolicy).length > 0) {
                return true;
            } else {
                this.router.navigate(['/forbidden'], { queryParams: { returnUrl: routerState.url } });
            }
        }
        return false;
    }
}