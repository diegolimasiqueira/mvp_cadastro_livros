export interface HealthCheckResponse {
  status: string;
  database: DatabaseInfo;
  apiVersion: string;
  timestamp: string;
}

export interface DatabaseInfo {
  provider: string;
  host: string;
  port: string;
  databaseName: string;
  connectionUrl: string;
  isConnected: boolean;
  errorMessage?: string;
  responseTimeMs: number;
}

