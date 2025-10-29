import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  styles: [`
    :host {
      display: block;
      height: 100vh;
    }

    .navbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1030;
    }

    .sidebar {
      position: fixed;
      top: 56px;
      bottom: 0;
      left: 0;
      z-index: 100;
      padding: 20px 0;
      background-color: #f8f9fa;
      box-shadow: 2px 0 5px rgba(0,0,0,0.1);
      transition: width 0.3s ease;
      overflow-x: hidden;
      overflow-y: auto;
    }

    .sidebar.expanded {
      width: 250px;
    }

    .sidebar.collapsed {
      width: 70px;
    }

    .sidebar .nav-link {
      display: flex;
      align-items: center;
      padding: 12px 20px;
      color: #333;
      text-decoration: none;
      transition: all 0.2s;
      white-space: nowrap;
      cursor: pointer;
    }

    .sidebar .nav-link:hover {
      background-color: #e9ecef;
    }

    .sidebar .nav-link.active {
      background-color: #0d6efd;
      color: white;
    }

    .sidebar .nav-link i {
      font-size: 1.3rem;
      min-width: 30px;
      text-align: center;
    }

    .sidebar .nav-link span {
      margin-left: 15px;
      opacity: 1;
      transition: opacity 0.2s;
    }

    .sidebar.collapsed .nav-link span {
      opacity: 0;
      width: 0;
      overflow: hidden;
    }

    .sidebar.collapsed .nav-link {
      justify-content: center;
      padding: 12px 0;
    }

    .main-content {
      padding-top: 76px;
      transition: margin-left 0.3s ease;
      min-height: 100vh;
    }

    .main-content.expanded {
      margin-left: 250px;
    }

    .main-content.collapsed {
      margin-left: 70px;
    }

    .toggle-btn {
      background: none;
      border: none;
      color: white;
      font-size: 1.5rem;
      cursor: pointer;
      padding: 5px 10px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .toggle-btn:hover {
      background-color: rgba(255,255,255,0.1);
      border-radius: 4px;
    }

    @media (max-width: 768px) {
      .sidebar.collapsed {
        width: 0;
      }

      .main-content.collapsed {
        margin-left: 0;
      }
    }
  `],
  template: `
    <nav class="navbar navbar-dark bg-primary">
      <div class="container-fluid">
        <div class="d-flex align-items-center">
          <button class="toggle-btn" (click)="toggleSidebar()" type="button">
            <i class="bi bi-list"></i>
          </button>
          <span class="navbar-brand mb-0 h1 ms-2">
            <i class="bi bi-book"></i> BookStore
          </span>
        </div>
        <button class="btn btn-outline-light btn-sm" (click)="logout()">
          <i class="bi bi-box-arrow-right"></i> Sair
        </button>
      </div>
    </nav>

    <div class="sidebar" [class.expanded]="isExpanded" [class.collapsed]="!isExpanded">
      <nav class="nav flex-column">
        <a routerLink="/livros" routerLinkActive="active" class="nav-link" 
           [title]="!isExpanded ? 'Livros' : ''">
          <i class="bi bi-book"></i>
          <span>Livros</span>
        </a>
        <a routerLink="/autores" routerLinkActive="active" class="nav-link"
           [title]="!isExpanded ? 'Autores' : ''">
          <i class="bi bi-person"></i>
          <span>Autores</span>
        </a>
        <a routerLink="/assuntos" routerLinkActive="active" class="nav-link"
           [title]="!isExpanded ? 'Assuntos' : ''">
          <i class="bi bi-tag"></i>
          <span>Assuntos</span>
        </a>
        <a routerLink="/formas-compra" routerLinkActive="active" class="nav-link"
           [title]="!isExpanded ? 'Formas de Compra' : ''">
          <i class="bi bi-credit-card"></i>
          <span>Formas de Compra</span>
        </a>
        <a routerLink="/relatorios" routerLinkActive="active" class="nav-link"
           [title]="!isExpanded ? 'Relatórios' : ''">
          <i class="bi bi-file-earmark-text"></i>
          <span>Relatórios</span>
        </a>
        <a routerLink="/health" routerLinkActive="active" class="nav-link"
           [title]="!isExpanded ? 'Status do Sistema' : ''">
          <i class="bi bi-heart-pulse"></i>
          <span>Status</span>
        </a>
      </nav>
    </div>

    <main class="main-content px-4" [class.expanded]="isExpanded" [class.collapsed]="!isExpanded">
      <router-outlet></router-outlet>
    </main>
  `
})
export class LayoutComponent {
  isExpanded = true;

  constructor(private authService: AuthService) {
    // Carregar estado do localStorage
    const savedState = localStorage.getItem('sidebarExpanded');
    if (savedState !== null) {
      this.isExpanded = savedState === 'true';
    }

    // Em mobile, inicia colapsado
    if (typeof window !== 'undefined' && window.innerWidth < 768) {
      this.isExpanded = false;
    }
  }

  toggleSidebar(): void {
    this.isExpanded = !this.isExpanded;
    console.log('Sidebar toggled:', this.isExpanded);
    // Salvar estado no localStorage
    localStorage.setItem('sidebarExpanded', this.isExpanded.toString());
  }

  logout(): void {
    this.authService.logout();
  }
}



