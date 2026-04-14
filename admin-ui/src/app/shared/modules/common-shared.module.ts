import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ValidationMessageComponent } from './validate/validation-message.component';
import { PermissionDirective } from './permission/permission.directive';
@NgModule({
  imports: [CommonModule, ValidationMessageComponent, PermissionDirective],
  // declarations: [ValidationMessageComponent, PermissionDirective],
  exports: [ValidationMessageComponent],
})
export class CommonSharedModule {}