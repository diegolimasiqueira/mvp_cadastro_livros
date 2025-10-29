// Pagination Models
export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

// Auth Models
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  expiresAt: string;
}

// Livro Models
export interface Livro {
  codI: number;
  titulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  autores: Autor[];
  assuntos: Assunto[];
  precos: LivroPreco[];
}

export interface LivroRequest {
  titulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  autoresIds: number[];
  assuntosIds: number[];
  precos: LivroPrecoRequest[];
}

export interface LivroPreco {
  codFc: number;
  formaCompraId: number;
  formaCompraDescricao: string;
  valor: number;
}

export interface LivroPrecoRequest {
  formaCompraId: number;
  valor: number;
}

// Autor Models
export interface Autor {
  codAu: number;
  nome: string;
}

export interface AutorRequest {
  nome: string;
}

// Assunto Models
export interface Assunto {
  codAs: number;
  descricao: string;
}

export interface AssuntoRequest {
  descricao: string;
}

// FormaCompra Models
export interface FormaCompra {
  codFc: number;
  descricao: string;
}

export interface FormaCompraRequest {
  descricao: string;
}

// Report Models
export interface BooksByAuthorReport {
  autorId: number;
  autorNome: string;
  livroId: number;
  livroTitulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  assuntos: string;
  formasCompra: string;
}

