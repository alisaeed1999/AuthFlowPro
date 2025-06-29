import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

interface AuditLog {
  id: string;
  action: string;
  entityType: string;
  entityId?: string;
  userName?: string;
  details?: string;
  ipAddress: string;
  createdAt: Date;
}

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    ReactiveFormsModule
  ],
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Audit Logs</mat-card-title>
      </mat-card-header>
      
      <mat-card-content>
        <!-- Filters -->
        <form [formGroup]="filterForm" class="filters-form">
          <div class="filters-row">
            <mat-form-field appearance="outline">
              <mat-label>Action</mat-label>
              <mat-select formControlName="action">
                <mat-option value="">All Actions</mat-option>
                <mat-option value="user.created">User Created</mat-option>
                <mat-option value="user.updated">User Updated</mat-option>
                <mat-option value="user.deleted">User Deleted</mat-option>
                <mat-option value="role.created">Role Created</mat-option>
                <mat-option value="role.updated">Role Updated</mat-option>
                <mat-option value="organization.created">Organization Created</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Entity Type</mat-label>
              <mat-select formControlName="entityType">
                <mat-option value="">All Types</mat-option>
                <mat-option value="User">User</mat-option>
                <mat-option value="Role">Role</mat-option>
                <mat-option value="Organization">Organization</mat-option>
                <mat-option value="Subscription">Subscription</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>From Date</mat-label>
              <input matInput [matDatepicker]="fromPicker" formControlName="fromDate">
              <mat-datepicker-toggle matSuffix [for]="fromPicker"></mat-datepicker-toggle>
              <mat-datepicker #fromPicker></mat-datepicker>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>To Date</mat-label>
              <input matInput [matDatepicker]="toPicker" formControlName="toDate">
              <mat-datepicker-toggle matSuffix [for]="toPicker"></mat-datepicker-toggle>
              <mat-datepicker #toPicker></mat-datepicker>
            </mat-form-field>

            <button mat-raised-button color="primary" (click)="applyFilters()">
              <mat-icon>search</mat-icon>
              Filter
            </button>

            <button mat-button (click)="clearFilters()">
              <mat-icon>clear</mat-icon>
              Clear
            </button>
          </div>
        </form>

        <!-- Audit Logs Table -->
        <mat-table [dataSource]="auditLogs" class="audit-table">
          <ng-container matColumnDef="timestamp">
            <mat-header-cell *matHeaderCellDef>Timestamp</mat-header-cell>
            <mat-cell *matCellDef="let log">
              {{ log.createdAt | date:'medium' }}
            </mat-cell>
          </ng-container>

          <ng-container matColumnDef="user">
            <mat-header-cell *matHeaderCellDef>User</mat-header-cell>
            <mat-cell *matCellDef="let log">
              {{ log.userName || 'System' }}
            </mat-cell>
          </ng-container>

          <ng-container matColumnDef="action">
            <mat-header-cell *matHeaderCellDef>Action</mat-header-cell>
            <mat-cell *matCellDef="let log">
              <span class="action-badge" [class]="getActionClass(log.action)">
                {{ log.action }}
              </span>
            </mat-cell>
          </ng-container>

          <ng-container matColumnDef="entity">
            <mat-header-cell *matHeaderCellDef>Entity</mat-header-cell>
            <mat-cell *matCellDef="let log">
              <div class="entity-info">
                <div class="entity-type">{{ log.entityType }}</div>
                <div class="entity-id" *ngIf="log.entityId">{{ log.entityId }}</div>
              </div>
            </mat-cell>
          </ng-container>

          <ng-container matColumnDef="ipAddress">
            <mat-header-cell *matHeaderCellDef>IP Address</mat-header-cell>
            <mat-cell *matCellDef="let log">{{ log.ipAddress }}</mat-cell>
          </ng-container>

          <ng-container matColumnDef="details">
            <mat-header-cell *matHeaderCellDef>Details</mat-header-cell>
            <mat-cell *matCellDef="let log">
              <button mat-icon-button *ngIf="log.details" (click)="viewDetails(log)">
                <mat-icon>info</mat-icon>
              </button>
            </mat-cell>
          </ng-container>

          <mat-header-row *matHeaderRowDef="displayedColumns"></mat-header-row>
          <mat-row *matRowDef="let row; columns: displayedColumns"></mat-row>
        </mat-table>

        <mat-paginator 
          [pageSizeOptions]="[10, 25, 50, 100]"
          [pageSize]="20"
          showFirstLastButtons>
        </mat-paginator>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .filters-form {
      margin-bottom: 24px;
    }

    .filters-row {
      display: flex;
      gap: 16px;
      align-items: center;
      flex-wrap: wrap;
    }

    .filters-row mat-form-field {
      min-width: 150px;
    }

    .audit-table {
      width: 100%;
      margin-bottom: 16px;
    }

    .action-badge {
      padding: 4px 8px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
    }

    .action-created {
      background-color: #e8f5e8;
      color: #2e7d32;
    }

    .action-updated {
      background-color: #fff3e0;
      color: #f57c00;
    }

    .action-deleted {
      background-color: #ffebee;
      color: #c62828;
    }

    .action-default {
      background-color: #f5f5f5;
      color: #666;
    }

    .entity-info {
      display: flex;
      flex-direction: column;
    }

    .entity-type {
      font-weight: 500;
      font-size: 14px;
    }

    .entity-id {
      font-size: 12px;
      color: #666;
      font-family: monospace;
    }
  `]
})
export class AuditLogsComponent implements OnInit {
  auditLogs: AuditLog[] = [];
  displayedColumns: string[] = ['timestamp', 'user', 'action', 'entity', 'ipAddress', 'details'];
  filterForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.filterForm = this.fb.group({
      action: [''],
      entityType: [''],
      fromDate: [''],
      toDate: ['']
    });
  }

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    // Mock data for demonstration
    this.auditLogs = [
      {
        id: '1',
        action: 'user.created',
        entityType: 'User',
        entityId: 'user-123',
        userName: 'Admin User',
        details: '{"email": "test@example.com"}',
        ipAddress: '192.168.1.1',
        createdAt: new Date()
      },
      {
        id: '2',
        action: 'role.updated',
        entityType: 'Role',
        entityId: 'role-456',
        userName: 'Admin User',
        details: '{"permissions": ["read", "write"]}',
        ipAddress: '192.168.1.1',
        createdAt: new Date(Date.now() - 3600000)
      }
    ];
  }

  applyFilters(): void {
    // Apply filters and reload data
    console.log('Applying filters:', this.filterForm.value);
    this.loadAuditLogs();
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.loadAuditLogs();
  }

  getActionClass(action: string): string {
    if (action.includes('created')) return 'action-created';
    if (action.includes('updated')) return 'action-updated';
    if (action.includes('deleted')) return 'action-deleted';
    return 'action-default';
  }

  viewDetails(log: AuditLog): void {
    // Open dialog with detailed information
    console.log('View details for:', log);
  }
}