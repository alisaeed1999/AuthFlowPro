import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from "../models/user-model";
import { UpdateUserRoles } from '../models/update-user-role-model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'http://localhost:5063/api/user';

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<User[]> {
    
    return this.http.get<User[]>(this.baseUrl + "/users");
  }

  updateUserRoles(data: UpdateUserRoles): Observable<any> {
    return this.http.post(`${this.baseUrl}/assign-roles`, data);
  }

  createUser(userData: {
    userName: string;
    email: string;
    password: string;
    roles: string[];
  }): Observable<any> {
    return this.http.post(`${this.baseUrl}/create-user`, userData);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/delete-user/${userId}`);
  }

  editUser(data: {
    id: string;
    userName: string;
    email: string;
    roles : string[]
  }): Observable<any> {
  return this.http.put(`${this.baseUrl}/edit-user`, data);
}
}
