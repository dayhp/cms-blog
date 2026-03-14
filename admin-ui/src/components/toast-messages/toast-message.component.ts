import { Component } from '@angular/core';
import { ToastModule } from 'primeng/toast';

@Component({
    selector: 'app-toast',
    standalone: true,
    imports: [ToastModule],
    styleUrl: './toast-message.component.scss',
    templateUrl: './toast-message.component.html',
})
export class ToastComponent { }