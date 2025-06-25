import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of, tap } from 'rxjs';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';

interface LoginRequest {
  email: string;
  password: string;
}

interface RegisterRequest {
  email: string;
  userName: string;
  password: string;
}

interface AuthResponse {
  isSuccess: boolean;
  accessToken: string;
  expiresAt: Date | null;
  errors?: string[];
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseUrl = 'http://localhost:5063/api/auth';
  private tokenKey = 'access_token';

  private _isAuthenticated = new BehaviorSubject<boolean>(false);
  public isAuthenticatedVar = this._isAuthenticated.asObservable();

  private jwtHelper = new JwtHelperService();
  private refreshTimeout: any;

  constructor(private http: HttpClient, private router: Router) {
    const token = this.getToken();
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      this._isAuthenticated.next(true);
      this.startRefreshTimer(token);
    } else {
      this._isAuthenticated.next(false);
    }
  }

  login(data: LoginRequest) {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/login`, data, { withCredentials: true })
      .pipe(
        tap((res) => {
          if (res.isSuccess) {
            localStorage.setItem(this.tokenKey, res.accessToken);
            this._isAuthenticated.next(true);
            this.startRefreshTimer(res.accessToken);
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
      .post<AuthResponse>(`${this.baseUrl}/register`, data, { withCredentials: true })
      .pipe(
        tap((res) => {
          if (res.isSuccess) {
            localStorage.setItem(this.tokenKey, res.accessToken);
            this._isAuthenticated.next(true);
            this.startRefreshTimer(res.accessToken);
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
    this.stopRefreshTimer();
    this._isAuthenticated.next(false);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  refreshToken(): Observable<AuthResponse> {
    const accessToken = this.getToken();

    if (!accessToken || this.jwtHelper.isTokenExpired(accessToken)) {
      console.warn('[AuthService] No valid access token. Cannot refresh.');
      return of({
        isSuccess: false,
        errors: ['No access token'],
        accessToken: '',
        expiresAt: null,
      });
    }

    return this.http
      .post<AuthResponse>(`${this.baseUrl}/refresh-token`, { accessToken }, { withCredentials: true })
      .pipe(
        tap((res) => {
          
          if (res.isSuccess) {
            localStorage.setItem(this.tokenKey, res.accessToken);
            this._isAuthenticated.next(true);
            this.startRefreshTimer(res.accessToken);
          }
        }),
        catchError((error) => {
          console.error('[AuthService] Refresh token failed:', error);
          this.logout();
          return of({
            isSuccess: false,
            errors: ['Session expired'],
            accessToken: '',
            expiresAt: null,
          });
        })
      );
  }

  // Auto-refresh mechanism
  private startRefreshTimer(token: string) {
    this.stopRefreshTimer();

    const expiresAt = this.jwtHelper.getTokenExpirationDate(token);
    
    
    if (!expiresAt) return;

    const timeout = expiresAt.getTime() - Date.now() - 60 * 1000; // 1 minute before expiry

    if (timeout > 0) {
      this.refreshTimeout = setTimeout(() => {
        this.refreshToken().subscribe();
      }, timeout);
    }
  }

  private stopRefreshTimer() {
    if (this.refreshTimeout) {
      clearTimeout(this.refreshTimeout);
    }
  }
}
