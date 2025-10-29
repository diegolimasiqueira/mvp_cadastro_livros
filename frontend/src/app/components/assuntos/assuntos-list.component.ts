import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { ConfirmationService } from '../../services/confirmation.service';
import { Assunto } from '../../models';

@Component({
  selector: 'app-assuntos-list',
  standalone: true,
  imports: [CommonModule, NgbPaginationModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-tag"></i> Assuntos</h2>
      <button class="btn btn-primary" (click)="novoAssunto()">
        <i class="bi bi-plus"></i> Novo Assunto
      </button>
    </div>

    @if (loading()) {
      <div class="spinner-border"></div>
    } @else if (assuntos().length === 0) {
      <div class="alert alert-info">
        <i class="bi bi-info-circle"></i> Nenhum assunto cadastrado ainda. Clique em "Novo Assunto" para adicionar.
      </div>
    } @else {
      <div class="table-responsive">
        <table class="table table-hover">
          <thead><tr><th>Código</th><th>Descrição</th><th>Ações</th></tr></thead>
          <tbody>
            @for (assunto of assuntos(); track assunto.codAs) {
              <tr>
                <td>{{ assunto.codAs }}</td>
                <td>{{ assunto.descricao }}</td>
                <td>
                  <button class="btn btn-sm btn-primary me-1" (click)="editAssunto(assunto.codAs)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-sm btn-danger" (click)="deleteAssunto(assunto.codAs)">
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
          Mostrando {{ assuntos().length }} de {{ totalRecords() }} registros
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
export class AssuntosListComponent implements OnInit {
  assuntos = signal<Assunto[]>([]);
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
    this.loadAssuntos();
  }

  loadAssuntos(): void {
    this.loading.set(true);
    this.apiService.getAssuntosPaged(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.assuntos.set(response.data);
        this.totalRecords.set(response.totalRecords);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadAssuntos();
  }

  novoAssunto(): void {
    this.router.navigate(['/assuntos/novo']);
  }

  editAssunto(id: number): void {
    this.router.navigate(['/assuntos/editar', id]);
  }

  deleteAssunto(id: number): void {
    this.confirmationService.confirm({
      title: 'Excluir Assunto',
      message: 'Esta ação não pode ser desfeita. Deseja realmente excluir este assunto?',
      confirmText: 'Sim, Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.apiService.deleteAssunto(id).subscribe(() => this.loadAssuntos());
      }
    });
  }
}

