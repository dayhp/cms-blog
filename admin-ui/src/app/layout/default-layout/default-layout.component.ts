import { Component, OnInit } from '@angular/core';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';

import { IconDirective } from '@coreui/icons-angular';
import {
  ContainerComponent,
  ShadowOnScrollDirective,
  SidebarBrandComponent,
  SidebarComponent,
  SidebarFooterComponent,
  SidebarHeaderComponent,
  SidebarNavComponent,
  SidebarToggleDirective,
  SidebarTogglerDirective
} from '@coreui/angular';

import { DefaultFooterComponent, DefaultHeaderComponent } from './';
import { navItems } from './_nav';
import { TokenStorageService } from '../../shared/services/token-storage.services';
import { UrlConstants } from '../../shared/constants/url.constants';

function isOverflown(element: HTMLElement) {
  return (
    element.scrollHeight > element.clientHeight ||
    element.scrollWidth > element.clientWidth
  );
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html',
  styleUrls: ['./default-layout.component.scss'],
  imports: [
    SidebarComponent,
    SidebarHeaderComponent,
    SidebarBrandComponent,
    SidebarNavComponent,
    SidebarFooterComponent,
    SidebarToggleDirective,
    SidebarTogglerDirective,
    ContainerComponent,
    DefaultFooterComponent,
    DefaultHeaderComponent,
    IconDirective,
    NgScrollbar,
    RouterOutlet,
    RouterLink,
    ShadowOnScrollDirective
  ]
})
export class DefaultLayoutComponent implements OnInit {
  public navItems = [...navItems];

  constructor(private tokenStorage: TokenStorageService, private router: Router) {}

  ngOnInit(): void {
    const user = this.tokenStorage.getUser();
    if (user == null) {
      this.router.navigate([UrlConstants.LOGIN]);
    } else {
      const permissions = JSON.parse(user.permissions);
      for (let index = 0; index < navItems.length; index++) {
        for (let childIndex = 0; childIndex < navItems[index].children?.length!; childIndex++) {
          if (navItems[index].children![childIndex].attributes &&
            permissions.filter((p: string) => p === navItems[index].children![childIndex].attributes!['policyName']).length == 0
          ) {
            navItems[index].children![childIndex].class = 'hidden';
          }
        }
      }
    }
  }
}
