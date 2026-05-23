import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { AdminApiRoleApiClient, RoleDto, RoleDtoPageResult } from '../../../api/admin-api.service.generated';
import { PaginatorModule } from 'primeng/paginator';
import { CheckboxModule } from 'primeng/checkbox';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { BlockUIModule } from 'primeng/blockui';
import { TableModule } from 'primeng/table';
import { Subject, takeUntil, timeout } from 'rxjs';
import { DialogService, DynamicDialogModule, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ConfirmationService } from 'primeng/api';
import { FormsModule } from '@angular/forms';
import { RoleDetailComponent } from './role-detail.component';
import { PermissionGrantComponent } from './permission-grant.component';
import { ToastService } from '../../../shared/services/alert.services';
import { MessageConstants } from '../../../shared/constants/messages.constant';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
@Component({
  selector: 'app-role',
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
    ConfirmDialogModule
  ],
  templateUrl: './role.component.html',
  styleUrl: './role.component.scss',
})
export class RoleComponent implements OnInit, OnDestroy {
  constructor(
    private roleService: AdminApiRoleApiClient,
    private dialogService: DialogService,
    private confirmationService: ConfirmationService,
    private alertService: ToastService,
    private cd: ChangeDetectorRef,
  ) {

  }
  public ngSubscribe = new Subject<void>();
  public blockedPanel: boolean = false;
  ref: DynamicDialogRef | null | undefined;

  ngOnDestroy(): void {
    this.ngSubscribe.next();
    this.ngSubscribe.complete();
  }

  // pagination
  public pageIndex: number = 1;
  public pageSize: number = 10;
  public totalRecords: number = 0;

  // Business logic
  public items: RoleDto[] = [];
  public searchTerm: string = '';
  public selectedItems: RoleDto[] = [];

  ngOnInit(): void {
    this.cd.detectChanges();
    this.loadData();
  }

  loadData() {
    this.blockedPanel = true;
    this.roleService
      .getRolesAllPaging(this.searchTerm, this.pageIndex, this.pageSize)
      .pipe(timeout(5000), takeUntil(this.ngSubscribe))
      .subscribe({
        next: (response: RoleDtoPageResult) => {
          this.items = response.results ?? [];
          this.totalRecords = response.rowCount ?? 0;
          this.blockedPanel = false;
          this.cd.detectChanges()
        },
        error: (e) => {
          this.blockedPanel = false;
        },
      });
  }

  pageChanged(event: any): void {
    this.pageIndex = event.page + 1;
    this.pageSize = event.rows;
    this.loadData();
  }

  pageChange(event: any) {
    this.pageIndex = event.page;
    this.pageSize = event.rows;
    this.loadData();
  }

  showPermissionModal(roleId: number, roleName: string) {
    this.ref = this.dialogService.open(PermissionGrantComponent, {
      header: `Phân quyền cho ${roleName}`,
      width: '60%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
      data: { roleId: roleId }
    });

    // Handle dialog closure
    if (this.ref) {
      this.ref.onClose.subscribe((data: RoleDto) => {
        if (data) {
          this.alertService.success(MessageConstants.UPDATED_OK_MSG);
          this.selectedItems = [];
          this.loadData();
        }
      });
    }
  }

  showAddModal() {
    this.ref = this.dialogService.open(RoleDetailComponent, {
      header: 'Thêm mới quyền',
      width: '60%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      closable: true,
    });

    // Handle dialog closure
    if (this.ref) {
      this.ref.onClose.subscribe((data: RoleDto) => {
        if (data) {
          this.alertService.success(MessageConstants.UPDATED_OK_MSG);
          this.selectedItems = [];
          this.loadData();
        }
      });
    }
  }


  showEditModal() {
    if (this.selectedItems.length == 0) {
      this.alertService.error(MessageConstants.NOT_CHOOSE_ANY_RECORD);
      return;
    }
    var id = this.selectedItems[0].id;
    this.ref = this.dialogService.open(RoleDetailComponent, {
      header: 'Cập nhật quyền',
      width: '60%',
      contentStyle: { overflow: 'auto' },
      baseZIndex: 10000,
      data: { id: id },
      closable: true,
    });

    // Handle dialog closure
    if (this.ref) {
      this.ref.onClose.subscribe((data: RoleDto) => {
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
    let ids: any = [];
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
    this.roleService.deleteRoles(ids).subscribe({
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
}
