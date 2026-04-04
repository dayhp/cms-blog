import { Component } from '@angular/core';
import { ToastService } from '../../../shared/services/alert.services';
import { AdminApiPostApiClient } from '../../../api/admin-api.service.generated';
import { TokenStorageService } from '../../../shared/services/token-storage.services';

@Component({
  selector: 'app-post',
  imports: [],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss',
})
export class PostComponent {
  constructor(
    private toastService: ToastService,
    private adminApiPostApiClient: AdminApiPostApiClient,
    private tokenStorageService: TokenStorageService) {

  }

  authenticate() {
    const token = this.tokenStorageService.getToken();
    this.adminApiPostApiClient.getById('54460bfb-24ac-402e-0c07-08de6fcc23ce').subscribe({
      next: (response: any) => {
        this.toastService.success('successful!');
      },
      error: (error) => {
        // Handle error response
        console.log(error, 'error');
      }
    });
  }
}
