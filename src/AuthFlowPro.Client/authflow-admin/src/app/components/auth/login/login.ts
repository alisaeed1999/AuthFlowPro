import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../services/auth-service';

@Component({
  standalone: true,
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    RouterLink,
    MatSnackBarModule,
  ],
})
export class LoginComponent {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onSubmit() {
    if (
      this.form.valid 
    ) {
      const { email, password } = this.form.value;

      this.authService
        .login({
          email: this.form.value.email, // note this should match your backend casing
          password: this.form.value.password,
        })
        .subscribe({
          next: (res) => {
            if (res.isSuccess) {
              // Save token if needed or just navigate
              this.router.navigate(['/admin/users']);
              this.snackBar.open('login successful!', 'Close', {
                duration: 3000,
                horizontalPosition: 'end',
                verticalPosition: 'top',
                panelClass: ['snackbar-success'],
              });
            } else {
              console.error(res.errors);
              this.snackBar.open(
                res.errors?.join(', ') || 'login failed',
                'Close',
                {
                  duration: 4000,
                  panelClass: ['snackbar-error'],
                }
              );
            }
          },
          error: (err) => {
            console.error(err);
            this.snackBar.open('Something went wrong!', 'Close', {
              duration: 4000,
              panelClass: ['snackbar-error'],
            });
          },
        });
    } else {
      alert('Please fill out the form correctly.');
    }
  }
}
