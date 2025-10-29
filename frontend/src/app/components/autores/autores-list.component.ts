import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { ConfirmationService } from '../../services/confirmation.service';
import { Autor } from '../../models';

@Component({
  selector: 'app-autores-list',
  standalone: true,
  imports: [CommonModule, NgbPaginationModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-person"></i> Autores</h2>
      <button class="btn btn-primary" (click)="novoAutor()">
        <i class="bi bi-plus"></i> Novo Autor
      </button>
    </div>

    @if (loading()) {
      <div class="spinner-border"></div>
    } @else if (autores().length === 0) {
      <div class="alert alert-info">
        <i class="bi bi-info-circle"></i> Nenhum autor cadastrado ainda. Clique em "Novo Autor" para adicionar.
      </div>
    } @else {
      <div class="table-responsive">
        <table class="table table-hover">
          <thead><tr><th>Código</th><th>Nome</th><th>Ações</th></tr></thead>
          <tbody>
            @for (autor of autores(); track autor.codAu) {
              <tr>
                <td>{{ autor.codAu }}</td>
                <td>{{ autor.nome }}</td>
                <td>
                  <button class="btn btn-sm btn-primary me-1" (click)="editAutor(autor.codAu)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-sm btn-danger" (click)="deleteAutor(autor.codAu)">
                    <i class="bi bi-trash"></i>
                  </button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      <!-- Paginação -->
      <div class="d-flex justify-content-between align-items-center mt-3">
        <div class="text-muted">
          Mostrando {{ autores().length }} de {{ totalRecords() }} registros
        </div>
        <ngb-pagination
          [(page)]="currentPage"
          [pageSize]="pageSize"
          [collectionSize]="totalRecords()"
          [maxSize]="5"
          [rotate]="true"
          [boundaryLinks]="true"
          (pageChange)="onPageChange($event)">
        </ngb-pagination>
      </div>
    }
  `
})
export class AutoresListComponent implements OnInit {
  autores = signal<Autor[]>([]);
  loading = signal(false);
  currentPage = 1;
  pageSize = 10;
  totalRecords = signal(0);

  constructor(
    private apiService: ApiService, 
    private router: Router,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadAutores();
  }

  loadAutores(): void {
    this.loading.set(true);
    this.apiService.getAutoresPaged(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.autores.set(response.data);
        this.totalRecords.set(response.totalRecords);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadAutores();
  }

  novoAutor(): void {
    this.router.navigate(['/autores/novo']);
  }

  editAutor(id: number): void {
    this.router.navigate(['/autores/editar', id]);
  }

  deleteAutor(id: number): void {
    this.confirmationService.confirm({
      title: 'Excluir Autor',
      message: 'Esta ação não pode ser desfeita. Deseja realmente excluir este autor?',
      confirmText: 'Sim, Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.apiService.deleteAutor(id).subscribe(() => this.loadAutores());
      }
    });
  }
}

