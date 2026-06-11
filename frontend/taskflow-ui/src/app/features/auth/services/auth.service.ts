import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { RegisterRequest } from "../models/register-request";
import { Observable, tap } from "rxjs";
import { RegisterResponse } from "../models/register-response";
import { LoginRequest } from "../models/login-request";
import { LoginResponse } from "../models/login-response";

@Injectable({
    providedIn: `root`
})
export class AuthService{
    private readonly api = inject(ApiService);
    private readonly tokenKey = `access_token`;

    register(
        request: RegisterRequest
    ): Observable<RegisterResponse>{
        return this.api.post<RegisterResponse>(
            'Auth/register',
            request
        );
    }

    login(
        request: LoginRequest
    ): Observable<LoginResponse>{
        return this.api.post<LoginResponse>(
            'Auth/login',
            request
        )
        .pipe(
            tap(response => {
                localStorage.setItem(
                    this.tokenKey,
                    response.accessToken
                );
            })
        );
    }

    logout(): void{
        localStorage.removeItem(this.tokenKey);
    }

    getToken(): string | null{
        return localStorage.getItem(this.tokenKey);
    }

    isAuthenticated(): boolean{
        return !!this.getToken();
    }

}