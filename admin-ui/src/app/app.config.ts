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
  AdminApiPostApiClient
} from './api/admin-api.service.generated';
import { environment } from '../environments/environment';
import { ToastService } from './shared/services/alert.services';
import { providePrimeNG } from 'primeng/config';
import { ToastModule } from 'primeng/toast';
import Aura from '@primeuix/themes/aura';
import { MessageService } from 'primeng/api';
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
    MessageService,
    ToastService,
    ToastModule,
    TokenStorageService,
    AuthGuard,
    AdminApiPostApiClient,
    RefreshTokenState,
    PaginatorModule,
    CheckboxModule,
    PanelModule,
    ButtonModule,
    ScrollerModule,
    importProvidersFrom(ToastModule),
    provideAnimationsAsync(),
  ]
};

