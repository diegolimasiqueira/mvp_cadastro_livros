import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-success',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mt-5">
      <div class="row justify-content-center">
        <div class="col-md-6">
          <div class="card border-success">
            <div class="card-body text-center p-5">
              <div class="mb-4">
                <i class="bi bi-check-circle-fill text-success" style="font-size: 5rem;"></i>
              </div>
              
              <h2 class="card-title text-success mb-3">{{ message }}</h2>
              
              <p class="card-text text-muted mb-4">
                O que você deseja fazer agora?
              </p>
              
              <div class="d-flex gap-3 justify-content-center">
                <button class="btn btn-primary btn-lg" (click)="cadastrarNovo()">
                  <i class="bi bi-plus-circle"></i> Cadastrar Novo {{ entityName }}
                </button>
                <button class="btn btn-outline-secondary btn-lg" (click)="voltarLista()">
                  <i class="bi bi-list"></i> Ver Lista
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card {
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
  `]
})
export class SuccessComponent implements OnInit {
  message: string = 'Operação realizada com sucesso!';
  entityName: string = 'Item';
  listRoute: string = '/';
  newRoute: string = '/';

  constructor(
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Pegar parâmetros da navegação
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras?.state || history.state;

    if (state && state['message']) {
      this.message = state['message'];
      this.entityName = state['entityName'] || 'Item';
      this.listRoute = state['listRoute'] || '/';
      this.newRoute = state['newRoute'] || '/';
    } else {
      // Se não houver state, redireciona para home
      this.router.navigate(['/']);
    }
  }

  cadastrarNovo(): void {
    this.router.navigate([this.newRoute]);
  }

  voltarLista(): void {
    this.router.navigate([this.listRoute]);
  }
}



