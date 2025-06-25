import { Component, OnInit } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { UserService } from '../../services/user-service';
import { User } from '../../models/user-model';
import { UpdateUserRoles } from '../../models/update-user-role-model';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule, MatTableModule, MatCardModule, AsyncPipe],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css',
})
export class UserListComponent implements OnInit {
  users: User[] = [];
  displayedColumns: string[] = ['id', 'name', 'email', 'role'];

  constructor(
    private userService: UserService,
    private authService: AuthService
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

  // onUpdateRoles(userId: string) {
  //   const roles = this.roleForms[userId].value.roles;
  //   const payload: UpdateUserRoles = { userId, roles };
  //   this.userService.updateUserRoles(payload).subscribe({
  //     next: () => alert('Roles updated!'),
  //     error: () => alert('Error updating roles')
  //   });
  // }
}
