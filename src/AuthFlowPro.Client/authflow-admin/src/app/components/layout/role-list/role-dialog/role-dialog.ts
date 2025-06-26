import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RoleService } from '../../../../services/role-service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatOption } from '@angular/material/autocomplete';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-role-dialog',
  standalone : true,
  templateUrl: './role-dialog.html',
  styleUrls: ['./role-dialog.css'],
  imports : [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatOption,
    MatCheckbox,
    MatSelectModule]
})
export class RoleDialogComponent implements OnInit {
  form: FormGroup;
  availablePermissions: string[] = [];
  isEditMode = false;

  constructor(
    private dialogRef: MatDialogRef<RoleDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { roleName: string, permissions: string[] },
    private fb: FormBuilder,
    private roleService: RoleService
  ) {
    this.isEditMode = !!data?.roleName;

    this.form = this.fb.group({
      oldRoleName: [{ value: data?.roleName , disabled: true }],
      roleName: [data?.roleName || '', Validators.required],
      permissions: [data?.permissions || []]
    });
  }

  ngOnInit(): void {
    this.roleService.getAllPermissions().subscribe({
      next: (permissions: string[]) => {
        this.availablePermissions = permissions;
      },
      error: () => {
        console.error('Failed to load permissions');
      }
    });
  }

  onSubmit() {
  if (this.form.valid) {
    const formValue = this.form.getRawValue(); // includes disabled fields

    this.dialogRef.close({
      oldRoleName: this.isEditMode ? formValue.oldRoleName : formValue.roleName,
      roleName: formValue.roleName,
      permissions: formValue.permissions
    });
  }
}

  onCancel() {
    this.dialogRef.close(null);
  }
}