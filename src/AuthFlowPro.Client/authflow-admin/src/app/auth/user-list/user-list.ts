import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule, MatTableModule, MatCardModule],
  templateUrl: './user-list.html',
  styleUrl: './user-list.css'
})
export class UserListComponent {
  displayedColumns: string[] = ['id', 'name', 'email', 'role'];
  users = [
    { id: 1, name: 'Ali Saeed', email: 'ali@example.com', role: 'Admin' },
    { id: 2, name: 'John Doe', email: 'john@example.com', role: 'Manager' }
  ];
}
