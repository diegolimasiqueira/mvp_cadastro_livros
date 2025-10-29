import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { ConfirmationService } from '../../services/confirmation.service';
import { Livro } from '../../models';

@Component({
  selector: 'app-livros-list',
  standalone: true,
  imports: [CommonModule, NgbPaginationModule],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-book"></i> Livros</h2>
      <button class="btn btn-primary" (click)="novoLivro()">
        <i class="bi bi-plus"></i> Novo Livro
      </button>
    </div>

    @if (loading()) {
      <div class="text-center">
        <div class="spinner-border" role="status"></div>
      </div>
    } @else if (livros().length === 0) {
      <div class="alert alert-info">Nenhum livro cadastrado</div>
    } @else {
      <div class="table-responsive">
        <table class="table table-hover">
          <thead>
            <tr>
              <th>Código</th>
              <th>Título</th>
              <th>Editora</th>
              <th>Edição</th>
              <th>Ano</th>
              <th>Autores</th>
              <th>Assuntos</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            @for (livro of livros(); track livro.codI) {
              <tr>
                <td>{{ livro.codI }}</td>
                <td>{{ livro.titulo }}</td>
                <td>{{ livro.editora }}</td>
                <td>{{ livro.edicao }}</td>
                <td>{{ livro.anoPublicacao }}</td>
                <td>{{ getAutoresNomes(livro) }}</td>
                <td>{{ getAssuntosDescricoes(livro) }}</td>
                <td>
                  <button class="btn btn-sm btn-primary me-1" (click)="editLivro(livro.codI)">
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button class="btn btn-sm btn-danger" (click)="deleteLivro(livro.codI)">
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
          Mostrando {{ livros().length }} de {{ totalRecords() }} registros
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
export class LivrosListComponent implements OnInit {
  livros = signal<Livro[]>([]);
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
    this.loadLivros();
  }

  loadLivros(): void {
    this.loading.set(true);
    this.apiService.getLivrosPaged(this.currentPage, this.pageSize).subscribe({
      next: (response) => {
        this.livros.set(response.data);
        this.totalRecords.set(response.totalRecords);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.loadLivros();
  }

  getAutoresNomes(livro: Livro): string {
    return livro.autores.map(a => a.nome).join(', ');
  }

  getAssuntosDescricoes(livro: Livro): string {
    return livro.assuntos.map(a => a.descricao).join(', ');
  }

  novoLivro(): void {
    this.router.navigate(['/livros/novo']);
  }

  editLivro(id: number): void {
    this.router.navigate(['/livros/editar', id]);
  }

  deleteLivro(id: number): void {
    this.confirmationService.confirm({
      title: 'Excluir Livro',
      message: 'Esta ação não pode ser desfeita. Deseja realmente excluir este livro?',
      confirmText: 'Sim, Excluir',
      cancelText: 'Cancelar',
      type: 'danger'
    }).subscribe(confirmed => {
      if (confirmed) {
        this.apiService.deleteLivro(id).subscribe(() => this.loadLivros());
      }
    });
  }
}

