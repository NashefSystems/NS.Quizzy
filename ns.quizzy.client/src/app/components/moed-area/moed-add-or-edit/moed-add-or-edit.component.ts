import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MoedsService } from '../../../services/backend/moeds.service';
import { IMoedPayloadDto } from '../../../models/backend/moed.dto';

@Component({
  selector: 'app-moed-add-or-edit',
  standalone: false,
  templateUrl: './moed-add-or-edit.component.html',
  styleUrl: './moed-add-or-edit.component.scss'
})
export class MoedAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _moedsService = inject(MoedsService);
  private readonly _notificationsService = inject(NotificationsService);
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

    this._moedsService.getById(id).subscribe({
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

    const payload: IMoedPayloadDto = {
      name: name,
      itemOrder: itemOrder
    };

    if (this.id) {
      this._moedsService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._moedsService.insert(payload).subscribe({
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
    this._router.navigate(['/moeds']);
  }
}
