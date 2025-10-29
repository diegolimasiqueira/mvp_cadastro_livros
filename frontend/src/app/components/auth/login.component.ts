import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container">
      <div class="row justify-content-center mt-5">
        <div class="col-md-4">
          <div class="card">
            <div class="card-body">
              <h3 class="card-title text-center mb-4">BookStore Login</h3>
              @if (error()) {
                <div class="alert alert-danger">{{ error() }}</div>
              }
              <form (ngSubmit)="onSubmit()">
                <div class="mb-3">
                  <label class="form-label">Usuário</label>
                  <input type="text" class="form-control" [(ngModel)]="username" name="username" required>
                </div>
                <div class="mb-3">
                  <label class="form-label">Senha</label>
                  <input type="password" class="form-control" [(ngModel)]="password" name="password" required>
                </div>
                <button type="submit" class="btn btn-primary w-100" [disabled]="loading()">
                  {{ loading() ? 'Entrando...' : 'Entrar' }}
                </button>
              </form>
              <div class="mt-3 text-center small text-muted">
                <p>Credenciais padrão:</p>
                <p>Usuário: <strong>admin</strong> | Senha: <strong>admin123</strong></p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  username = '';
  password = '';
  loading = signal(false);
  error = signal('');

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.loading.set(true);
    this.error.set('');

    this.authService.login({ username: this.username, password: this.password })
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
        },
        error: () => {
          this.error.set('Credenciais inválidas');
          this.loading.set(false);
        }
      });
  }
}

