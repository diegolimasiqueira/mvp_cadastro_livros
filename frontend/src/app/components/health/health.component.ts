import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { HealthCheckResponse } from '../../models';

@Component({
  selector: 'app-health',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container-fluid py-4">
      <div class="row mb-4">
        <div class="col-12">
          <div class="d-flex justify-content-between align-items-center">
            <h2>
              <i class="bi bi-heart-pulse"></i> Status do Sistema
            </h2>
            <button 
              class="btn btn-primary" 
              (click)="loadHealthCheck()" 
              [disabled]="loading()">
              <i class="bi" [class.bi-arrow-clockwise]="!loading()" [class.bi-hourglass-split]="loading()"></i>
              {{ loading() ? 'Carregando...' : 'Atualizar' }}
            </button>
          </div>
        </div>
      </div>

      @if (loading() && !healthData()) {
        <div class="d-flex justify-content-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Carregando...</span>
          </div>
        </div>
      } @else if (error()) {
        <div class="alert alert-danger" role="alert">
          <div class="d-flex align-items-start mb-3">
            <i class="bi bi-exclamation-triangle-fill me-3 fs-3"></i>
            <div>
              <h5 class="alert-heading mb-2">Erro ao carregar status do sistema!</h5>
              <p class="mb-0">Não foi possível conectar com a API ou obter informações de health check.</p>
            </div>
          </div>
          
          <hr>
          
          <div class="error-details">
            <h6><i class="bi bi-info-circle"></i> Detalhes do Erro:</h6>
            <div class="bg-dark text-light p-3 rounded">
              <pre class="mb-0 text-light" style="white-space: pre-wrap; word-wrap: break-word;">{{ error() }}</pre>
            </div>
            
            <div class="mt-3">
              <strong>Possíveis causas:</strong>
              <ul class="mb-0 mt-2">
                <li>Backend não está rodando (verifique se <code>http://localhost:8080</code> está acessível)</li>
                <li>Problemas de CORS (Cross-Origin Resource Sharing)</li>
                <li>Firewall bloqueando a conexão</li>
                <li>Timeout de conexão</li>
                <li>Erro de rede local</li>
              </ul>
            </div>
            
            <div class="mt-3">
              <strong>Como resolver:</strong>
              <ul class="mb-0 mt-2">
                <li>Execute: <code>docker compose up -d</code></li>
                <li>Verifique logs: <code>docker logs bookstore-backend</code></li>
                <li>Teste diretamente: <code>curl http://localhost:8080/api/health</code></li>
              </ul>
            </div>
          </div>
        </div>
      } @else if (healthData()) {
        <!-- Status Geral da API -->
        <div class="row mb-4">
          <div class="col-lg-6 mb-4">
            <div class="card h-100 shadow-sm">
              <div class="card-header" [class.bg-success]="healthData()!.status === 'Healthy'" 
                                       [class.bg-warning]="healthData()!.status === 'Degraded'"
                                       [class.bg-danger]="healthData()!.status === 'Unhealthy'"
                                       [class.text-white]="healthData()!.status !== 'Degraded'">
                <h5 class="mb-0">
                  <i class="bi" [class.bi-check-circle-fill]="healthData()!.status === 'Healthy'"
                                [class.bi-exclamation-triangle-fill]="healthData()!.status === 'Degraded'"
                                [class.bi-x-circle-fill]="healthData()!.status === 'Unhealthy'"></i>
                  Status da API
                </h5>
              </div>
              <div class="card-body">
                <div class="status-item">
                  <strong>Status:</strong>
                  <span class="badge" 
                        [class.bg-success]="healthData()!.status === 'Healthy'"
                        [class.bg-warning]="healthData()!.status === 'Degraded'"
                        [class.bg-danger]="healthData()!.status === 'Unhealthy'">
                    {{ healthData()!.status }}
                  </span>
                </div>
                <div class="status-item">
                  <strong>Versão:</strong>
                  <span class="text-muted">{{ healthData()!.apiVersion }}</span>
                </div>
                <div class="status-item">
                  <strong>Última verificação:</strong>
                  <span class="text-muted">{{ formatTimestamp(healthData()!.timestamp) }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Status do Banco de Dados -->
          <div class="col-lg-6 mb-4">
            <div class="card h-100 shadow-sm">
              <div class="card-header" [class.bg-success]="healthData()!.database.isConnected" 
                                       [class.bg-danger]="!healthData()!.database.isConnected"
                                       [class.text-white]="true">
                <h5 class="mb-0">
                  <i class="bi" [class.bi-database-check]="healthData()!.database.isConnected"
                                [class.bi-database-x]="!healthData()!.database.isConnected"></i>
                  Status do Banco de Dados
                </h5>
              </div>
              <div class="card-body">
                <div class="status-item">
                  <strong>Conexão:</strong>
                  <span class="badge" 
                        [class.bg-success]="healthData()!.database.isConnected"
                        [class.bg-danger]="!healthData()!.database.isConnected">
                    <i class="bi" [class.bi-check-lg]="healthData()!.database.isConnected"
                                  [class.bi-x-lg]="!healthData()!.database.isConnected"></i>
                    {{ healthData()!.database.isConnected ? 'Conectado' : 'Desconectado' }}
                  </span>
                </div>
                <div class="status-item">
                  <strong>Tempo de resposta:</strong>
                  <span class="badge" 
                        [class.bg-success]="healthData()!.database.responseTimeMs < 100"
                        [class.bg-warning]="healthData()!.database.responseTimeMs >= 100 && healthData()!.database.responseTimeMs < 500"
                        [class.bg-danger]="healthData()!.database.responseTimeMs >= 500">
                    {{ healthData()!.database.responseTimeMs }} ms
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Detalhes do Banco de Dados -->
        <div class="row">
          <div class="col-12">
            <div class="card shadow-sm">
              <div class="card-header bg-primary text-white">
                <h5 class="mb-0">
                  <i class="bi bi-info-circle"></i> Informações do Banco de Dados
                </h5>
              </div>
              <div class="card-body">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <div class="info-box">
                      <div class="info-label">
                        <i class="bi bi-hdd-stack text-primary"></i> Provedor
                      </div>
                      <div class="info-value">{{ healthData()!.database.provider }}</div>
                    </div>
                  </div>
                  
                  <div class="col-md-6 mb-3">
                    <div class="info-box">
                      <div class="info-label">
                        <i class="bi bi-server text-primary"></i> Host
                      </div>
                      <div class="info-value">{{ healthData()!.database.host }}</div>
                    </div>
                  </div>
                  
                  <div class="col-md-6 mb-3">
                    <div class="info-box">
                      <div class="info-label">
                        <i class="bi bi-hdd-network text-primary"></i> Porta
                      </div>
                      <div class="info-value">{{ healthData()!.database.port }}</div>
                    </div>
                  </div>
                  
                  <div class="col-md-6 mb-3">
                    <div class="info-box">
                      <div class="info-label">
                        <i class="bi bi-database text-primary"></i> Nome do Banco
                      </div>
                      <div class="info-value">{{ healthData()!.database.databaseName }}</div>
                    </div>
                  </div>
                  
                  <div class="col-12 mb-3">
                    <div class="info-box">
                      <div class="info-label">
                        <i class="bi bi-link-45deg text-primary"></i> URL de Conexão
                      </div>
                      <div class="info-value">
                        <code class="bg-light p-2 d-inline-block">{{ healthData()!.database.connectionUrl }}</code>
                      </div>
                    </div>
                  </div>
                  
                  @if (healthData()!.database.errorMessage) {
                    <div class="col-12">
                      <div class="alert alert-danger mb-0">
                        <strong>
                          <i class="bi bi-exclamation-triangle-fill"></i> Erro:
                        </strong>
                        <div class="mt-2">
                          <code>{{ healthData()!.database.errorMessage }}</code>
                        </div>
                      </div>
                    </div>
                  }
                </div>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .status-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 10px 0;
      border-bottom: 1px solid #e9ecef;
    }

    .status-item:last-child {
      border-bottom: none;
    }

    .info-box {
      background: #f8f9fa;
      padding: 15px;
      border-radius: 8px;
      border-left: 4px solid #0d6efd;
    }

    .info-label {
      font-size: 0.85rem;
      font-weight: 600;
      color: #6c757d;
      margin-bottom: 8px;
    }

    .info-value {
      font-size: 1.1rem;
      font-weight: 500;
      color: #212529;
    }

    .card-header h5 {
      display: flex;
      align-items: center;
      gap: 10px;
    }

    code {
      font-size: 0.9rem;
      border-radius: 4px;
    }

    .badge {
      font-size: 0.9rem;
      padding: 6px 12px;
    }

    .bi {
      font-size: 1.2rem;
    }
  `]
})
export class HealthComponent implements OnInit {
  healthData = signal<HealthCheckResponse | null>(null);
  loading = signal(false);
  error = signal<string | null>(null);

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.loadHealthCheck();
    // Auto-refresh a cada 30 segundos
    setInterval(() => this.loadHealthCheck(), 30000);
  }

  loadHealthCheck(): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.apiService.getHealthCheck().subscribe({
      next: (data) => {
        this.healthData.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        // Capturar TODOS os detalhes do erro para debugging
        const errorDetails = {
          message: err.message || 'Erro desconhecido',
          status: err.status || 0,
          statusText: err.statusText || 'Unknown',
          url: err.url || 'http://localhost:8080/api/health/detailed',
          name: err.name || 'Error',
          error: err.error || null,
          headers: err.headers ? this.extractHeaders(err.headers) : null,
          timestamp: new Date().toISOString()
        };
        
        // Formatar como JSON legível
        const formattedError = JSON.stringify(errorDetails, null, 2);
        this.error.set(formattedError);
        this.loading.set(false);
      }
    });
  }

  private extractHeaders(headers: any): any {
    const headerObj: any = {};
    if (headers && headers.keys) {
      headers.keys().forEach((key: string) => {
        headerObj[key] = headers.get(key);
      });
    }
    return headerObj;
  }

  formatTimestamp(timestamp: string): string {
    const date = new Date(timestamp);
    return date.toLocaleString('pt-BR');
  }
}

