import { Injectable } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, from, map, catchError, of } from 'rxjs';
import { ConfirmationModalComponent, ConfirmationConfig } from '../components/shared/confirmation-modal.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmationService {
  constructor(private modalService: NgbModal) {}

  confirm(config: ConfirmationConfig): Observable<boolean> {
    const modalRef = this.modalService.open(ConfirmationModalComponent, {
      centered: true,
      backdrop: 'static'
    });

    modalRef.componentInstance.title = config.title;
    modalRef.componentInstance.message = config.message;
    modalRef.componentInstance.confirmText = config.confirmText || 'Confirmar';
    modalRef.componentInstance.cancelText = config.cancelText || 'Cancelar';
    modalRef.componentInstance.type = config.type || 'danger';

    return from(modalRef.result).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }
}




