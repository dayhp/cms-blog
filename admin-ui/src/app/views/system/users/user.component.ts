import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ToastService } from '../../../shared/services/alert.services';
import { AdminApiUserApiClient, RoleDto, UserDto, UserDtoPageResult } from '../../../api/admin-api.service.generated';
import { PaginatorModule } from 'primeng/paginator';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { TableModule } from 'primeng/table';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { Subject, takeUntil, timeout } from 'rxjs';
import { CommonModule, DecimalPipe } from '@angular/common';
import { BadgeModule } from 'primeng/badge';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { MessageConstants } from '../../../shared/constants/messages.constant';
import { UserDetailComponent } from './user-detail.component';
import { SetPasswordComponent } from './set-password.component';

@Component({
  selector: 'app-user',
  imports: [
    PaginatorModule,
    CheckboxModule,
    ButtonModule,
    InputTextModule,
    PanelModule,
    ProgressSpinnerModule,
    BlockUIModule,
    TableModule,
    FormsModule,
    ConfirmDialogModule,
    DecimalPipe,
    BadgeModule,
    CommonModule,
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserComponent implements OnInit, OnDestroy {
  //System variables
  private ngUnsubscribe = new Subject<void>();
  public blockedPanel: boolean = false;

  //Paging variables
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalCount: number = 0;

  //Business variables
  public items: UserDto[] = [];
  public selectedItems: UserDto[] = [];
  public keyword: string = '';
  ref: DynamicDialogRef | null | undefined;

  constructor(
    private userService: AdminApiUserApiClient,
    private toastService: ToastService,
    public dialogService: DialogService,
    private alertService: ToastService,
    private confirmationService: ConfirmationService,
    private cd: ChangeDetectorRef,) {
  }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit() {
    this.cd.detectChanges();
    this.loadData();
  }

  loadData(selectionId = null) {
    this.blockedPanel = true;
    this.userService
      .getUsersPaging(this.keyword, this.pageIndex, this.pageSize)
      .pipe(timeout(5000), takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: UserDtoPageResult) => {
          this.items = response.results ?? [];
          this.totalCount = response.rowCount ?? 0;
          if (selectionId != null && this.items.length > 0) {
            this.selectedItems = this.items.filter(
              (x) => x.id == selectionId
            );
          }
          this.blockedPanel = false;
          this.cd.detectChanges()
        },
        error: () => {
          this.blockedPanel = false;
        },
      });
  }

  showAddModal() {
    this.ref = this.dialogService.open(UserDetailComponent, {
      header: 'Thêm mới người dùng',
      width: '60%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
    });
    if (this.ref) {
      this.ref.onClose.subscribe((data: UserDto) => {
        if (data) {
          this.alertService.success(MessageConstants.CREATED_OK_MSG);
          this.selectedItems = [];
          this.loadData();
        }
      });
    }
  }

  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.alertService.error(
        MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    this.ref = this.dialogService.open(UserDetailComponent, {
      header: 'Cập nhật người dùng',
      width: '60%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { id: this.selectedItems[0].id },
    });
    if (this.ref) {
      this.ref.onClose.subscribe((data: UserDto) => {
        if (data) {
          this.alertService.success(MessageConstants.UPDATED_OK_MSG);
          this.selectedItems = [];
          this.loadData();
        }
      });
    }
  }

  deleteItems() {
    if (this.selectedItems.length == 0) {
      this.alertService.error(
        MessageConstants.NOT_CHOOSE_ANY_RECORD
      );
      return;
    }
    var ids: any = [];
    this.selectedItems.forEach((element) => {
      ids.push(element.id);
    });
    this.confirmationService.confirm({
      message: MessageConstants.CONFIRM_DELETE_MSG,
      accept: () => {
        this.deleteItemsConfirm(ids);
      },
    });
  }

  deleteItemsConfirm(ids: any[]) {
    this.blockedPanel = true;
    this.userService.delete(ids).subscribe({
      next: () => {
        this.alertService.success(
          MessageConstants.DELETED_OK_MSG
        );
        this.loadData();
        this.selectedItems = [];
        this.blockedPanel = false;
      },
      error: () => {
        this.blockedPanel = false;
      },
    });
  }

  setPassword(id: string) {
    this.ref = this.dialogService.open(SetPasswordComponent, {
      header: 'Set password',
      width: '60%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { id: id },
    });
    if (this.ref) {
      this.ref.onClose.subscribe((result: boolean) => {
        if (result) {
          this.alertService.success(MessageConstants.CHANGE_PASSWORD_SUCCCESS_MSG);
          this.selectedItems = [];
          this.loadData();
        }
      });
    }
  }


  pageChanged(event: any): void {
    this.pageIndex = event.page + 1;
    this.pageSize = event.rows;
    this.loadData();
  }

}
