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
  AdminApiRoleApiClient
} from './api/admin-api.service.generated';
import { environment } from '../environments/environment';
import { ToastService } from './shared/services/alert.services';
import { providePrimeNG } from 'primeng/config';
import { ToastModule } from 'primeng/toast';
import Aura from '@primeuix/themes/aura';
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
    providePrimeNG({
      theme: {
        preset: Aura
      }
    }),
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    IconSetService,
    AdminApiAuthApiClient,
    AdminApiPostApiClient,
    AdminApiRoleApiClient,
    MessageService,
    ToastService,
    TokenStorageService,
    DialogService,
    DynamicDialogModule,
    ConfirmationService,
    ToastModule,
    AuthGuard,
    RefreshTokenState,
    PaginatorModule,
    CheckboxModule,
    PanelModule,
    ButtonModule,
    ScrollerModule,
    TableModule,
    ProgressSpinnerModule,
    BlockUIModule,
    KeyFilterModule,
    ValidationMessageComponent,
    PermissionDirective,
    UtilityService,
    RoleDetailComponent,
    // CommonSharedModule,
    importProvidersFrom(ToastModule),
    provideAnimationsAsync(),
  ]
};

