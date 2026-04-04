import { Component, OnDestroy } from '@angular/core';
import { IconDirective } from '@coreui/icons-angular';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  ButtonDirective,
  CardBodyComponent,
  CardComponent,
  CardGroupComponent,
  ColComponent,
  ContainerComponent,
  FormControlDirective,
  FormDirective,
  InputGroupComponent,
  InputGroupTextDirective,
  RowComponent
} from '@coreui/angular';
import { AdminApiAuthApiClient, AuthenticatedResult, LoginRequest } from '../../../api/admin-api.service.generated';
import { ToastService } from '../../../shared/services/alert.services';
import { ToastModule } from 'primeng/toast';
import { Router } from '@angular/router';
import { UrlConstants } from '../../../shared/constants/url.constants';
import { TokenStorageService } from '../../../shared/services/token-storage.services';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  standalone: true,
  imports: [
    ContainerComponent,
    RowComponent,
    ColComponent,
    CardGroupComponent,
    CardComponent,
    CardBodyComponent,
    FormDirective,
    InputGroupComponent,
    InputGroupTextDirective,
    IconDirective,
    FormControlDirective,
    ButtonDirective,
    ReactiveFormsModule,
    ToastModule
  ]
})
export class LoginComponent implements OnDestroy {
  loginFrom: FormGroup;
  private ngUnsubscribe = new Subject<void>();
  loading: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private adminApiAuthApiClient: AdminApiAuthApiClient,
    private alertService: ToastService,
    private router: Router,
    private tokenStorageService: TokenStorageService) {
    this.loginFrom = this.formBuilder.group({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  login() {
    if (this.loginFrom.valid) {
      this.loading = true;
      const email = this.loginFrom.get('email')?.value;
      const password = this.loginFrom.get('password')?.value;
      const requestBody: LoginRequest = new LoginRequest({
        email: email,
        password: password
      });
      this.adminApiAuthApiClient.login(requestBody).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
        next: (response: AuthenticatedResult) => {
          const refreshToken: string = response.refreshToken ?? ''; 
           const accessToken: string = response.accessToken ?? ''; 
          this.tokenStorageService.saveRefreshToken(refreshToken);
          this.tokenStorageService.saveToken(accessToken);
          this.tokenStorageService.saveUser(response);
          this.alertService.success('Login successful!');
          // Navigate to dashboard or home page
          this.router.navigate([UrlConstants.DASHBOARD]);
          this.loading = false;
        },
        error: (error) => {
          this.alertService.error('Login failed. Please check your credentials and try again.');
          // Handle login failure, e.g., show error message to user
        }
      });
    }
  }
}
