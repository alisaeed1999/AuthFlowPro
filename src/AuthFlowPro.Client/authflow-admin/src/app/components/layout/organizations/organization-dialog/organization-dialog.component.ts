import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Organization } from '../../../../services/organization.service';

@Component({
  selector: 'app-organization-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule
  ],
  template: `
    <h2 mat-dialog-title>{{ data.isEdit ? 'Edit Organization' : 'Create Organization' }}</h2>

    <form [formGroup]="form" (ngSubmit)="onSubmit()" class="dialog-form">
      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Organization Name</mat-label>
        <input matInput formControlName="name" placeholder="Enter organization name">
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Slug</mat-label>
        <input matInput formControlName="slug" placeholder="organization-slug">
        <mat-hint>This will be used in URLs</mat-hint>
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Description</mat-label>
        <textarea matInput formControlName="description" rows="3" placeholder="Optional description"></textarea>
      </mat-form-field>

      <mat-form-field appearance="fill" class="full-width">
        <mat-label>Website</mat-label>
        <input matInput formControlName="website" placeholder="https://example.com">
      </mat-form-field>

      <div class="actions">
        <button mat-button type="button" (click)="onCancel()">Cancel</button>
        <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid">
          {{ data.isEdit ? 'Update' : 'Create' }}
        </button>
      </div>
    </form>
  `,
  styles: [`
    .dialog-form {
      display: flex;
      flex-direction: column;
      padding: 16px 0;
    }

    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }

    .actions {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
      margin-top: 16px;
    }
  `]
})
export class OrganizationDialogComponent implements OnInit {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<OrganizationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { isEdit: boolean; organization?: Organization }
  ) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      slug: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      description: [''],
      website: ['', Validators.pattern(/^https?:\/\/.+/)]
    });
  }

  ngOnInit(): void {
    if (this.data.isEdit && this.data.organization) {
      this.form.patchValue({
        name: this.data.organization.name,
        slug: this.data.organization.slug,
        description: this.data.organization.description,
        website: this.data.organization.website
      });
    }

    // Auto-generate slug from name
    if (!this.data.isEdit) {
      this.form.get('name')?.valueChanges.subscribe(name => {
        if (name) {
          const slug = name.toLowerCase().replace(/[^a-z0-9]/g, '-').replace(/-+/g, '-').replace(/^-|-$/g, '');
          this.form.get('slug')?.setValue(slug);
        }
      });
    }
  }

  onSubmit(): void {
    if (this.form.valid) {
      this.dialogRef.close(this.form.value);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}