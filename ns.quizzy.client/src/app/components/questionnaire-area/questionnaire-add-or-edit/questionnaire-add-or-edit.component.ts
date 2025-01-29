import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { QuestionnairesService } from '../../../services/backend/questionnaires.service';
import { NotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { IQuestionnairePayloadDto } from '../../../models/backend/questionnaire.dto';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { SubjectsService } from '../../../services/backend/subjects.service';

@Component({
  selector: 'app-questionnaire-add-or-edit',
  standalone: false,
  templateUrl: './questionnaire-add-or-edit.component.html',
  styleUrl: './questionnaire-add-or-edit.component.scss'
})
export class QuestionnaireAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _questionnairesService = inject(QuestionnairesService);
  private readonly _subjectsService = inject(SubjectsService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;
  subjects: ISubjectDto[] = [];

  form: FormGroup = this._fb.group({
    code: ['', [Validators.required, Validators.min(1)]],
    name: ['', [Validators.required]],
    subjectId: ['', [Validators.required]],
    duration: ['', [Validators.required]],
    durationWithExtra: ['', [Validators.required]],
  });

  ngOnInit(): void {
    this._activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromId(this.id);
    });
  }

  loadDataFromId(id: string | null) {
    this._subjectsService.get().subscribe({
      next: (responseBody) => {
        this.subjects = responseBody;
        if (id) {
          this._questionnairesService.getById(id).subscribe({
            next: data => {
              const { code, name, subjectId, duration, durationWithExtra } = data;
              const newValue = {
                ...this.form.value,
                code, name, subjectId, duration, durationWithExtra
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
    const { code, name, subjectId, duration, durationWithExtra } = this.form.value;
    const payload: IQuestionnairePayloadDto = {
      code,
      name,
      subjectId,
      duration,
      durationWithExtra,
    };

    if (this.id) {
      this._questionnairesService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._questionnairesService.insert(payload).subscribe({
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
    this._router.navigate(['/questionnaires']);
  }
}
