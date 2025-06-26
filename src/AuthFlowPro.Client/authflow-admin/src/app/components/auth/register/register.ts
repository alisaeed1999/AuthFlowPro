import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth-service'; // adjust the path as needed

@Component({
  standalone: true,
  selector: 'app-register',
  templateUrl: './register.html',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    RouterLink,
    MatSnackBarModule,
  ],
  styleUrls: ['./register.css'],
})

export class RegisterComponent {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
    });
  }

  onSubmit() {
    if (
      this.form.valid &&
      this.form.value.password === this.form.value.confirmPassword
    ) {
      const { username, email, password } = this.form.value;

      this.authService
        .register({
          email: this.form.value.email,
          userName: this.form.value.username, // note this should match your backend casing
          password: this.form.value.password,
        })
        .subscribe({
          next: (res) => {
            if (res.isSuccess) {
              // Save token if needed or just navigate
              this.router.navigate(['/admin/users']);
              this.snackBar.open('Registration successful!', 'Close', {
                duration: 3000,
                horizontalPosition: 'end',
                verticalPosition: 'top',
                panelClass: ['snackbar-success'],
              });
            } else {
              console.error(res.errors);
              this.snackBar.open(
                res.errors?.join(', ') || 'Registration failed',
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
