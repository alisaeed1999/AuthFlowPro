import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { CommonModule } from '@angular/common';
import { MatOption } from '@angular/material/autocomplete';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
@Component({
  selector: 'app-user-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatOption,
    MatCheckbox,
    MatSelectModule
  ],
  templateUrl: './user-dialog.html',
  styleUrl: './user-dialog.css'
})

export class UserDialogComponent implements OnInit {
  form: FormGroup;
  availableRoles: string[] = ['Admin', 'Basic', 'Manager'];
  isEditMode = false;

  constructor(
    private dialogRef: MatDialogRef<UserDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, // receive data for edit
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      id: [''],
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: [''],
      confirmPassword: [''],
      roles: [[]]
    });

    if (data && data.user) {
      this.isEditMode = true;
      this.form.patchValue({
        id: data.user.id,
        userName: data.user.userName,
        email: data.user.email,
        roles: data.user.roles
      });
    }
  }

  ngOnInit() {}

  onSubmit(): void {
    if (this.form.valid) {
      // If creating: password required. If editing: skip unless changed.
      if (!this.isEditMode || this.form.value.password === this.form.value.confirmPassword) {
        // only send roles that are selected
        const selectedRoles = this.form.value.roles || [];
        const result = { ...this.form.value, roles: selectedRoles };
        this.dialogRef.close(result);
      }
    }
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}