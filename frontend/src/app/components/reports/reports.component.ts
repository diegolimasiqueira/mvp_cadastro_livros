import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { BooksByAuthorReport } from '../../models';

interface AutorGroup {
  autorNome: string;
  livros: BooksByAuthorReport[];
}

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule],
  styles: [`
    .filtros-box {
      background: #f8f9fa;
      border: 1px solid #dee2e6;
      border-radius: 8px;
      padding: 20px;
      margin-bottom: 20px;
    }

    .filtros-box h5 {
      margin-bottom: 15px;
      color: #495057;
    }

    .autor-header {
      background: linear-gradient(135deg, #0066cc 0%, #0052a3 100%);
      color: white;
      padding: 15px 20px;
      border-radius: 8px 8px 0 0;
      margin-top: 20px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .autor-header h4 {
      margin: 0;
      font-size: 1.1rem;
      font-weight: 600;
    }

    .livros-table {
      margin-bottom: 30px;
      border-radius: 0 0 8px 8px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .livros-table table {
      margin-bottom: 0;
    }

    .livros-table thead {
      background-color: #e0e0e0;
    }

    .livros-table thead th {
      font-size: 0.9rem;
      font-weight: 600;
      padding: 12px;
      border-bottom: 2px solid #ccc;
    }

    .livros-table tbody td {
      padding: 10px 12px;
      vertical-align: middle;
    }

    .export-buttons {
      gap: 10px;
    }

    .btn-pdf {
      background-color: #dc3545;
      border-color: #dc3545;
    }

    .btn-pdf:hover {
      background-color: #c82333;
      border-color: #bd2130;
    }

    .btn-excel {
      background-color: #28a745;
      border-color: #28a745;
    }

    .btn-excel:hover {
      background-color: #218838;
      border-color: #1e7e34;
    }

    .small-text {
      font-size: 0.85rem;
    }

    .badge-filter {
      margin-left: 10px;
    }

    .livros-pagination {
      background: linear-gradient(to bottom, #ffffff 0%, #f8f9fa 100%);
      border-top: 2px solid #0066cc;
      padding: 12px;
      margin-top: 10px;
    }

    ::ng-deep .livros-pagination .pagination {
      margin: 0;
    }

    ::ng-deep .livros-pagination .page-item .page-link {
      font-size: 0.85rem;
      padding: 0.25rem 0.5rem;
    }

    .autores-pagination-card {
      background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
      border: 2px solid #28a745;
    }

    .autores-pagination-card .card-body {
      padding: 15px 20px;
    }
  `],
  template: `
    <div class="mb-4 d-flex justify-content-between align-items-center">
      <h2><i class="bi bi-file-earmark-text"></i> Relatório de Livros por Autor</h2>
      <div class="export-buttons d-flex">
        <button 
          class="btn btn-pdf text-white" 
          (click)="downloadPdf()" 
          [disabled]="loadingPdf() || report().length === 0">
          <i class="bi bi-file-earmark-pdf"></i> 
          @if (loadingPdf()) {
            <span class="spinner-border spinner-border-sm me-1"></span>
            Gerando...
          } @else {
            Exportar PDF
          }
        </button>
        <button 
          class="btn btn-excel text-white" 
          (click)="downloadExcel()" 
          [disabled]="loadingExcel() || report().length === 0">
          <i class="bi bi-file-earmark-excel"></i> 
          @if (loadingExcel()) {
            <span class="spinner-border spinner-border-sm me-1"></span>
            Gerando...
          } @else {
            Exportar Excel
          }
        </button>
      </div>
    </div>

    <!-- Box de Filtros -->
    <div class="filtros-box">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h5 class="mb-0">
          <i class="bi bi-funnel"></i> Filtros
          @if (hasActiveFilters()) {
            <span class="badge bg-primary badge-filter">{{ countActiveFilters() }}</span>
          }
        </h5>
        <button 
          class="btn btn-sm btn-outline-secondary" 
          (click)="limparFiltros()"
          [disabled]="!hasActiveFilters()">
          <i class="bi bi-x-circle"></i> Limpar Filtros
        </button>
      </div>
      <div class="row g-3">
        <div class="col-md-3">
          <label class="form-label">Autor</label>
          <input 
            type="text" 
            class="form-control form-control-sm" 
            [ngModel]="filtroAutor()"
            (ngModelChange)="filtroAutor.set($event); currentPage.set(1)"
            placeholder="Buscar por autor...">
        </div>
        <div class="col-md-3">
          <label class="form-label">Livro</label>
          <input 
            type="text" 
            class="form-control form-control-sm" 
            [ngModel]="filtroLivro()"
            (ngModelChange)="filtroLivro.set($event); currentPage.set(1)"
            placeholder="Buscar por livro...">
        </div>
        <div class="col-md-2">
          <label class="form-label">Editora</label>
          <input 
            type="text" 
            class="form-control form-control-sm" 
            [ngModel]="filtroEditora()"
            (ngModelChange)="filtroEditora.set($event); currentPage.set(1)"
            placeholder="Editora...">
        </div>
        <div class="col-md-2">
          <label class="form-label">Ano</label>
          <input 
            type="text" 
            class="form-control form-control-sm" 
            [ngModel]="filtroAno()"
            (ngModelChange)="filtroAno.set($event); currentPage.set(1)"
            placeholder="Ano...">
        </div>
        <div class="col-md-2">
          <label class="form-label">Assunto</label>
          <input 
            type="text" 
            class="form-control form-control-sm" 
            [ngModel]="filtroAssunto()"
            (ngModelChange)="filtroAssunto.set($event); currentPage.set(1)"
            placeholder="Assunto...">
        </div>
      </div>
    </div>

    @if (loading()) {
      <div class="d-flex justify-content-center py-5">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Carregando...</span>
        </div>
      </div>
    } @else if (filteredGroupedReport().length === 0) {
      <div class="alert alert-info">
        <i class="bi bi-info-circle"></i> 
        @if (hasActiveFilters()) {
          Nenhum registro encontrado com os filtros aplicados.
        } @else {
          Nenhum dado disponível para o relatório. Cadastre livros com autores para visualizar o relatório.
        }
      </div>
    } @else {
      @for (group of paginatedGroups(); track group.autorNome) {
        <div class="mb-4">
          <!-- Cabeçalho do Autor -->
          <div class="autor-header">
            <h4>
              <i class="bi bi-person-circle"></i> Autor: {{ group.autorNome }}
              <span class="badge bg-light text-dark ms-2">
                {{ group.livros.length }} {{ group.livros.length === 1 ? 'livro' : 'livros' }}
                @if (group.livros.length > livrosPerPage) {
                  - Exibindo {{ getPaginatedLivros(group.livros, group.autorNome).length }} de {{ group.livros.length }}
                }
              </span>
            </h4>
          </div>

          <!-- Tabela de Livros -->
          <div class="livros-table">
            <div class="table-responsive">
              <table class="table table-hover mb-0">
                <thead>
                  <tr>
                    <th>Livro</th>
                    <th>Editora</th>
                    <th class="text-center">Edição</th>
                    <th class="text-center">Ano</th>
                    <th>Assuntos</th>
                    <th>Preços</th>
                  </tr>
                </thead>
                <tbody>
                  @for (livro of getPaginatedLivros(group.livros, group.autorNome); track livro.livroId) {
                    <tr>
                      <td>{{ livro.livroTitulo }}</td>
                      <td>{{ livro.editora }}</td>
                      <td class="text-center">{{ livro.edicao }}</td>
                      <td class="text-center">{{ livro.anoPublicacao }}</td>
                      <td class="small-text">{{ livro.assuntos || '-' }}</td>
                      <td class="small-text">{{ livro.formasCompra || '-' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
            
            <!-- Paginação de livros do autor -->
            @if (group.livros.length > livrosPerPage) {
              <div class="livros-pagination d-flex justify-content-between align-items-center px-3 pb-3">
                <small class="text-muted">
                  <i class="bi bi-book"></i> Livros do autor - Página {{ getAutorPage(group.autorNome) }} de {{ getTotalLivrosPages(group.livros.length) }}
                </small>
                <ngb-pagination
                  [page]="getAutorPage(group.autorNome)"
                  (pageChange)="setAutorPage(group.autorNome, $event)"
                  [pageSize]="livrosPerPage"
                  [collectionSize]="group.livros.length"
                  [maxSize]="3"
                  [rotate]="true"
                  [boundaryLinks]="true"
                  size="sm">
                </ngb-pagination>
              </div>
            }
          </div>
        </div>
      }

      <!-- Paginação Principal (Autores) -->
      <div class="card mt-4 shadow-sm autores-pagination-card">
        <div class="card-body">
          <div class="d-flex justify-content-between align-items-center">
            <div>
              <h6 class="mb-1 text-primary">
                <i class="bi bi-people-fill"></i> Navegação de Autores
              </h6>
              <small class="text-muted">
                @if (hasActiveFilters()) {
                  Exibindo <strong>{{ totalAutoresFiltrados() }}</strong> {{ totalAutoresFiltrados() === 1 ? 'autor' : 'autores' }} | 
                  <strong>{{ totalLivrosFiltrados() }}</strong> {{ totalLivrosFiltrados() === 1 ? 'livro' : 'livros' }}
                  de um total de {{ totalAutores() }} {{ totalAutores() === 1 ? 'autor' : 'autores' }}
                } @else {
                  Total: <strong>{{ totalAutores() }}</strong> {{ totalAutores() === 1 ? 'autor' : 'autores' }} | 
                  <strong>{{ totalLivros() }}</strong> {{ totalLivros() === 1 ? 'livro' : 'livros' }}
                }
              </small>
            </div>
            
            @if (filteredGroupedReport().length > itemsPerPage) {
              <div>
                <ngb-pagination
                  [page]="currentPage()"
                  (pageChange)="currentPage.set($event)"
                  [pageSize]="itemsPerPage"
                  [collectionSize]="filteredGroupedReport().length"
                  [maxSize]="5"
                  [rotate]="true"
                  [boundaryLinks]="true">
                </ngb-pagination>
              </div>
            }
          </div>
        </div>
      </div>
    }
  `
})
export class ReportsComponent implements OnInit {
  report = signal<BooksByAuthorReport[]>([]);
  loading = signal(false);
  loadingPdf = signal(false);
  loadingExcel = signal(false);

