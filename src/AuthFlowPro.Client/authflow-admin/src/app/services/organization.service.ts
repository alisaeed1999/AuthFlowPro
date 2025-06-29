import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Organization {
  id: string;
  name: string;
  slug: string;
  description?: string;
  website?: string;
  logo?: string;
  createdAt: Date;
  isActive: boolean;
  memberCount: number;
  subscription?: Subscription;
}

export interface Subscription {
  id: string;
  planId: string;
  planName: string;
  price: number;
  currency: string;
  interval: 'Monthly' | 'Yearly';
  status: 'Active' | 'Cancelled' | 'PastDue' | 'Unpaid' | 'Trialing';
  currentPeriodStart: Date;
  currentPeriodEnd: Date;
  autoRenew: boolean;
}

export interface OrganizationMember {
  id: string;
  userId: string;
  email: string;
  fullName: string;
  avatar?: string;
  role: 'Owner' | 'Admin' | 'Member';
  joinedAt: Date;
  isActive: boolean;
}

export interface CreateOrganizationRequest {
  name: string;
  slug: string;
  description?: string;
  website?: string;
}

export interface InviteMemberRequest {
  email: string;
  role: 'Owner' | 'Admin' | 'Member';
}

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private baseUrl = 'http://localhost:5063/api/organization';

  constructor(private http: HttpClient) {}

  getMyOrganizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(`${this.baseUrl}/my-organizations`);
  }

  getOrganization(id: string): Observable<Organization> {
    return this.http.get<Organization>(`${this.baseUrl}/${id}`);
  }

  createOrganization(request: CreateOrganizationRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}`, request);
  }

  updateOrganization(id: string, request: Partial<CreateOrganizationRequest>): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, request);
  }

  deleteOrganization(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }

  getOrganizationMembers(id: string): Observable<OrganizationMember[]> {
    return this.http.get<OrganizationMember[]>(`${this.baseUrl}/${id}/members`);
  }

  inviteMember(organizationId: string, request: InviteMemberRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/${organizationId}/members/invite`, request);
  }

  updateMemberRole(organizationId: string, memberId: string, role: string): Observable<any> {
    return this.http.put(`${this.baseUrl}/${organizationId}/members/role`, { memberId, role });
  }

  removeMember(organizationId: string, memberId: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${organizationId}/members/${memberId}`);
  }
}