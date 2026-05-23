import { DialogService, DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { AdminApiRoleApiClient, PermissionDto, RoleClaimDto } from "../../../api/admin-api.service.generated";
import { ConfirmationService } from "primeng/api";
import { finalize, find, Subject, takeUntil, timeout } from "rxjs";
import { ToastService } from "../../../shared/services/alert.services";
import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { BlockUIModule } from "primeng/blockui";
import { PanelModule } from "primeng/panel";
import { ButtonModule } from "@coreui/angular";
import { InputTextModule } from "primeng/inputtext";
import { KeyFilterModule } from "primeng/keyfilter";
import { CheckboxModule } from "primeng/checkbox";

@Component({
    standalone: true,
    selector: 'app-permission-grant',
    imports: [
        ProgressSpinnerModule,
        FormsModule,
        BlockUIModule,
        PanelModule,
        ButtonModule,
        InputTextModule,
        KeyFilterModule,
        ReactiveFormsModule,
        CheckboxModule
    ],
    templateUrl: './permission-grant.component.html'
})
export class PermissionGrantComponent implements OnInit, OnDestroy {

    // Default
    public ngSubscribe = new Subject<void>();
    public blockedGrantPermission: boolean = false;
    // ref: DynamicDialogRef | null | undefined;

    public form!: FormGroup;
    public title: string = 'Grant Permission';
    public btnDisabled: boolean = false;
    public saveBtnName: string = '';
    public closeBtnName: string = '';

    public permissions: RoleClaimDto[] = [];
    public selectedPermissions: RoleClaimDto[] = [];
    public id: string = '';
    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();


    constructor(
        public ref: DynamicDialogRef,
        private roleService: AdminApiRoleApiClient,
        private alertService: ToastService,
        private cd: ChangeDetectorRef,
        private fb: FormBuilder,
        private config: DynamicDialogConfig,
    ) { }

    ngOnDestroy(): void {
        this.ngSubscribe.next();
        this.ngSubscribe.complete();
    }
    ngOnInit(): void {
        this.buildForm();
        this.loadDetail(this.config.data.roleId);
        this.saveBtnName = 'Cập nhật';
        this.closeBtnName = 'Hủy';
        this.cd.detectChanges();
        this.blockedGrantPermission = false;
    }

    loadDetail(roleId: string) {
        this.blockedGrantPermission = true;
        this.roleService.getAllRolePermission(roleId)
            .pipe(
                timeout(5000),
                takeUntil(this.ngSubscribe),
            )
            .subscribe({
                next: (res: PermissionDto) => {
                    this.permissions = res.roleClaims || [];
                    this.buildForm();
                    this.blockedGrantPermission = false;
                },
                error: (e) => {
                    console.error(e);
                    this.alertService.error('Failed to load permissions');
                    this.blockedGrantPermission = false;
                },
                complete: () => {
                    this.blockedGrantPermission = false;
                },
            });
    }

    buildForm() {
        this.form = this.fb.group({});
        for (let index = 0; index < this.permissions.length; index++) {
            const permission = this.permissions[index];
            if (permission.selected) {
                this.selectedPermissions.push(new RoleClaimDto({
                    selected: true,
                    displayName: permission.displayName,
                    type: permission.type,
                    value: permission.value
                }));
            }
        }
    }

    closeDialog() {
        this.ref?.close();
    }

    saveChange() {
        this.blockedGrantPermission = true;
        const roleClaims: RoleClaimDto[] = [];
        for (let index = 0; index < this.permissions.length; index++) {
            const isGranted = this.selectedPermissions
                .some(p => p.type === this.permissions[index].type && p.value === this.permissions[index].value);
            roleClaims.push(new RoleClaimDto({
                selected: isGranted,
                displayName: this.permissions[index].displayName,
                type: this.permissions[index].type,
                value: this.permissions[index].value
            }));
        }
        const updateValues = new PermissionDto({
            roleId: this.config.data.roleId,
            roleClaims: roleClaims
        });

        this.roleService.savePermission(updateValues)
            .pipe(
                timeout(5000),
                takeUntil(this.ngSubscribe),
                finalize(() => {
                    this.blockedGrantPermission = false;
                    this.ref?.close(true);
                })
            )
            .subscribe({
                next: () => {
                    this.blockedGrantPermission = false;
                    this.ref?.close();
                },
                error: (e) => {
                    console.error('Error updating permissions:', e);
                    this.alertService.error('Failed to update permissions');
                }
            });
    }
}