  // Filtros como signals para reatividade automática
  filtroAutor = signal('');
  filtroLivro = signal('');
  filtroEditora = signal('');
  filtroAno = signal('');
  filtroAssunto = signal('');

  // Paginação principal (autores)
  currentPage = signal(1);
  itemsPerPage = 5; // 5 autores por página
  
  // Paginação interna (livros por autor)
  livrosPerPage = 10; // 10 livros por autor
  autorPageMap = signal<Record<string, number>>({}); // Map de páginas por autor

  // Computed para agrupar os dados por autor
  groupedReport = computed(() => {
    const data = this.report();
    const grouped = new Map<string, BooksByAuthorReport[]>();
    
    data.forEach(item => {
      if (!grouped.has(item.autorNome)) {
        grouped.set(item.autorNome, []);
      }
      grouped.get(item.autorNome)!.push(item);
    });

    const sortedGroups: AutorGroup[] = Array.from(grouped.entries())
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([autorNome, livros]) => ({ autorNome, livros }));

    return sortedGroups;
  });

  // Computed para aplicar filtros (reage automaticamente aos signals)
  filteredGroupedReport = computed(() => {
    let groups = this.groupedReport();

    // Filtro por autor
    const autorFiltro = this.filtroAutor().trim();
    if (autorFiltro) {
      const autorLower = autorFiltro.toLowerCase();
      groups = groups.filter(g => g.autorNome.toLowerCase().includes(autorLower));
    }

    // Filtros nos livros
    const livroFiltro = this.filtroLivro().trim();
    const editoraFiltro = this.filtroEditora().trim();
    const anoFiltro = this.filtroAno().trim();
    const assuntoFiltro = this.filtroAssunto().trim();

    if (livroFiltro || editoraFiltro || anoFiltro || assuntoFiltro) {
      groups = groups.map(group => {
        let livrosFiltrados = group.livros;

        if (livroFiltro) {
          const livroLower = livroFiltro.toLowerCase();
          livrosFiltrados = livrosFiltrados.filter(l => l.livroTitulo.toLowerCase().includes(livroLower));
        }

        if (editoraFiltro) {
          const editoraLower = editoraFiltro.toLowerCase();
          livrosFiltrados = livrosFiltrados.filter(l => l.editora.toLowerCase().includes(editoraLower));
        }

        if (anoFiltro) {
          livrosFiltrados = livrosFiltrados.filter(l => l.anoPublicacao.includes(anoFiltro));
        }

        if (assuntoFiltro) {
          const assuntoLower = assuntoFiltro.toLowerCase();
          livrosFiltrados = livrosFiltrados.filter(l => 
            l.assuntos?.toLowerCase().includes(assuntoLower)
          );
        }

        return { ...group, livros: livrosFiltrados };
      }).filter(g => g.livros.length > 0);
    }

    return groups;
  });

  // Computed para paginação
  paginatedGroups = computed(() => {
    const groups = this.filteredGroupedReport();
    const page = this.currentPage();
    const start = (page - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return groups.slice(start, end);
  });

  totalAutores = computed(() => this.groupedReport().length);
  totalLivros = computed(() => this.report().length);
  totalAutoresFiltrados = computed(() => this.filteredGroupedReport().length);
  totalLivrosFiltrados = computed(() => {
    return this.filteredGroupedReport().reduce((sum, group) => sum + group.livros.length, 0);
  });

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadReport();
  }

  loadReport(): void {
    this.loading.set(true);
    this.apiService.getBooksByAuthorReport().subscribe({
      next: (data) => { 
        this.report.set(data); 
        this.loading.set(false); 
      },
      error: () => this.loading.set(false)
    });
  }

  limparFiltros(): void {
    this.filtroAutor.set('');
    this.filtroLivro.set('');
    this.filtroEditora.set('');
    this.filtroAno.set('');
    this.filtroAssunto.set('');
    this.currentPage.set(1);
    this.autorPageMap.set({}); // Reset páginas dos autores
  }

  hasActiveFilters(): boolean {
    return !!(this.filtroAutor() || this.filtroLivro() || this.filtroEditora() || this.filtroAno() || this.filtroAssunto());
  }

  countActiveFilters(): number {
    let count = 0;
    if (this.filtroAutor()) count++;
    if (this.filtroLivro()) count++;
    if (this.filtroEditora()) count++;
    if (this.filtroAno()) count++;
    if (this.filtroAssunto()) count++;
    return count;
  }

  // Métodos para paginação de livros por autor
  getAutorPage(autorNome: string): number {
    return this.autorPageMap()[autorNome] || 1;
  }

  setAutorPage(autorNome: string, page: number): void {
    const current = this.autorPageMap();
    this.autorPageMap.set({ ...current, [autorNome]: page });
  }

  getPaginatedLivros(livros: BooksByAuthorReport[], autorNome: string): BooksByAuthorReport[] {
    const page = this.getAutorPage(autorNome);
    const start = (page - 1) * this.livrosPerPage;
    const end = start + this.livrosPerPage;
    return livros.slice(start, end);
  }

  getTotalLivrosPages(totalLivros: number): number {
    return Math.ceil(totalLivros / this.livrosPerPage);
  }

  downloadPdf(): void {
    this.loadingPdf.set(true);
    // PDF usa TODOS os dados (sem filtros/paginação)
    this.apiService.downloadBooksByAuthorReportPdf().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        const fileName = `Relatorio_Livros_Por_Autor_${new Date().toISOString().slice(0, 10)}.pdf`;
        link.download = fileName;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingPdf.set(false);
      },
      error: (err) => {
        console.error('Erro ao baixar PDF:', err);
        alert('Erro ao gerar o relatório PDF. Por favor, tente novamente.');
        this.loadingPdf.set(false);
      }
    });
  }

  downloadExcel(): void {
    this.loadingExcel.set(true);
    // Excel usa TODOS os dados (sem filtros/paginação)
    this.apiService.downloadBooksByAuthorReportExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        const fileName = `Relatorio_Livros_Por_Autor_${new Date().toISOString().slice(0, 10)}.xlsx`;
        link.download = fileName;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingExcel.set(false);
      },
      error: (err) => {
        console.error('Erro ao baixar Excel:', err);
        alert('Erro ao gerar o relatório Excel. Por favor, tente novamente.');
        this.loadingExcel.set(false);
      }
    });
  }
}
