// auth-interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './services/auth-service';
import { HttpRequest, HttpHandlerFn, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

export const authInterceptor: HttpInterceptorFn = (
  req: HttpRequest<any>,
  next: HttpHandlerFn
): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);

  const token = authService.getToken();
  let cloned = req;

  if (token) {
    cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      },
      withCredentials: true
    });
  }

  return next(cloned).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        return authService.refreshToken().pipe(
          switchMap(res => {
            if (res.isSuccess) {
              const newReq = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${res.accessToken}`
                },
                withCredentials: true
              });
              return next(newReq);
            } else {
              authService.logout();
              return throwError(() => error);
            }
          }),
          catchError(err => {
            authService.logout();
            return throwError(() => err);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
