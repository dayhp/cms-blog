import { Component } from '@angular/core';
import { ToastService } from '../../../shared/services/alert.services';
import { AdminApiPostApiClient } from '../../../api/admin-api.service.generated';
import { TokenStorageService } from '../../../shared/services/token-storage.services';

@Component({
  selector: 'app-user',
  imports: [],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserComponent {
  constructor(
    private toastService: ToastService,
    private adminApiPostApiClient: AdminApiPostApiClient,
    private tokenStorageService: TokenStorageService) {

  }
}
