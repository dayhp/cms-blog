import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
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
export class LoginComponent {
  loginFrom: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private adminApiAuthApiClient: AdminApiAuthApiClient,
    private alertService: ToastService) {
    this.loginFrom = this.formBuilder.group({
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
    });
  }

  login() {
    if (this.loginFrom.valid) {
      const email = this.loginFrom.get('email')?.value;
      const password = this.loginFrom.get('password')?.value;
      const requestBody: LoginRequest = new LoginRequest({
        email: email,
        password: password
      });
      this.adminApiAuthApiClient.login(requestBody).subscribe({
        next: (response: AuthenticatedResult) => {
          // Handle successful login, e.g., store token, navigate to dashboard, etc.
          // Save token to local storage or a service
          //localStorage.setItem('authToken', response.accessToken);
          // Navigate to dashboard or home page
          // this.router.navigate(['/dashboard']);
        },
        error: (error) => {
          this.alertService.warn('Login failed. Please check your credentials and try again.');
          // Handle login failure, e.g., show error message to user
        }
      });
    }
  }
}
