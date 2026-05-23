import { ChangeDetectorRef, Component, EventEmitter, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { ButtonModule, TableModule } from "@coreui/angular";
import { BlockUIModule } from "primeng/blockui";
import { CheckboxModule } from "primeng/checkbox";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { InputTextModule } from "primeng/inputtext";
import { PaginatorModule } from "primeng/paginator";
import { PanelModule } from "primeng/panel";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { finalize, forkJoin, Subject, takeUntil, timeout } from "rxjs";
import { AdminApiRoleApiClient, AdminApiUserApiClient, RoleDto, UserDto } from "../../../api/admin-api.service.generated";
import { DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { UtilityService } from "../../../shared/services/utility.service";
import { DomSanitizer } from "@angular/platform-browser";
import { CommonSharedModule } from "../../../shared/modules/common-shared.module";
import { ImageModule } from 'primeng/image';
import { KeyFilterModule } from "primeng/keyfilter";
import { formatDate } from "@angular/common";
import { InputNumberModule } from 'primeng/inputnumber';
import { ToastService } from "../../../shared/services/alert.services";

@Component({
    selector: 'app-user-detail',
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
        ImageModule,
        CommonSharedModule,
        InputTextModule,
        KeyFilterModule,
        ReactiveFormsModule,
        InputNumberModule
    ],
    templateUrl: './user-detail.component.html'
})
export class UserDetailComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();

    // Default
    public blockedPanelDetail: boolean = false;
    public form!: FormGroup;
    public title: string = '';
    public btnDisabled = false;
    public saveBtnName: string = '';
    public roles: any[] = [];
    selectedEntity = {} as UserDto;
    public avatarImage = '';

    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(
        public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private roleService: AdminApiRoleApiClient,
        private userService: AdminApiUserApiClient,
        private utilService: UtilityService,
        private fb: FormBuilder,
        private cd: ChangeDetectorRef,
        private sanitizer: DomSanitizer,
        private alertService: ToastService
    ) {

    }
    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    // Validate
    noSpecial: RegExp = /^[^<>*!_~]+$/;
    validationMessages = {
        fullName: [{ type: 'required', message: 'Bạn phải nhập tên' }],
        email: [{ type: 'required', message: 'Bạn phải nhập email' }],
        userName: [{ type: 'required', message: 'Bạn phải nhập tài khoản' }],
        password: [
            { type: 'required', message: 'Bạn phải nhập mật khẩu' },
            {
                type: 'pattern',
                message: 'Mật khẩu ít nhất 8 ký tự, ít nhất 1 số, 1 ký tự đặc biệt, và một chữ hoa',
            },
        ],
        phoneNumber: [{ type: 'required', message: 'Bạn phải nhập số điện thoại' }],
        royaltyAmountPerPost: [{ type: 'required', message: 'Bạn phải nhập nhuận bút' }],

    };
    ngOnInit(): void {
        this.buildForm();
        var roles = this.roleService.getAll();
        this.blockedPanelDetail = true;
        forkJoin([roles])
            .pipe(takeUntil(this.ngUnsubscribe))
            .subscribe({
                next: (response: any) => {
                    var roles = response[0] as RoleDto[];
                    roles.forEach((item) => {
                        this.roles.push({ label: item.displayName, value: item.name });
                    });
                    if (this.utilService.isEmpty(this.config.data?.id) == false) {
                        this.loadDetail(this.config.data.id);
                        this.saveBtnName = 'Cập nhật';
                        this.title = 'Cập nhật người dùng';
                    } else {
                        this.setMode('create');
                        this.saveBtnName = 'Thêm';
                        this.title = 'Thêm mới người dùng';
                        this.blockedPanelDetail = false;
                    }
                    this.cd.detectChanges();
                },
                error: () => {
                    this.blockedPanelDetail = false;
                },
                complete: () => {
                    this.blockedPanelDetail = false;
                    this.cd.detectChanges();
                }
            });
        this.cd.detectChanges();
        this.blockedPanelDetail = false;
    }

    saveChange() {
        this.saveData();
    }

    loadDetail(id: string) {
        this.blockedPanelDetail = true;
        this.userService.getUserById(id).subscribe({
            next: (res: UserDto) => {
                this.selectedEntity = res;
                this.buildForm();
                this.setMode('update');
            },
            error: (e) => {
                console.error('Error loading user:', e);
            },
            complete: () => {
                this.blockedPanelDetail = false;
            }
        });
    }

    private saveData() {
        this.blockedPanelDetail = true;
        if (this.form && this.utilService.isEmpty(this.config.data?.id)) {
            this.userService.createUser(this.form.getRawValue())
                .pipe(timeout(10000), takeUntil(this.ngUnsubscribe)
                    , finalize(() => {
                        this.blockedPanelDetail = false;
                        this.btnDisabled = false;
                        this.ref.close(true);
                    }))
                .subscribe({
                    next: () => {
                        this.ref.close(this.form.value);
                        this.cd.detectChanges();
                    },
                    error: (e) => {
                        console.error('Error saving user:', e);
                    }
                });
        } else {
            this.userService.updateUser(this.config.data.id, this.form.getRawValue())
                .pipe(timeout(10000), takeUntil(this.ngUnsubscribe)
                    , finalize(() => {
                        this.blockedPanelDetail = false;
                    }))
                .subscribe({
                    next: () => {
                        this.ref.close(this.form.value);
                        this.cd.detectChanges();
                    },
                    error: (e) => {
                        console.error('Error updating user:', e);
                    }
                });
        }
    }

    closeDialog() {
        this.ref.close();
    }


    onFileChange(event: any) {
        const reader = new FileReader();

        if (event.target.files && event.target.files.length) {
            const [file] = event.target.files;
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.form.patchValue({
                    avatarFileName: file.name,
                    avatarFileContent: reader.result,
                });

                // need to run CD since file load runs outside of zone
                this.cd.markForCheck();
            };
        }
    }

    setMode(mode: string) {
        console.log('Setting mode:', mode);
        if (mode == 'update') {
            this.form.controls['userName'].clearValidators();
            this.form.controls['userName'].disable();
            this.form.controls['email'].clearValidators();
            this.form.controls['email'].disable();
            this.form.controls['password'].clearValidators();
            this.form.controls['password'].disable();
        } else if (mode == 'create') {
            this.form.controls['userName'].addValidators(Validators.required);
            this.form.controls['userName'].enable();
            this.form.controls['email'].addValidators(Validators.required);
            this.form.controls['email'].enable();
            this.form.controls['password'].addValidators(Validators.required);
            this.form.controls['password'].enable();
        }
    }

    buildForm() {
        this.blockedPanelDetail = true;
        this.form = this.fb.group({
            firstName: new FormControl(this.selectedEntity.firstName || null, Validators.required),
            lastName: new FormControl(this.selectedEntity.lastName || null, Validators.required),
            userName: new FormControl(this.selectedEntity.userName || null, Validators.required),
            email: new FormControl(this.selectedEntity.email || null, Validators.required),
            phoneNumber: new FormControl(this.selectedEntity.phoneNumber || null, Validators.required),
            password: new FormControl(
                null,
                Validators.compose([
                    Validators.required,
                    Validators.pattern(
                        '^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-zd$@$!%*?&].{8,}$'
                    ),
                ])
            ),
            dob: new FormControl(
                this.selectedEntity.dob ? formatDate(this.selectedEntity.dob, 'yyyy-MM-dd', 'en') : null
            ),
            avatarFile: new FormControl(null),
            avatar: new FormControl(this.selectedEntity.avatar || null),
            isActive: new FormControl(this.selectedEntity.isActive || true),
            royaltyAmountPerPost: new FormControl(this.selectedEntity.royaltyAmountPerPost || 0, Validators.required)
        });
    }

}