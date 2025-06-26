import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth-service'; 
import { HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of, ReplaySubject, filter, take } from 'rxjs';
import { catchError, switchMap, from } from 'rxjs';
import { isTokenExpiringSoon } from '../core/utils/jwt-utils'; // adjust path as needed
import { Router } from '@angular/router';

let isRefreshing = false;
const tokenSubject = new ReplaySubject<string | null>(1);
let refreshAttempts = 0;
const MAX_REFRESH_ATTEMPTS = 2;

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Attach access token if request is not for auth endpoint
  let modifiedReq = req.clone({ withCredentials: true });
  if (!isAuthEndpoint(req.url)) {
    const token = authService.getToken();
    if (token) {
      modifiedReq = modifiedReq.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
  }

  return next(modifiedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !isAuthEndpoint(req.url)) {
        return handle401Error(modifiedReq, next, authService, router);
      }
      return throwError(() => error);
    })
  );
};

function isAuthEndpoint(url: string): boolean {
  return url.includes('/login') || url.includes('/register') || url.includes('/refresh-token');
}

function handle401Error(
  request: HttpRequest<any>,
  next: HttpHandlerFn,
  authService: AuthService,
  router: Router
): Observable<HttpEvent<any>> {
  if (refreshAttempts >= MAX_REFRESH_ATTEMPTS) {
    authService.logout();
    router.navigate(['/login']);
    return throwError(() => new Error('Max refresh attempts reached'));
  }

  if (!isRefreshing) {
    isRefreshing = true;
    tokenSubject.next(null);
    refreshAttempts++;

    return authService.refreshToken().pipe(
      switchMap(() => {
        isRefreshing = false;
        refreshAttempts = 0;
        const newToken = authService.getToken();
        tokenSubject.next(newToken);
        return next(handleToken(request, newToken));
      }),
      catchError(err => {
        isRefreshing = false;
        authService.logout();
        router.navigate(['/login']);
        return throwError(() => err);
      })
    );
  }

  return tokenSubject.pipe(
    filter(token => token !== null),
    take(1),
    switchMap(token => next(handleToken(request, token)))
  );
}

function handleToken(request: HttpRequest<any>, token: string | null): HttpRequest<any> {
  if (token) {
    return request.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  return request;
}