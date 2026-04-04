import { Component } from '@angular/core';
import { ToastService } from '../../../shared/services/alert.services';
import { AdminApiPostApiClient } from '../../../api/admin-api.service.generated';
import { TokenStorageService } from '../../../shared/services/token-storage.services';
import { PaginatorModule } from 'primeng/paginator';
import { CheckboxModule } from 'primeng/checkbox';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
@Component({
  selector: 'app-role',
  imports: [
    PaginatorModule,
     CheckboxModule,
     ButtonModule,
     InputTextModule,
     PanelModule],
  templateUrl: './role.component.html',
  styleUrl: './role.component.scss',
})
export class RoleComponent {
  constructor(
    private toastService: ToastService,
    private adminApiPostApiClient: AdminApiPostApiClient,
    private tokenStorageService: TokenStorageService) {

  }
}
