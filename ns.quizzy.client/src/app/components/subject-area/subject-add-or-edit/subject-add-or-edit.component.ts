import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SubjectsService } from '../../../services/backend/subjects.service';
import { ISubjectPayloadDto } from '../../../models/backend/subject.dto';
import { AppNotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-subject-add-or-edit',
  standalone: false,
  templateUrl: './subject-add-or-edit.component.html',
  styleUrl: './subject-add-or-edit.component.scss'
})
export class SubjectAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _subjectsService = inject(SubjectsService);
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

    this._subjectsService.getById(id).subscribe({
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

    const payload: ISubjectPayloadDto = {
      name: name,
      itemOrder: itemOrder
    };

    if (this.id) {
      this._subjectsService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._subjectsService.insert(payload).subscribe({
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
    this._router.navigate(['/subjects']);
  }
}
