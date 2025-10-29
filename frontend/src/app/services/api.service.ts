import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Livro, LivroRequest, Autor, AutorRequest, Assunto, AssuntoRequest, 
         FormaCompra, FormaCompraRequest, BooksByAuthorReport, PagedResponse, HealthCheckResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly API_URL = 'http://localhost:8080/api';

  constructor(private http: HttpClient) {}

  // Livros
  getLivros(): Observable<Livro[]> {
    return this.http.get<Livro[]>(`${this.API_URL}/livros`);
  }

  getLivrosPaged(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<Livro>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<Livro>>(`${this.API_URL}/livros/paged`, { params });
  }

  getLivro(id: number): Observable<Livro> {
    return this.http.get<Livro>(`${this.API_URL}/livros/${id}`);
  }

  createLivro(livro: LivroRequest): Observable<Livro> {
    return this.http.post<Livro>(`${this.API_URL}/livros`, livro);
  }

  updateLivro(id: number, livro: LivroRequest): Observable<Livro> {
    return this.http.put<Livro>(`${this.API_URL}/livros/${id}`, livro);
  }

  deleteLivro(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/livros/${id}`);
  }

  // Autores
  getAutores(): Observable<Autor[]> {
    return this.http.get<Autor[]>(`${this.API_URL}/autores`);
  }

  getAutoresPaged(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<Autor>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<Autor>>(`${this.API_URL}/autores/paged`, { params });
  }

  getAutor(id: number): Observable<Autor> {
    return this.http.get<Autor>(`${this.API_URL}/autores/${id}`);
  }

  createAutor(autor: AutorRequest): Observable<Autor> {
    return this.http.post<Autor>(`${this.API_URL}/autores`, autor);
  }

  updateAutor(id: number, autor: AutorRequest): Observable<Autor> {
    return this.http.put<Autor>(`${this.API_URL}/autores/${id}`, autor);
  }

  deleteAutor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/autores/${id}`);
  }

  // Assuntos
  getAssuntos(): Observable<Assunto[]> {
    return this.http.get<Assunto[]>(`${this.API_URL}/assuntos`);
  }

  getAssuntosPaged(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<Assunto>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<Assunto>>(`${this.API_URL}/assuntos/paged`, { params });
  }

  getAssunto(id: number): Observable<Assunto> {
    return this.http.get<Assunto>(`${this.API_URL}/assuntos/${id}`);
  }

  createAssunto(assunto: AssuntoRequest): Observable<Assunto> {
    return this.http.post<Assunto>(`${this.API_URL}/assuntos`, assunto);
  }

  updateAssunto(id: number, assunto: AssuntoRequest): Observable<Assunto> {
    return this.http.put<Assunto>(`${this.API_URL}/assuntos/${id}`, assunto);
  }

  deleteAssunto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/assuntos/${id}`);
  }

  // Formas de Compra
  getFormasCompra(): Observable<FormaCompra[]> {
    return this.http.get<FormaCompra[]>(`${this.API_URL}/formascompra`);
  }

  getFormasCompraPaged(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<FormaCompra>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<FormaCompra>>(`${this.API_URL}/formascompra/paged`, { params });
  }

  getFormaCompra(id: number): Observable<FormaCompra> {
    return this.http.get<FormaCompra>(`${this.API_URL}/formascompra/${id}`);
  }

  createFormaCompra(request: FormaCompraRequest): Observable<FormaCompra> {
    return this.http.post<FormaCompra>(`${this.API_URL}/formascompra`, request);
  }

  updateFormaCompra(id: number, request: FormaCompraRequest): Observable<FormaCompra> {
    return this.http.put<FormaCompra>(`${this.API_URL}/formascompra/${id}`, request);
  }

  deleteFormaCompra(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/formascompra/${id}`);
  }

  // Reports
  getBooksByAuthorReport(): Observable<BooksByAuthorReport[]> {
    return this.http.get<BooksByAuthorReport[]>(`${this.API_URL}/reports/books-by-author`);
  }

  downloadBooksByAuthorReportPdf(): Observable<Blob> {
    return this.http.get(`${this.API_URL}/reports/books-by-author/pdf`, {
      responseType: 'blob'
    });
  }

  downloadBooksByAuthorReportExcel(): Observable<Blob> {
    return this.http.get(`${this.API_URL}/reports/books-by-author/excel`, {
      responseType: 'blob'
    });
  }

  // Health Check
  getHealthCheck(): Observable<HealthCheckResponse> {
    return this.http.get<HealthCheckResponse>(`${this.API_URL}/health/detailed`);
  }
}

