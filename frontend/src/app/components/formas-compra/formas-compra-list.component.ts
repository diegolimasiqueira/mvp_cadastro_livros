import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { ConfirmationService } from '../../services/confirmation.service';
import { FormaCompra } from '../../models';

@Component({
  selector: 'app-formas-compra-list',
  standalone: true,
  imports: [CommonModule, NgbPaginationModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-credit-card"></i> Formas de Compra</h2>
      <button class="btn btn-primary" (click)="novaForma()">
        <i class="bi bi-plus"></i> Nova Forma de Compra
      </button>
    </div>

    @if (loading()) {
      <div class="spinner-border"></div>
    } @else if (formas().length === 0) {
      <div class="alert alert-info">
        <i class="bi bi-info-circle"></i> Nenhuma forma de compra cadastrada ainda. Clique em "Nova Forma de Compra" para adicionar.
      </div>
    } @else {
      <div class="table-responsive">
        <table class="table table-hover">
          <thead><tr><th>Código</th><th>Descrição</th><th>Ações</th></tr></thead>
          <tbody>
            @for (forma of formas(); track forma.codFc) {
              <tr>
                <td>{{ forma.codFc }}</td>
                <td>{{ forma.descricao }}</td>
                <td>
                  <button class="btn btn-sm btn-primary me-1" (click)="editForma(forma.codFc)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-sm btn-danger" (click)="deleteForma(forma.codFc)">
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
          Mostrando {{ formas().length }} de {{ totalRecords() }} registros
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
export class FormasCompraListComponent implements OnInit {
  formas = signal<FormaCompra[]>([]);
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
    this.loadFormas();
  }

  loadFormas(): void {
    this.loading.set(true);
    this.apiService.getFormasCompraPaged(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.formas.set(response.data);
        this.totalRecords.set(response.totalRecords);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadFormas();
  }

  novaForma(): void {
    this.router.navigate(['/formas-compra/novo']);
  }

  editForma(id: number): void {
    this.router.navigate(['/formas-compra/editar', id]);
  }

  deleteForma(id: number): void {
    this.confirmationService.confirm({
      title: 'Excluir Forma de Compra',
      message: 'Esta ação não pode ser desfeita. Deseja realmente excluir esta forma de compra?',
      confirmText: 'Sim, Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.apiService.deleteFormaCompra(id).subscribe(() => this.loadFormas());
      }
    });
  }
}

