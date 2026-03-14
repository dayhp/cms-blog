import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor(private messageService: MessageService) {}

  success(message: string, summary: string = 'Success') {
    this.messageService.add({
      severity: 'success',
      summary,
      detail: message
    });
  }

  error(message: string, summary: string = 'Error') {
    this.messageService.add({
      severity: 'error',
      summary,
      detail: message
    });
  }

  warn(message: string, summary: string = 'Warning') {
    this.messageService.add({
      severity: 'warn',
      summary,
      detail: message
    });
  }

  info(message: string, summary: string = 'Info') {
    this.messageService.add({
      severity: 'info',
      summary,
      detail: message
    });
  }
}