import { Component } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterLink } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-login',
  templateUrl: "./login.html",
  styleUrls : ["./login.css"],
  imports: [CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    RouterLink 
  ]
})
export class LoginComponent {
  
  form! : FormGroup
  
  constructor(private fb: FormBuilder, private router: Router) {
    this.form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });
  }

  onSubmit() {
    if (this.form.valid) {
      console.log('Logging in with:', this.form.value);
      // TODO: Call backend API
      // this.authService.login(this.form.value).subscribe(...)

      // Temporary: redirect to dashboard or some route
      this.router.navigate(['/dashboard']);
    }
  }
}
