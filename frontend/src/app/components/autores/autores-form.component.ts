import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { AutorRequest } from '../../models';

@Component({
  selector: 'app-autores-form',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbAlertModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3><i class="bi bi-person"></i> {{ isEdit() ? 'Editar Autor' : 'Novo Autor' }}</h3>
      </div>
      <div class="card-body">
        @if (successMessage()) {
          <ngb-alert [dismissible]="true" (closed)="successMessage.set('')" type="success">
            <i class="bi bi-check-circle"></i> {{ successMessage() }}
          </ngb-alert>
        }
        
        <form (ngSubmit)="onSubmit()">
          <div class="mb-3">
            <label class="form-label">Nome *</label>
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="nome" 
              name="nome"
              maxlength="40"
              required>
            <small class="form-text text-muted">Máximo 40 caracteres</small>
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
export class AutoresFormComponent implements OnInit {
  nome = '';
  loading = signal(false);
  error = signal('');
  successMessage = signal('');
  isEdit = signal(false);
  autorId: number | null = null;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.autorId = +id;
      this.loadAutor();
    }
  }

  loadAutor(): void {
    if (this.autorId) {
      this.loading.set(true);
      this.apiService.getAutor(this.autorId).subscribe({
        next: (autor: any) => {
          this.nome = autor.nome;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Erro ao carregar autor');
          this.loading.set(false);
        }
      });
    }
  }

  onSubmit(): void {
    if (!this.nome.trim()) {
      this.error.set('Nome é obrigatório');
      return;
    }

    this.loading.set(true);
    this.error.set('');

    const request: AutorRequest = { nome: this.nome.trim() };

    const operation = this.isEdit() && this.autorId
      ? this.apiService.updateAutor(this.autorId, request)
      : this.apiService.createAutor(request);

    operation.subscribe({
      next: () => {
        this.loading.set(false);
        
        if (this.isEdit()) {
          this.successMessage.set('Autor atualizado com sucesso!');
          setTimeout(() => {
            this.router.navigate(['/autores']);
          }, 2000);
        } else {
          this.router.navigate(['/success'], {
            state: {
              message: 'Autor cadastrado com sucesso!',
              entityName: 'Autor',
              listRoute: '/autores',
              newRoute: '/autores/novo'
            }
          });
        }
      },
      error: (err: any) => {
        this.error.set(err.error?.message || 'Erro ao salvar autor');
        this.loading.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/autores']);
  }
}

