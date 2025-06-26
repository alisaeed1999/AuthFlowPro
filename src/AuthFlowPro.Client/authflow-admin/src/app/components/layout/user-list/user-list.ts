import { Component, OnInit , TemplateRef , ViewChild, viewChild } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { UserService } from '../../../services/user-service';
import { User } from '../../../models/user-model';
import { UpdateUserRoles } from '../../../models/update-user-role-model';
import { AuthService } from '../../../services/auth-service';
import { MatDialog , MatDialogModule } from '@angular/material/dialog';
import { UserDialogComponent } from './user-dialog/user-dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIcon } from '@angular/material/icon';
import { MatButton, MatButtonModule, MatIconButton } from '@angular/material/button';


@Component({
  selector: 'app-user-list',
  imports: [CommonModule, 
    MatTableModule, 
    MatCardModule, 
    AsyncPipe  , 
    UserDialogComponent,
    MatIconButton , 
    MatIcon,
    MatDialogModule,
    MatButtonModule
   ],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css',
})



export class UserListComponent implements OnInit {
  users: User[] = [];
  displayedColumns: string[] = ['no.', 'name', 'email', 'role' , 'actions'];

  @ViewChild('confirmDeleteDialog') confirmDeleteDialog!: TemplateRef<any>;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private dialog : MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.userService.getAllUsers().subscribe((users) => {
        this.users = users;
      });
    } else {
      console.warn('Not authenticated yet');
    }
  }

  openCreateUserDialog(): void {

  const dialogRef = this.dialog.open(UserDialogComponent);

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.userService.createUser({
        email : result.email,
        userName:result.userName,
        password :result.password,
        roles : result.roles
      }).subscribe({
        next: () => {
          this.snackBar.open('User created successfully!', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top',
          panelClass: ['snackbar-success']
        });
          this.ngOnInit(); // reload users
        },
        error: () => this.snackBar.open('Failed to create user.', 'Close', {
          duration: 3000,
          horizontalPosition: 'end',
          verticalPosition: 'top',
          panelClass: ['snackbar-error']
        })
      });
    }
  });
}

openEditUserDialog(user: User): void {
  const dialogRef = this.dialog.open(UserDialogComponent, {
    width: '400px',
    data: { user }  // pass user to edit
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result) {
      this.userService.editUser(result).subscribe({
        next: () => {
          this.snackBar.open('User updated!', 'Close', {
            duration: 3000,
            panelClass: 'snackbar-success'
          });
          this.ngOnInit(); // refresh list
        },
        error: () => {
          this.snackBar.open('Error updating user', 'Close', {
            duration: 3000,
            panelClass: 'snackbar-error'
          });
        }
      });
    }
  });
}


editUser(user: User): void {
  // Optional: open the dialog pre-filled for editing if needed
  this.snackBar.open('Edit feature not implemented yet.', 'Close', {
    duration: 2500
  });
}

onDelete(user: User) {
  const dialogRef = this.dialog.open(this.confirmDeleteDialog, {
    width: '350px',
    disableClose: true,
    data: null
  });

  dialogRef.afterClosed().subscribe(result => {
    if (result === true) {
      this.userService.deleteUser(user.id).subscribe({
        next: () => {
          this.snackBar.open('User deleted successfully.', 'Close', { duration: 3000 });
          this.ngOnInit();
        },
        error: () => {
          this.snackBar.open('Failed to delete user.', 'Close', { duration: 3000 });
        }
      });
    }
  });
}

}
