import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { ApiService } from '../../services/api.service';
import { LivroRequest, Autor, Assunto, FormaCompra } from '../../models';

interface PrecoForm {
  formaCompraId: number;
  valor: number;
}

@Component({
  selector: 'app-livros-form',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbAlertModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3><i class="bi bi-book"></i> {{ isEdit() ? 'Editar Livro' : 'Novo Livro' }}</h3>
      </div>
      <div class="card-body">
        @if (successMessage()) {
          <ngb-alert [dismissible]="true" (closed)="successMessage.set('')" type="success">
            <i class="bi bi-check-circle"></i> {{ successMessage() }}
          </ngb-alert>
        }
        
        <form (ngSubmit)="onSubmit()">
          <!-- Informações Básicas -->
          <div class="row mb-3">
            <div class="col-md-8">
              <label class="form-label">Título *</label>
              <input 
                type="text" 
                class="form-control" 
                [(ngModel)]="titulo" 
                name="titulo" 
                maxlength="40" 
                required
                [class.is-invalid]="tituloInvalido()">
              <div class="invalid-feedback">Título é obrigatório</div>
            </div>
            <div class="col-md-4">
              <label class="form-label">Ano de Publicação *</label>
              <input 
                type="number" 
                class="form-control" 
                [(ngModel)]="anoPublicacao" 
                name="anoPublicacao" 
                min="1900"
                [max]="anoAtual"
                placeholder="Ex: 2024"
                required
                [class.is-invalid]="anoInvalido()">
              <div class="invalid-feedback">
                Digite um ano entre 1900 e {{ anoAtual }}
              </div>
            </div>
          </div>

          <div class="row mb-3">
            <div class="col-md-8">
              <label class="form-label">Editora *</label>
              <input 
                type="text" 
                class="form-control" 
                [(ngModel)]="editora" 
                name="editora" 
                maxlength="40" 
                required
                [class.is-invalid]="editoraInvalida()">
              <div class="invalid-feedback">Editora é obrigatória</div>
            </div>
            <div class="col-md-4">
              <label class="form-label">Edição *</label>
              <input 
                type="number" 
                class="form-control" 
                [(ngModel)]="edicao" 
                name="edicao" 
                min="1" 
                step="1"
                placeholder="Ex: 1"
                required
                [class.is-invalid]="edicaoInvalida()">
              <div class="invalid-feedback">Edição deve ser um número inteiro positivo</div>
            </div>
          </div>

          <!-- Autores -->
          <div class="mb-3">
            <label class="form-label">Autores *</label>
            <div class="input-group">
              <select class="form-select" #autorSelect (change)="onAutorChange(autorSelect.value)">
                <option value="">Selecione um autor</option>
                @for (autor of autoresDisponiveis(); track autor.codAu) {
                  <option [value]="autor.codAu">{{ autor.nome }}</option>
                }
              </select>
              <button type="button" class="btn btn-primary" (click)="addAutorFromSelect()">
                <i class="bi bi-plus-circle"></i> Adicionar
              </button>
            </div>
            <div class="mt-2">
              @for (autorId of autoresSelecionados; track autorId) {
                <span class="badge bg-secondary me-2">
                  {{ getAutorNome(autorId) }}
                  <i class="bi bi-x" style="cursor: pointer;" (click)="removeAutor(autorId)"></i>
                </span>
              }
              @if (autoresSelecionados.length === 0) {
                <small class="text-danger">⚠ Nenhum autor selecionado - obrigatório</small>
              }
            </div>
          </div>

          <!-- Assuntos -->
          <div class="mb-3">
            <label class="form-label">Assuntos *</label>
            <div class="input-group">
              <select class="form-select" #assuntoSelect (change)="onAssuntoChange(assuntoSelect.value)">
                <option value="">Selecione um assunto</option>
                @for (assunto of assuntosDisponiveis(); track assunto.codAs) {
                  <option [value]="assunto.codAs">{{ assunto.descricao }}</option>
                }
              </select>
              <button type="button" class="btn btn-primary" (click)="addAssuntoFromSelect()">
                <i class="bi bi-plus-circle"></i> Adicionar
              </button>
            </div>
            <div class="mt-2">
              @for (assuntoId of assuntosSelecionados; track assuntoId) {
                <span class="badge bg-info me-2">
                  {{ getAssuntoDescricao(assuntoId) }}
                  <i class="bi bi-x" style="cursor: pointer;" (click)="removeAssunto(assuntoId)"></i>
                </span>
              }
              @if (assuntosSelecionados.length === 0) {
                <small class="text-danger">⚠ Nenhum assunto selecionado - obrigatório</small>
              }
            </div>
          </div>

          <!-- Preços -->
          <div class="mb-3">
            <label class="form-label">Preços por Forma de Compra *</label>
            <div class="row g-2 mb-2">
              <div class="col-md-5">
                <select class="form-select" [(ngModel)]="selectedFormaCompra" name="selectedFormaCompra">
                  <option [ngValue]="null">Selecione uma forma de compra</option>
                  @for (forma of formasDisponiveis(); track forma.codFc) {
                    <option [ngValue]="forma.codFc">{{ forma.descricao }}</option>
                  }
                </select>
              </div>
              <div class="col-md-4">
                <div class="input-group">
                  <span class="input-group-text">R$</span>
                  <input 
                    type="number" 
                    class="form-control" 
                    [(ngModel)]="valorPreco" 
                    name="valorPreco" 
                    placeholder="0,00" 
                    step="0.01" 
                    min="0.01"
                    [class.is-invalid]="valorPrecoInvalido()">
                </div>
                <small class="text-muted">Use ponto para decimais (ex: 50.90)</small>
              </div>
              <div class="col-md-3">
                <button 
                  type="button" 
                  class="btn btn-primary w-100" 
                  (click)="addPreco()"
                  [disabled]="!selectedFormaCompra || valorPreco <= 0">
                  <i class="bi bi-plus-circle"></i> Adicionar
                </button>
              </div>
            </div>
            <div class="list-group">
              @for (preco of precos; track $index) {
                <div class="list-group-item d-flex justify-content-between align-items-center">
                  <span>{{ getFormaCompraNome(preco.formaCompraId) }}: R$ {{ preco.valor.toFixed(2) }}</span>
                  <button type="button" class="btn btn-sm btn-outline-danger" (click)="removePreco($index)">
                    <i class="bi bi-trash"></i>
                  </button>
                </div>
              }
              @if (precos.length === 0) {
                <div class="text-center py-3">
                  <small class="text-danger">⚠ Nenhum preço adicionado - obrigatório</small>
                </div>
              }
            </div>
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
export class LivrosFormComponent implements OnInit {
  titulo = '';
  editora = '';
  edicao = 1;
  anoPublicacao: number = new Date().getFullYear();
  anoAtual = new Date().getFullYear();
  
  autoresDisponiveis = signal<Autor[]>([]);
  assuntosDisponiveis = signal<Assunto[]>([]);
  formasDisponiveis = signal<FormaCompra[]>([]);
  
  autoresSelecionados: number[] = [];
  assuntosSelecionados: number[] = [];
  precos: PrecoForm[] = [];
  
  selectedAutorId: string = '';
  selectedAssuntoId: string = '';
  selectedFormaCompra: number | null = null;
  valorPreco = 0;
  
  loading = signal(false);
  error = signal('');
  successMessage = signal('');
  isEdit = signal(false);
  livroId: number | null = null;
  formTouched = false;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadDependencias();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.livroId = +id;
      this.loadLivro();
    }
  }

  loadDependencias(): void {
    this.apiService.getAutores().subscribe((data: any) => this.autoresDisponiveis.set(data));
    this.apiService.getAssuntos().subscribe((data: any) => this.assuntosDisponiveis.set(data));
    this.apiService.getFormasCompra().subscribe((data: any) => this.formasDisponiveis.set(data));
  }

  loadLivro(): void {
    if (this.livroId) {
      this.loading.set(true);
      this.apiService.getLivro(this.livroId).subscribe({
        next: (livro: any) => {
          this.titulo = livro.titulo;
          this.editora = livro.editora;
          this.edicao = livro.edicao;
          this.anoPublicacao = parseInt(livro.anoPublicacao, 10);
          this.autoresSelecionados = livro.autores.map((a: any) => a.codAu);
          this.assuntosSelecionados = livro.assuntos.map((a: any) => a.codAs);
          this.precos = livro.precos.map((p: any) => ({ formaCompraId: p.formaCompraId, valor: p.valor }));
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Erro ao carregar livro');
          this.loading.set(false);
        }
      });
    }
  }

  onAutorChange(value: string): void {
    this.selectedAutorId = value;
    console.log('Autor selecionado:', value);
  }

  addAutorFromSelect(): void {
    console.log('addAutorFromSelect chamado. selectedAutorId:', this.selectedAutorId);
    
    if (this.selectedAutorId && this.selectedAutorId !== '') {
      const autorId = Number(this.selectedAutorId);
      console.log('autorId convertido:', autorId);
      
      if (!isNaN(autorId) && !this.autoresSelecionados.includes(autorId)) {
        this.autoresSelecionados.push(autorId);
        console.log('Autor adicionado! Lista atual:', this.autoresSelecionados);
        this.selectedAutorId = '';
      } else if (this.autoresSelecionados.includes(autorId)) {
        console.log('Autor já está na lista');
      }
    } else {
      console.log('Nenhum autor selecionado no dropdown');
    }
  }

  removeAutor(id: number): void {
    this.autoresSelecionados = this.autoresSelecionados.filter(a => a !== id);
    console.log('Autor removido. Lista atual:', this.autoresSelecionados);
  }

  onAssuntoChange(value: string): void {
    this.selectedAssuntoId = value;
    console.log('Assunto selecionado:', value);
  }

  addAssuntoFromSelect(): void {
    console.log('addAssuntoFromSelect chamado. selectedAssuntoId:', this.selectedAssuntoId);
    
    if (this.selectedAssuntoId && this.selectedAssuntoId !== '') {
      const assuntoId = Number(this.selectedAssuntoId);
      console.log('assuntoId convertido:', assuntoId);
      
      if (!isNaN(assuntoId) && !this.assuntosSelecionados.includes(assuntoId)) {
        this.assuntosSelecionados.push(assuntoId);
        console.log('Assunto adicionado! Lista atual:', this.assuntosSelecionados);
        this.selectedAssuntoId = '';
      } else if (this.assuntosSelecionados.includes(assuntoId)) {
        console.log('Assunto já está na lista');
      }
    } else {
      console.log('Nenhum assunto selecionado no dropdown');
    }
  }

  removeAssunto(id: number): void {
    this.assuntosSelecionados = this.assuntosSelecionados.filter(a => a !== id);
    console.log('Assunto removido. Lista atual:', this.assuntosSelecionados);
  }

  addPreco(): void {
    if (this.selectedFormaCompra !== null && this.selectedFormaCompra !== undefined && this.valorPreco > 0) {
      const formaCompraId = Number(this.selectedFormaCompra);
      const exists = this.precos.find(p => p.formaCompraId === formaCompraId);
      if (!exists) {
        this.precos.push({ formaCompraId: formaCompraId, valor: this.valorPreco });
        this.selectedFormaCompra = null;
        this.valorPreco = 0;
      }
    }
  }

  removePreco(index: number): void {
    this.precos.splice(index, 1);
  }

  getAutorNome(id: number): string {
    return this.autoresDisponiveis().find(a => a.codAu === id)?.nome || '';
  }

  getAssuntoDescricao(id: number): string {
    return this.assuntosDisponiveis().find(a => a.codAs === id)?.descricao || '';
  }

  getFormaCompraNome(id: number): string {
    return this.formasDisponiveis().find(f => f.codFc === id)?.descricao || '';
  }

  onSubmit(): void {
    this.formTouched = true;

    // Validar título
    if (!this.titulo.trim()) {
      this.error.set('Título é obrigatório');
      return;
    }

    // Validar editora
    if (!this.editora.trim()) {
      this.error.set('Editora é obrigatória');
      return;
    }

    // Validar ano de publicação
    if (!this.anoPublicacao || this.anoPublicacao < 1900 || this.anoPublicacao > this.anoAtual) {
      this.error.set(`Ano de publicação deve estar entre 1900 e ${this.anoAtual}`);
      return;
    }

    // Validar edição
    if (!this.edicao || this.edicao < 1 || !Number.isInteger(this.edicao)) {
      this.error.set('Edição deve ser um número inteiro positivo');
      return;
    }

    // Validar autores
    if (this.autoresSelecionados.length === 0) {
      this.error.set('Selecione pelo menos um autor');
      return;
    }

    // Validar assuntos
    if (this.assuntosSelecionados.length === 0) {
      this.error.set('Selecione pelo menos um assunto');
      return;
    }

    // Validar preços
    if (this.precos.length === 0) {
      this.error.set('Adicione pelo menos um preço');
      return;
    }

    // Validar se todos os preços são válidos
    for (const preco of this.precos) {
      if (preco.valor <= 0) {
        this.error.set('Todos os preços devem ser maiores que zero');
        return;
      }
    }

    this.loading.set(true);
    this.error.set('');

    const request: LivroRequest = {
      titulo: this.titulo.trim(),
      editora: this.editora.trim(),
      edicao: this.edicao,
      anoPublicacao: String(this.anoPublicacao),
      autoresIds: this.autoresSelecionados,
      assuntosIds: this.assuntosSelecionados,
      precos: this.precos.map(p => ({ formaCompraId: p.formaCompraId, valor: p.valor }))
    };

    const operation = this.isEdit() && this.livroId
      ? this.apiService.updateLivro(this.livroId, request)
      : this.apiService.createLivro(request);

    operation.subscribe({
      next: () => {
        this.loading.set(false);
        
        if (this.isEdit()) {
          // Para edição, mostrar alert e redirecionar normalmente
          this.successMessage.set('Livro atualizado com sucesso!');
          setTimeout(() => {
            this.router.navigate(['/livros']);
          }, 2000);
        } else {
          // Para cadastro, redirecionar para tela de sucesso
          this.router.navigate(['/success'], {
            state: {
              message: 'Livro cadastrado com sucesso!',
              entityName: 'Livro',
              listRoute: '/livros',
              newRoute: '/livros/novo'
            }
          });
        }
      },
      error: (err: any) => {
        this.error.set(err.error?.message || 'Erro ao salvar livro');
        this.loading.set(false);
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/livros']);
  }

  limparFormulario(): void {
    this.titulo = '';
    this.editora = '';
    this.edicao = 1;
    this.anoPublicacao = new Date().getFullYear();
    this.autoresSelecionados = [];
    this.assuntosSelecionados = [];
    this.precos = [];
    this.selectedAutorId = '';
    this.selectedAssuntoId = '';
    this.selectedFormaCompra = null;
    this.valorPreco = 0;
    this.error.set('');
    this.formTouched = false;
  }

  // Funções de validação para o template
  tituloInvalido(): boolean {
    return this.formTouched && !this.titulo.trim();
  }

  editoraInvalida(): boolean {
    return this.formTouched && !this.editora.trim();
  }

  anoInvalido(): boolean {
    if (!this.formTouched) return false;
    return !this.anoPublicacao || this.anoPublicacao < 1900 || this.anoPublicacao > this.anoAtual;
  }

  edicaoInvalida(): boolean {
    if (!this.formTouched) return false;
    return !this.edicao || this.edicao < 1 || !Number.isInteger(this.edicao);
  }

  valorPrecoInvalido(): boolean {
    // O campo só fica vermelho se o usuário digitou um valor inválido (<=0)
    // Não fica vermelho se está vazio e já existem preços adicionados
    if (this.precos.length > 0 && (this.valorPreco === null || this.valorPreco === 0)) {
      return false; // Já tem preços adicionados, campo vazio não é erro
    }
    return this.valorPreco !== null && this.valorPreco <= 0;
  }
}

