// src/app/core/services/auth.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, EMPTY, map, Observable, of, tap } from 'rxjs';
import { Router } from '@angular/router';

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  email: string;
  userName: string
  password: string;
}

interface AuthResponse {
  isSuccess: boolean;
  accessToken: string;
  // refreshToken: string;
  expiresAt: Date | null;
  errors?: string[];
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'http://localhost:5063/api/auth'; // adjust for your API
  private tokenKey = 'access_token';

  private _isAuthenticated = new BehaviorSubject<boolean>(false);
  public isAuthenticatedVar = this._isAuthenticated.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    const token = localStorage.getItem(this.tokenKey);
    this._isAuthenticated.next(!!token);
  }

  login(data: LoginRequest) {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/login`, data, {
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          if (res.isSuccess) {
            localStorage.setItem(this.tokenKey, res.accessToken);
            this._isAuthenticated.next(true);
            this.router.navigate(['/dashboard']);
          }
        }),
        catchError((error) => {
          console.error('Login error', error);
          return of({
            isSuccess: false,
            errors: ['Login failed'],
          } as AuthResponse);
        })
      );
  }

  register(data: RegisterRequest) {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/register`, data, {
        withCredentials: true,
      })
      .pipe(
        tap((res) => {
          if (res.isSuccess) {
            localStorage.setItem(this.tokenKey, res.accessToken);
            this._isAuthenticated.next(true);
            
          }
        }),
        catchError((error) => {
          console.error('Register error', error);
          return of({
            isSuccess: false,
            errors: ['Registration failed'],
          } as AuthResponse);
        })
      );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this._isAuthenticated.next(false);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
  const token = this.getToken(); // not getAccessToken()
  return !!token;
}

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  refreshToken(): Observable<AuthResponse> {
    const accessToken = this.getToken();

    if (!accessToken) {
      return of({
        isSuccess: false,
        errors: ['No access token'],
        accessToken: '',
        expiresAt: null,
      });
    }

    return this.http
      .post<AuthResponse>(
        `${this.baseUrl}/refresh-token`,
        { accessToken },
        { withCredentials: true } // send cookies with the request
      )
      .pipe(
        tap((res) => {
          if (res.isSuccess) {
            localStorage.setItem(this.tokenKey, res.accessToken);
          }
        }),
        catchError((error) => {
          this.logout();
          return of({
            isSuccess: false,
            errors: ['Session expired'],
          } as AuthResponse);
        })
      );
  }
}
