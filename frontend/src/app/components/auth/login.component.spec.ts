import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent, HttpClientTestingModule, FormsModule],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should login successfully and navigate to home', () => {
    component.username = 'admin';
    component.password = 'admin123';
    authService.login.and.returnValue(of({ token: 'mock-token', username: 'admin', expiresAt: '2025-12-31T23:59:59' }));

    component.onSubmit();

    expect(authService.login).toHaveBeenCalledWith({ username: 'admin', password: 'admin123' });
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should show error message on login failure', () => {
    component.username = 'admin';
    component.password = 'wrong';
    authService.login.and.returnValue(throwError(() => new Error('Unauthorized')));

    component.onSubmit();

    expect(component.error()).toBe('Credenciais inválidas');
    expect(component.loading()).toBeFalse();
  });

  it('should clear error message before login', () => {
    component.username = 'admin';
    component.password = 'admin123';
    component.error.set('Previous error');
    authService.login.and.returnValue(of({ token: 'mock-token', username: 'admin', expiresAt: '2025-12-31T23:59:59' }));

    component.onSubmit();

    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});

