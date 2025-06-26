import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RoleDto {
  roleName: string;
  permissions: string[];
}

export interface CreateRoleRequest {
  roleName: string;
  permissions: string[];
}

export interface UpdateRoleRequest {
  oldRoleName : string;
  newRoleName: string;
  permissions: string[];
}

export interface AssignPermissionRequest {
  roleName: string;
  permissions: string[];
}

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private baseUrl = 'http://localhost:5063/api/role'; // Adjust this path as needed

  constructor(private http: HttpClient) {}

  getAllRoles(): Observable<RoleDto[]> {
    return this.http.get<RoleDto[]>(`${this.baseUrl}`);
  }

  createRole(data: CreateRoleRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}`, data);
  }

  updateRole(data: UpdateRoleRequest): Observable<any> {
    return this.http.put(`${this.baseUrl}`, data);
  }

  deleteRole(roleName: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${roleName}`);
  }

  assignPermissions(data: AssignPermissionRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/assign-permissions`, data);
  }

  getAllPermissions(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/permissions`);
  }

}
