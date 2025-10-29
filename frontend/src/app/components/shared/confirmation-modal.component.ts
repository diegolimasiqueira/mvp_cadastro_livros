import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

export interface ConfirmationConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  type?: 'danger' | 'warning' | 'info';
}

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [],
  template: `
    <div class="modal-header bg-{{ type }} text-white">
      <h4 class="modal-title">
        <i class="bi bi-exclamation-triangle-fill me-2"></i>{{ title }}
      </h4>
      <button type="button" class="btn-close btn-close-white" aria-label="Close" (click)="activeModal.dismiss()"></button>
    </div>
    <div class="modal-body">
      <div class="alert alert-{{ type }} mb-0" role="alert">
        <strong>Atenção!</strong> {{ message }}
      </div>
    </div>
    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" (click)="activeModal.dismiss()">
        {{ cancelText }}
      </button>
      <button type="button" class="btn btn-{{ type }}" (click)="activeModal.close(true)">
        {{ confirmText }}
      </button>
    </div>
  `
})
export class ConfirmationModalComponent {
  @Input() title = '';
  @Input() message = '';
  @Input() confirmText = 'Confirmar';
  @Input() cancelText = 'Cancelar';
  @Input() type: 'danger' | 'warning' | 'info' = 'danger';

  constructor(public activeModal: NgbActiveModal) {}
}

