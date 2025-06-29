import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrganizationService, Organization } from '../../../services/organization.service';
import { OrganizationDialogComponent } from './organization-dialog/organization-dialog.component';

@Component({
  selector: 'app-organizations',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatDialogModule
  ],
  template: `
    <mat-card>
      <div class="header-row">
        <mat-card-title>My Organizations</mat-card-title>
        <mat-card-actions>
          <button
            mat-raised-button
            color="primary"
            class="create-btn"
            (click)="openCreateDialog()">
            <mat-icon>add</mat-icon>Create Organization
          </button>
        </mat-card-actions>
      </div>

      <mat-table [dataSource]="organizations">
        <ng-container matColumnDef="name">
          <mat-header-cell *matHeaderCellDef>Name</mat-header-cell>
          <mat-cell *matCellDef="let org">
            <div class="org-info">
              <div class="org-name">{{ org.name }}</div>
              <div class="org-slug">{{ org.slug }}</div>
            </div>
          </mat-cell>
        </ng-container>

        <ng-container matColumnDef="members">
          <mat-header-cell *matHeaderCellDef>Members</mat-header-cell>
          <mat-cell *matCellDef="let org">{{ org.memberCount }}</mat-cell>
        </ng-container>

        <ng-container matColumnDef="subscription">
          <mat-header-cell *matHeaderCellDef>Plan</mat-header-cell>
          <mat-cell *matCellDef="let org">
            <span class="plan-badge" [class]="'plan-' + (org.subscription?.planId || 'free')">
              {{ org.subscription?.planName || 'Free' }}
            </span>
          </mat-cell>
        </ng-container>

        <ng-container matColumnDef="status">
          <mat-header-cell *matHeaderCellDef>Status</mat-header-cell>
          <mat-cell *matCellDef="let org">
            <span class="status-badge" [class]="org.isActive ? 'status-active' : 'status-inactive'">
              {{ org.isActive ? 'Active' : 'Inactive' }}
            </span>
          </mat-cell>
        </ng-container>

        <ng-container matColumnDef="actions">
          <mat-header-cell *matHeaderCellDef>Actions</mat-header-cell>
          <mat-cell *matCellDef="let org">
            <button mat-icon-button (click)="editOrganization(org)">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button (click)="viewMembers(org)">
              <mat-icon>people</mat-icon>
            </button>
            <button mat-icon-button color="warn" (click)="deleteOrganization(org)">
              <mat-icon>delete</mat-icon>
            </button>
          </mat-cell>
        </ng-container>

        <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
        <mat-row *matRowDef="let row; columns: displayedColumns"></mat-row>
      </mat-table>
    </mat-card>
  `,
  styles: [`
    .header-row {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px;
    }

    .create-btn {
      border-radius: 20px;
      padding: 6px 16px;
      font-weight: 500;
    }

    .org-info {
      display: flex;
      flex-direction: column;
    }

    .org-name {
      font-weight: 500;
      font-size: 14px;
    }

    .org-slug {
      font-size: 12px;
      color: #666;
    }

    .plan-badge, .status-badge {
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
    }

    .plan-starter {
      background-color: #e3f2fd;
      color: #1976d2;
    }

    .plan-pro {
      background-color: #f3e5f5;
      color: #7b1fa2;
    }

    .plan-enterprise {
      background-color: #fff3e0;
      color: #f57c00;
    }

    .plan-free {
      background-color: #f5f5f5;
      color: #666;
    }

    .status-active {
      background-color: #e8f5e8;
      color: #2e7d32;
    }

    .status-inactive {
      background-color: #ffebee;
      color: #c62828;
    }
  `]
})
export class OrganizationsComponent implements OnInit {
  organizations: Organization[] = [];
  displayedColumns: string[] = ['name', 'members', 'subscription', 'status', 'actions'];

  constructor(
    private organizationService: OrganizationService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadOrganizations();
  }

  loadOrganizations(): void {
    this.organizationService.getMyOrganizations().subscribe({
      next: (organizations) => {
        this.organizations = organizations;
      },
      error: (error) => {
        this.snackBar.open('Failed to load organizations', 'Close', { duration: 3000 });
      }
    });
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(OrganizationDialogComponent, {
      width: '500px',
      data: { isEdit: false }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.organizationService.createOrganization(result).subscribe({
          next: () => {
            this.snackBar.open('Organization created successfully', 'Close', { duration: 3000 });
            this.loadOrganizations();
          },
          error: () => {
            this.snackBar.open('Failed to create organization', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  editOrganization(organization: Organization): void {
    const dialogRef = this.dialog.open(OrganizationDialogComponent, {
      width: '500px',
      data: { isEdit: true, organization }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.organizationService.updateOrganization(organization.id, result).subscribe({
          next: () => {
            this.snackBar.open('Organization updated successfully', 'Close', { duration: 3000 });
            this.loadOrganizations();
          },
          error: () => {
            this.snackBar.open('Failed to update organization', 'Close', { duration: 3000 });
          }
        });
      }
    });
  }

  viewMembers(organization: Organization): void {
    // Navigate to members view or open members dialog
    console.log('View members for:', organization.name);
  }

  deleteOrganization(organization: Organization): void {
    if (confirm(`Are you sure you want to delete "${organization.name}"?`)) {
      this.organizationService.deleteOrganization(organization.id).subscribe({
        next: () => {
          this.snackBar.open('Organization deleted successfully', 'Close', { duration: 3000 });
          this.loadOrganizations();
        },
        error: () => {
          this.snackBar.open('Failed to delete organization', 'Close', { duration: 3000 });
        }
      });
    }
  }
}