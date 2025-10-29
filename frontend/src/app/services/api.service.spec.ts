import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from './api.service';
import { Livro, Autor, Assunto } from '../models';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;
  const baseUrl = 'http://localhost:8080/api';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiService]
    });
    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('Livros', () => {
    it('should get all livros', (done) => {
      const mockLivros: Livro[] = [
        { codI: 1, titulo: 'Test Book', editora: 'Test', edicao: 1, anoPublicacao: '2024', autores: [], assuntos: [], precos: [] }
      ];

      service.getLivros().subscribe({
        next: (livros) => {
          expect(livros).toEqual(mockLivros);
          done();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/livros`);
      expect(req.request.method).toBe('GET');
      req.flush(mockLivros);
    });

    it('should delete livro', (done) => {
      service.deleteLivro(1).subscribe({
        next: () => {
          expect(true).toBeTrue();
          done();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/livros/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
    });
  });

  describe('Autores', () => {
    it('should get all autores', (done) => {
      const mockAutores: Autor[] = [
        { codAu: 1, nome: 'Test Author' }
      ];

      service.getAutores().subscribe({
        next: (autores) => {
          expect(autores).toEqual(mockAutores);
          done();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/autores`);
      expect(req.request.method).toBe('GET');
      req.flush(mockAutores);
    });
  });

  describe('Assuntos', () => {
    it('should get all assuntos', (done) => {
      const mockAssuntos: Assunto[] = [
        { codAs: 1, descricao: 'Test Subject' }
      ];

      service.getAssuntos().subscribe({
        next: (assuntos) => {
          expect(assuntos).toEqual(mockAssuntos);
          done();
        }
      });

      const req = httpMock.expectOne(`${baseUrl}/assuntos`);
      expect(req.request.method).toBe('GET');
      req.flush(mockAssuntos);
    });
  });
});



