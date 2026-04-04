import { Inject } from "@angular/core";
import { BehaviorSubject } from "rxjs";
@Inject({
    providedIn: 'root'
})
export class RefreshTokenState {
    public isRefreshing = false;
    public  refreshTokenSubject = new BehaviorSubject<string | null>(null);

}