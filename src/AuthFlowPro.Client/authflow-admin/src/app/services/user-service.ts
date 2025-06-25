import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from "../models/user-model";
import { UpdateUserRoles } from '../models/update-user-role-model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'http://localhost:5063/api/admin';

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<User[]> {
    
    return this.http.get<User[]>(this.baseUrl + "/users");
  }

  updateUserRoles(data: UpdateUserRoles): Observable<any> {
    return this.http.post(`${this.baseUrl}/assign-roles`, data);
  }
}
