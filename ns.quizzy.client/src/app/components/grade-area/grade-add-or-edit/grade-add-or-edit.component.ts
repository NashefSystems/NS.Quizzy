import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { GradesService } from '../../../services/backend/grades.service';
import { IGradePayloadDto } from '../../../models/backend/grade.dto';

@Component({
  selector: 'app-grade-add-or-edit',
  standalone: false,
  templateUrl: './grade-add-or-edit.component.html',
  styleUrl: './grade-add-or-edit.component.scss'
})
export class GradeAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _gradesService = inject(GradesService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;

  form: FormGroup = this._fb.group({
    name: ['', [Validators.required]],
    code: ['', [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void {
    this._activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromId(this.id);
    });
  }

  loadDataFromId(id: string | null) {
    if (!id) {
      return;
    }

    this._gradesService.getById(id).subscribe({
      next: data => {
        const { name, code } = data;
        const newValue = {
          ...this.form.value,
          name, code
        };
        this.form.setValue(newValue);
      }
    });
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { name, code } = this.form.value;

    const payload: IGradePayloadDto = {
      name: name,
      code: code
    };

    if (this.id) {
      this._gradesService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._gradesService.insert(payload).subscribe({
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
    this._router.navigate(['/grades']);
  }
}
