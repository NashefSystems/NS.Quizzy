import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppNotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ExamTypesService } from '../../../services/backend/exam-types.service';
import { IExamTypePayloadDto } from '../../../models/backend/exam-type.dto';

@Component({
  selector: 'app-exam-type-add-or-edit',
  standalone: false,
  templateUrl: './exam-type-add-or-edit.component.html',
  styleUrl: './exam-type-add-or-edit.component.scss'
})
export class ExamTypeAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _examTypesService = inject(ExamTypesService);
  private readonly _notificationsService = inject(AppNotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;

  form: FormGroup = this._fb.group({
    name: ['', [Validators.required]],
    itemOrder: ['', [Validators.required]],
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

    this._examTypesService.getById(id).subscribe({
      next: data => {
        const { name, itemOrder } = data;
        const newValue = {
          ...this.form.value,
          name, itemOrder
        };
        this.form.setValue(newValue);
      }
    });
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { name, itemOrder } = this.form.value;

    const payload: IExamTypePayloadDto = {
      name: name,
      itemOrder: itemOrder
    };

    if (this.id) {
      this._examTypesService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._examTypesService.insert(payload).subscribe({
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
    this._router.navigate(['/exam-types']);
  }
}
