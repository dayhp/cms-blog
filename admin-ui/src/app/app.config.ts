import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import {
  provideRouter,
  withEnabledBlockingInitialNavigation,
  withHashLocation,
  withInMemoryScrolling,
  withRouterConfig,
  withViewTransitions
} from '@angular/router';
import { IconSetService } from '@coreui/icons-angular';
import { routes } from './app.routes';
import {
  ADMIN_API_BASE_URL,
  AdminApiAuthApiClient,
  AdminApiPostApiClient,
  AdminApiRoleApiClient,
  AdminApiUserApiClient
} from './api/admin-api.service.generated';
import { environment } from '../environments/environment';
import { ToastService } from './shared/services/alert.services';
import { ToastModule } from 'primeng/toast';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TokenStorageService } from './shared/services/token-storage.services';
import { AuthGuard } from './shared/auth.guard';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './shared/interceptors/auth.interceptor';
import { RefreshTokenState } from './shared/interceptors/refresh-token.service';
import { PaginatorModule } from 'primeng/paginator';
import { CheckboxModule } from 'primeng/checkbox';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { ScrollerModule } from 'primeng/scroller';
import { TableModule } from 'primeng/table';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { CommonSharedModule } from './shared/modules/common-shared.module';
import { KeyFilterModule } from 'primeng/keyfilter';
import { ValidationMessageComponent } from './shared/modules/validate/validation-message.component';
import { PermissionDirective } from './shared/modules/permission/permission.directive';
import { UtilityService } from './shared/services/utility.service';
import { RoleDetailComponent } from './views/system/roles/role-detail.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { PermissionGrantComponent } from './views/system/roles/permission-grant.component';
import { DecimalPipe } from '@angular/common';
import { BadgeModule } from 'primeng/badge';
import { ImageModule } from 'primeng/image';
import { UserDetailComponent } from './views/system/users/user-detail.component';
import { SetPasswordComponent } from './views/system/users/set-password.component';

export const appConfig: ApplicationConfig = {
  providers: [
    {
      provide: ADMIN_API_BASE_URL,
      useValue: environment.apiBaseUrl
    },
    provideRouter(routes,
      withRouterConfig({
        onSameUrlNavigation: 'reload'
      }),
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled'
      }),
      withEnabledBlockingInitialNavigation(),
      withViewTransitions(),
      withHashLocation()
    ),
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    IconSetService,
    AdminApiAuthApiClient,
    AdminApiPostApiClient,
    AdminApiRoleApiClient,
    AdminApiUserApiClient,
    MessageService,
    ToastService,
    TokenStorageService,
    DialogService,
    ConfirmationService,
    AuthGuard,
    RefreshTokenState,
    DecimalPipe,
    UtilityService,
    importProvidersFrom(
      ToastModule,
      ConfirmDialogModule,
      DynamicDialogModule,
      PaginatorModule,
      CheckboxModule,
      PanelModule,
      ButtonModule,
      ScrollerModule,
      TableModule,
      ProgressSpinnerModule,
      BlockUIModule,
      KeyFilterModule,
      BadgeModule,
      ImageModule
    ),
    provideAnimationsAsync(),
  ]
};

