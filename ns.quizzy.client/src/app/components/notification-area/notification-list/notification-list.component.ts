import { Component, inject, OnInit } from '@angular/core';
import { NotificationsService } from '../../../services/backend/notifications.service';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { INotificationDto } from '../../../models/backend/notification.dto';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { AppTranslateService } from '../../../services/app-translate.service';
import { CamelToSnakePipe } from '../../../pipes/camel-to-snake.pipe';
import { DatePipe } from '@angular/common';

export interface INotificationDtoEx extends INotificationDto {
  text: string;
}

@Component({
  selector: 'app-notification-list',
  standalone: false,
  templateUrl: './notification-list.component.html',
  styleUrl: './notification-list.component.scss'
})
export class NotificationListComponent implements OnInit {
  private readonly _camelToSnakePipe = inject(CamelToSnakePipe);
  private readonly _datePipe = inject(DatePipe);
  private readonly _appTranslateService = inject(AppTranslateService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  searchValue: string = '';
  _items: INotificationDtoEx[];
  filteredItems: INotificationDtoEx[];

  ngOnInit(): void {
    this.loadData();
  }

  applyFilter(): void {
    this.filteredItems = [... this._items.filter(e => !this.isFilteredItem(e, this.searchValue)).map(x => x)];
  }

  isFilteredItem(item: INotificationDtoEx, filterValue: string): boolean {
    filterValue = filterValue.trim();
    return item.text.indexOf(filterValue) === -1;
  }

  getSearchTextValue(item: INotificationDto) {
    const types = item.targets.map(x => {
      const camelToSnakeValue = this._camelToSnakePipe.transform(x.type);
      return this._appTranslateService.translate(`NOTIFICATION_TYPES.${camelToSnakeValue}`);
    });

    return `
      ${item.title}
      ${item.body}
      ${this._datePipe.transform(item.createdTime, 'yyyy/MM/dd HH:mm')}
      ${types.join('\r\n')}
    `;
  }

  loadData() {
    this._notificationsService.get().subscribe({
      next: (responseBody) => {
        this._items = responseBody.map(x => {
          const res: INotificationDtoEx = {
            ...x,
            text: this.getSearchTextValue(x)
          };
          return res;
        });
        this.applyFilter();
      }
    });
  }

  onAdd() {
    this._router.navigate(['/notifications/new']);
  };

  onDuplicate(selected: INotificationDto) {
    this._router.navigate(
      ['/notifications/new'],
      {
        //queryParams:
        state: {
          DuplicateItemId: selected.id
        }
      });
  }

  onDelete(selected: INotificationDto) {
    const item = this.filteredItems.find(x => x.id == selected.id);
    if (!item) {
      return;
    }

    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: item.title
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._notificationsService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
