import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-role-list',
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './role-list.html',
  styleUrl: './role-list.css'
})
export class RoleListComponent {
  displayedColumns: string[] = ['name', 'permissions', 'actions'];
  roles = [
    {
      name: 'Admin',
      permissions: ['View Users', 'Create Users', 'Delete Users']
    },
    {
      name: 'Manager',
      permissions: ['View Products', 'Create Products']
    },
    {
      name: 'Basic',
      permissions: ['View Products']
    }
  ];

  editRole(roleName: string) {
    console.log('Edit:', roleName);
  }

  deleteRole(roleName: string) {
    console.log('Delete:', roleName);
  }
}
