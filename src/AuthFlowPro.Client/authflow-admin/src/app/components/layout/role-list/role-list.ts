import { Component, OnInit , ViewChild, TemplateRef} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RoleService, RoleDto } from '../../../services/role-service'; // adjust path as needed
import { RoleDialogComponent } from './role-dialog/role-dialog';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-role-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule
  ],
  templateUrl: './role-list.html',
  styleUrl: './role-list.css',
})
export class RoleListComponent implements OnInit {
  displayedColumns: string[] = ['name', 'permissions', 'actions'];
  roles: RoleDto[] = [];

  @ViewChild('confirmDeleteDialog') confirmDeleteDialog!: TemplateRef<any>;

  constructor(
    private roleService: RoleService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles() {
    this.roleService.getAllRoles().subscribe({
      next: (res) => (this.roles = res),
      error: () => console.error('Failed to load roles'),
    });
  }

  openCreateRoleDialog() {
    const dialogRef = this.dialog.open(RoleDialogComponent, {
      width: '400px',
      data: { roleName: '', permissions: [] },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.roleService
          .createRole({
            roleName: result.roleName,
            permissions: result.permissions,
          })
          .subscribe({
            next: () => {
              this.snackBar.open('Role created successfully', 'Close', {
                duration: 3000,
              }),
                this.loadRoles();
            },
            error: () =>
              this.snackBar.open('error creating role', 'Close', {
                duration: 3000,
              }),
          });
      }
    });
  }

  editRole(role: RoleDto) {
    const dialogRef = this.dialog.open(RoleDialogComponent, {
      width: '400px',
      data: { roleName: role.roleName, permissions: role.permissions },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.roleService
          .updateRole({
            oldRoleName: result.oldRoleName,
            newRoleName: result.roleName,
            permissions: result.permissions,
          })
          .subscribe({
            next: () => {
              this.snackBar.open('Role updated successfully', 'Close', {
                duration: 3000,
              }),
              this.loadRoles()},
            error: () => this.snackBar.open('error updating role', 'Close', {
                duration: 3000,
              })
          });
      }
    });
  }

  deleteRole(role: RoleDto) {
  const dialogRef = this.dialog.open(this.confirmDeleteDialog, {
    width: '350px',
    data: role.roleName
  });

  dialogRef.afterClosed().subscribe(confirmed => {
    if (confirmed) {
      this.roleService.deleteRole(role.roleName).subscribe({
        next: () => {
          this.loadRoles();
          this.snackBar.open(`Role "${role.roleName}" deleted.`, 'Close', { duration: 3000 });
        },
        error: () => this.snackBar.open('Failed to delete role', 'Close', { duration: 3000 })
      });
    }
  });
}

}
