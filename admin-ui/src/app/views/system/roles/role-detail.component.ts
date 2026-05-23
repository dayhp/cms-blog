import { ChangeDetectorRef, Component, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { pipe, Subject, takeUntil, timeout, finalize } from "rxjs";
import { AdminApiRoleApiClient, RoleDto } from "../../../api/admin-api.service.generated";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { UtilityService } from "../../../shared/services/utility.service";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { BlockUIModule } from "primeng/blockui";
import { PanelModule } from "primeng/panel";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { ValidationMessageComponent } from "../../../shared/modules/validate/validation-message.component";
import { KeyFilterModule } from "primeng/keyfilter";
import { CommonSharedModule } from "../../../shared/modules/common-shared.module";
import { PermissionDirective } from "../../../shared/modules/permission/permission.directive";

@Component({
    standalone: true,
    selector: 'app-role-detail',
    imports: [
        ProgressSpinnerModule,
        FormsModule,
        BlockUIModule,
        PanelModule,
        ButtonModule,
        InputTextModule,
        KeyFilterModule,
        ReactiveFormsModule,
        ValidationMessageComponent,
        // CommonSharedModule,
        // PermissionDirective
    ],
    templateUrl: './role-detail.component.html',
    styleUrl: './role-detail.component.scss',
})
export class RoleDetailComponent implements OnInit, OnDestroy {
    private ngSubscribe: Subject<void> = new Subject<void>();

    // Default
    public blockedPanelDetail: boolean = false;
    public form!: FormGroup;
    public title: string = 'Create Role';
    public btnDisabled: boolean = false;
    public saveBtnName: string = 'Create';
    public closeBtnName: string = 'Close';
    selectEntity = {} as RoleDto;
    constructor(
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private roleService: AdminApiRoleApiClient,
        private fb: FormBuilder,
        private utilService: UtilityService,
        private cd: ChangeDetectorRef
    ) { }
    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngSubscribe.next();
        this.ngSubscribe.complete();
    }

    ngOnInit(): void {
        this.buildForm();
        if (this.utilService.isEmpty(this.config.data?.id) == false) {
            console.log('ID provided, loading role details for ID:', this.config.data.id);
            this.loadDetail(this.config.data.id);
            this.saveBtnName = 'Cập nhật';
            this.closeBtnName = 'Hủy';
        } else {
            this.saveBtnName = 'Thêm';
            this.closeBtnName = 'Đóng';
        }
        this.cd.detectChanges();
        this.blockedPanelDetail = false;
    }

    // Validate form
    noSpecial: RegExp = /^[a-zA-Z0-9\s]*$/;
    validationMessages = {
        name: [
            { type: 'required', message: 'Bạn phải nhập tên quyền' },
            { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
        ],
        displayName: [{ type: 'required', message: 'Bạn phải nhập tên hiển thị' }],
    };
    //

    loadDetail(id: string) {
        console.log('Loading role details for ID:', id);
        this.blockedPanelDetail = true;
        this.roleService.getRoleById(id as any)
            .pipe(
                timeout(5000),
                takeUntil(this.ngSubscribe),
                // finalize(() => {
                //     this.blockedPanelDetail = false;
                // })
            )
            .subscribe({
                next: (res: RoleDto) => {
                    this.selectEntity = res;
                    this.buildForm();
                },
                error: (e) => {
                    console.error('Error loading role:', e);
                },
            });
    }

    saveChange() {
        this.blockedPanelDetail = true;
        this.btnDisabled = true;

        if (this.form && this.utilService.isEmpty(this.config.data?.id)) {
            this.roleService.createRole(this.form.value)
                .pipe(
                    timeout(5000),
                    takeUntil(this.ngSubscribe),
                    finalize(() => {
                        this.blockedPanelDetail = false;
                        this.btnDisabled = false;
                        this.ref.close(true);
                    })
                )
                .subscribe({
                    next: (res) => {
                        this.ref.close(true);
                    },
                    error: (e) => {
                        console.error('Error creating role:', e);
                    },
                });

        } else {
            this.roleService.updateRole(this.config.data?.id, this.form.value)
                .pipe(
                    timeout(5000),
                    takeUntil(this.ngSubscribe),
                    finalize(() => {
                        this.blockedPanelDetail = false;
                        this.btnDisabled = false;
                        this.ref.close(true);
                    })
                )
                .subscribe({
                    next: (res) => {
                        this.ref.close(true);
                    },
                    error: (e) => {
                        console.error('Error updating role:', e);
                    },
                });
        }
    }

    closeDialog() {
        this.ref.close();
    }


    buildForm() {
        this.blockedPanelDetail = true;
        this.form = this.fb.group({
            name: new FormControl(
                this.selectEntity?.name ?? null,
                Validators.compose([
                    Validators.required,
                    Validators.maxLength(255),
                    Validators.minLength(3),
                ])
            ),
            displayName: new FormControl(
                this.selectEntity?.displayName ?? null,
                Validators.required
            ),
        });
    }
}