import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppNotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassPayloadDto } from '../../../models/backend/class.dto';
import { GradesService } from '../../../services/backend/grades.service';
import { IGradeDto } from '../../../models/backend/grade.dto';

@Component({
  selector: 'app-class-add-or-edit',
  standalone: false,
  templateUrl: './class-add-or-edit.component.html',
  styleUrl: './class-add-or-edit.component.scss'
})
export class ClassAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _gradesService = inject(GradesService);
  private readonly _classesService = inject(ClassesService);
  private readonly _notificationsService = inject(AppNotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;
  grades: IGradeDto[] = [];

  form: FormGroup = this._fb.group({
    name: ['', [Validators.required]],
    gradeId: ['', [Validators.required]],
    code: ['', [Validators.required, Validators.min(1)]],
    fullCode: [''],
  });

  onFullCodeChange() {
    const { gradeId, code } = this.form.value;
    const gradeCode = this.grades.find(x => x.id === gradeId)?.code || 0;
    const fullCode = gradeCode * 100 + code;
    this.form.setValue({ ...this.form.value, fullCode: fullCode });
  }

  ngOnInit(): void {
    this._activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromId(this.id);
    });
  }

  loadDataFromId(id: string | null) {
    this._gradesService.get().subscribe({
      next: (data) => {
        this.grades = data;
        if (id) {
          this._classesService.getById(id).subscribe({
            next: data => {
              const { name, gradeId, code, fullCode } = data;
              const newValue = {
                ...this.form.value,
                name, gradeId, code, fullCode
              };
              this.form.setValue(newValue);
            }
          });
        }
      }
    });
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { name, gradeId, code } = this.form.value;

    const payload: IClassPayloadDto = {
      name: name,
      gradeId: gradeId,
      code: code
    };

    if (this.id) {
      this._classesService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._classesService.insert(payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_ADDED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    }
  }

  navigateToList() {
    this._router.navigate(['/classes']);
  }
}
