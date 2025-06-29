import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';

interface Plan {
  id: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  interval: string;
  maxUsers: number;
  maxProjects: number;
  hasAdvancedFeatures: boolean;
  isActive: boolean;
}

@Component({
  selector: 'app-subscriptions',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule
  ],
  template: `
    <div class="subscriptions-container">
      <mat-card class="current-plan-card">
        <mat-card-header>
          <mat-card-title>Current Plan</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="plan-info">
            <h3>Free Plan</h3>
            <p>You're currently on the free plan</p>
            <div class="plan-features">
              <mat-chip-set>
                <mat-chip>5 Users</mat-chip>
                <mat-chip>10 Projects</mat-chip>
                <mat-chip>Basic Features</mat-chip>
              </mat-chip-set>
            </div>
          </div>
        </mat-card-content>
        <mat-card-actions>
          <button mat-raised-button color="primary">Upgrade Plan</button>
        </mat-card-actions>
      </mat-card>

      <mat-card class="plans-card">
        <mat-card-header>
          <mat-card-title>Available Plans</mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <div class="plans-grid">
            <div class="plan-card" *ngFor="let plan of plans">
              <div class="plan-header">
                <h3>{{ plan.name }}</h3>
                <div class="plan-price">
                  <span class="price">\${{ plan.price }}</span>
                  <span class="interval">/{{ plan.interval.toLowerCase() }}</span>
                </div>
              </div>
              <p class="plan-description">{{ plan.description }}</p>
              <div class="plan-features">
                <ul>
                  <li>{{ plan.maxUsers === -1 ? 'Unlimited' : plan.maxUsers }} Users</li>
                  <li>{{ plan.maxProjects === -1 ? 'Unlimited' : plan.maxProjects }} Projects</li>
                  <li *ngIf="plan.hasAdvancedFeatures">Advanced Features</li>
                  <li *ngIf="!plan.hasAdvancedFeatures">Basic Features</li>
                </ul>
              </div>
              <button mat-raised-button color="primary" class="select-plan-btn">
                Select Plan
              </button>
            </div>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .subscriptions-container {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    .current-plan-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .current-plan-card mat-card-title {
      color: white;
    }

    .plan-info h3 {
      margin: 0 0 8px 0;
      font-size: 24px;
    }

    .plan-features {
      margin-top: 16px;
    }

    .plans-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 24px;
      margin-top: 16px;
    }

    .plan-card {
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      padding: 24px;
      text-align: center;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .plan-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 24px rgba(0,0,0,0.1);
    }

    .plan-header h3 {
      margin: 0 0 8px 0;
      font-size: 20px;
      color: #333;
    }

    .plan-price {
      margin-bottom: 16px;
    }

    .price {
      font-size: 32px;
      font-weight: bold;
      color: #2196f3;
    }

    .interval {
      font-size: 14px;
      color: #666;
    }

    .plan-description {
      color: #666;
      margin-bottom: 24px;
    }

    .plan-features ul {
      list-style: none;
      padding: 0;
      margin: 0 0 24px 0;
    }

    .plan-features li {
      padding: 8px 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .plan-features li:last-child {
      border-bottom: none;
    }

    .select-plan-btn {
      width: 100%;
    }
  `]
})
export class SubscriptionsComponent implements OnInit {
  plans: Plan[] = [
    {
      id: 'starter',
      name: 'Starter',
      description: 'Perfect for small teams getting started',
      price: 9.99,
      currency: 'USD',
      interval: 'Monthly',
      maxUsers: 5,
      maxProjects: 10,
      hasAdvancedFeatures: false,
      isActive: true
    },
    {
      id: 'pro',
      name: 'Professional',
      description: 'For growing teams that need more features',
      price: 29.99,
      currency: 'USD',
      interval: 'Monthly',
      maxUsers: 25,
      maxProjects: 100,
      hasAdvancedFeatures: true,
      isActive: true
    },
    {
      id: 'enterprise',
      name: 'Enterprise',
      description: 'For large organizations with advanced needs',
      price: 99.99,
      currency: 'USD',
      interval: 'Monthly',
      maxUsers: -1,
      maxProjects: -1,
      hasAdvancedFeatures: true,
      isActive: true
    }
  ];

  ngOnInit(): void {
    // Load current subscription and available plans
  }
}