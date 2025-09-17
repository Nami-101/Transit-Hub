import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Observable, Subject, interval } from 'rxjs';
import { map, takeUntil, startWith } from 'rxjs/operators';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule
  ],
  template: `
    <mat-toolbar color="primary" class="navbar">
      <div class="navbar-container">
        <div class="navbar-brand">
          <a routerLink="/" class="brand-link">
            <mat-icon>directions_bus</mat-icon>
            <span class="brand-text">Transit Hub</span>
          </a>
        </div>

        <div class="navbar-nav">
          <ng-container *ngIf="isAuthenticated$ | async; else guestNav">
            <!-- Authenticated Navigation -->
            <a routerLink="/dashboard" 
               mat-button 
               routerLinkActive="active-link">
              <mat-icon>dashboard</mat-icon>
              Dashboard
            </a>
            
            <a routerLink="/bookings" 
               mat-button 
               routerLinkActive="active-link">
              <mat-icon>confirmation_number</mat-icon>
              My Bookings
            </a>

            <button mat-button [matMenuTriggerFor]="userMenu">
              <mat-icon>account_circle</mat-icon>
              <span>Account</span>
              <mat-icon>arrow_drop_down</mat-icon>
            </button>
            
            <mat-menu #userMenu="matMenu">
              <button mat-menu-item routerLink="/profile">
                <mat-icon>person</mat-icon>
                <span>Profile</span>
              </button>
              <button mat-menu-item (click)="logout()">
                <mat-icon>logout</mat-icon>
                <span>Logout</span>
              </button>
            </mat-menu>
          </ng-container>

          <ng-template #guestNav>
            <!-- Guest Navigation -->
            <a routerLink="/auth/login" 
               mat-button 
               routerLinkActive="active-link">
              <mat-icon>login</mat-icon>
              Login
            </a>
            
            <a routerLink="/auth/register" 
               mat-raised-button 
               color="accent"
               routerLinkActive="active-link">
              <mat-icon>person_add</mat-icon>
              Sign Up
            </a>
          </ng-template>
        </div>
      </div>
    </mat-toolbar>
  `,
  styles: [`
    .navbar {
      position: sticky;
      top: 0;
      z-index: 1000;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .navbar-container {
      display: flex;
      justify-content: space-between;
      align-items: center;
      width: 100%;
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 1rem;
    }

    .navbar-brand {
      display: flex;
      align-items: center;
    }

    .brand-link {
      display: flex;
      align-items: center;
      text-decoration: none;
      color: inherit;
      font-size: 1.25rem;
      font-weight: 500;
    }

    .brand-text {
      margin-left: 0.5rem;
    }

    .navbar-nav {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .navbar-nav a {
      display: flex;
      align-items: center;
      gap: 0.25rem;
      text-decoration: none;
    }

    .navbar-nav button {
      display: flex;
      align-items: center;
      gap: 0.25rem;
    }

    .active-link {
      background-color: rgba(255, 255, 255, 0.1) !important;
    }

    @media (max-width: 768px) {
      .navbar-container {
        padding: 0 0.5rem;
      }
      
      .navbar-nav {
        gap: 0.25rem;
      }

      .brand-text {
        display: none;
      }
    }
  `]
})
export class NavbarComponent implements OnInit, OnDestroy {
  isAuthenticated$: Observable<boolean>;
  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    // Create an observable that checks authentication status periodically
    this.isAuthenticated$ = interval(1000).pipe(
      startWith(0),
      map(() => this.authService.isAuthenticated()),
      takeUntil(this.destroy$)
    );
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/auth/login']);
      },
      error: (error) => {
        console.error('Logout error:', error);
        // Force navigation even if logout API fails
        this.router.navigate(['/auth/login']);
      }
    });
  }
}