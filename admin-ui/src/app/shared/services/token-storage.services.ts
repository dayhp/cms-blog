import { UserModel } from "../models/usser.model";

const TOKEN_KEY = 'auth-token';
const USER_KEY = 'auth-user';
const REFRESH_TOKEN_KEY = 'auth-refresh-token';
export class TokenStorageService {
    constructor() { }

    public signOut(): void {
        window.localStorage.clear();
    }

    public saveToken(token: string): void {
        window.localStorage.removeItem(TOKEN_KEY);
        window.localStorage.setItem(TOKEN_KEY, token);

        const user = this.getUser();
        if (user && user.id) {
            this.saveUser({...user, accessToken: token});
        }
    }

    public getToken(): string | null {
        return window.localStorage.getItem(TOKEN_KEY);
    }

    public saveRefreshToken(refreshToken: string): void {
        window.localStorage.removeItem(REFRESH_TOKEN_KEY);
        window.localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
    }

    public getRefreshToken(): string | null {
        return window.localStorage.getItem(REFRESH_TOKEN_KEY);
    }

    public saveUser(user: any): void {
        window.localStorage.removeItem(USER_KEY);
        window.localStorage.setItem(USER_KEY, JSON.stringify(user));
    }

    public getUser(): UserModel | null {
        const user = window.localStorage.getItem(USER_KEY);
        if (!user) {
            return null;
        }
        const base64Url = user.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const userData: UserModel = JSON.parse(this.b64DecodeUnicode(base64));
        return userData;
    }

    // private dbase64Decode(str: string): string {
    //     const decodedStr = atob(str);
    //     return decodedStr;
    // }

    private b64DecodeUnicode(str: string): string {

        const decodedStr = decodeURIComponent(Array.prototype.map.call(atob(str), (c) => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return decodedStr;
    }
}