import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { FormaCompraRequest } from '../../models';

@Component({
  selector: 'app-formas-compra-form',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbAlertModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3><i class="bi bi-credit-card"></i> {{ isEdit() ? 'Editar Forma de Compra' : 'Nova Forma de Compra' }}</h3>
      </div>
      <div class="card-body">
        @if (successMessage()) {
          <ngb-alert [dismissible]="true" (closed)="successMessage.set('')" type="success">
            <i class="bi bi-check-circle"></i> {{ successMessage() }}
          </ngb-alert>
        }
        
        <form (ngSubmit)="onSubmit()">
          <div class="mb-3">
            <label class="form-label">Descrição *</label>
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="descricao" 
              name="descricao"
              maxlength="30"
              required>
            <small class="form-text text-muted">Máximo 30 caracteres</small>
          </div>

          @if (error()) {
            <ngb-alert [dismissible]="false" type="danger">
              <strong>Erro!</strong> {{ error() }}
            </ngb-alert>
          }

          <div class="d-flex gap-2">
            <button type="submit" class="btn btn-primary" [disabled]="loading()">
              <i class="bi bi-check"></i> {{ loading() ? 'Salvando...' : 'Salvar' }}
            </button>
            <button type="button" class="btn btn-secondary" (click)="cancel()">
              <i class="bi bi-x"></i> Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  `
})
export class FormasCompraFormComponent implements OnInit {
  descricao = '';
  loading = signal(false);
  error = signal('');
  successMessage = signal('');
  isEdit = signal(false);
  formaId: number | null = null;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.formaId = +id;
      this.loadFormaCompra();
    }
  }

  loadFormaCompra(): void {
    if (this.formaId) {
      this.loading.set(true);
      this.apiService.getFormaCompra(this.formaId).subscribe({
        next: (forma: any) => {
          this.descricao = forma.descricao;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Erro ao carregar forma de compra');
          this.loading.set(false);
        }
      });
    }
  }

  onSubmit(): void {
    if (!this.descricao.trim()) {
      this.error.set('Descrição é obrigatória');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const request: FormaCompraRequest = { descricao: this.descricao.trim() };

    const operation = this.isEdit() && this.formaId
      ? this.apiService.updateFormaCompra(this.formaId, request)
      : this.apiService.createFormaCompra(request);

    operation.subscribe({
      next: () => {
        this.loading.set(false);
        
        if (this.isEdit()) {
          this.successMessage.set('Forma de compra atualizada com sucesso!');
          setTimeout(() => {
            this.router.navigate(['/formas-compra']);
          }, 2000);
        } else {
          this.router.navigate(['/success'], {
            state: {
              message: 'Forma de compra cadastrada com sucesso!',
              entityName: 'Forma de Compra',
              listRoute: '/formas-compra',
              newRoute: '/formas-compra/novo'
            }
          });
        }
      },
      error: (err: any) => {
        this.error.set(err.error?.message || 'Erro ao salvar forma de compra');
        this.loading.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/formas-compra']);
  }
}

