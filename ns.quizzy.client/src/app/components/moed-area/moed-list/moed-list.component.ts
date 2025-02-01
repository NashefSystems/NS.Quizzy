import { Component, inject, OnInit } from '@angular/core';
import { MoedsService } from '../../../services/backend/moeds.service';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { IMoedDto } from '../../../models/backend/moed.dto';
import { TableColumnInfo } from '../../global-area/table/table-column-info';

@Component({
  selector: 'app-moed-list',
  standalone: false,
  templateUrl: './moed-list.component.html',
  styleUrl: './moed-list.component.scss'
})
export class MoedListComponent implements OnInit {
  private readonly _moedsService = inject(MoedsService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);


  items: IMoedDto[];
  columns: TableColumnInfo[] = [
    {
      key: 'name',
      title: 'MOED_AREA.NAME'
    }, {
      key: 'itemOrder',
      title: 'MOED_AREA.ITEM_ORDER'
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._moedsService.get().subscribe({
      next: (responseBody) => this.items = responseBody
    });
  }

  onAdd() {
    this._router.navigate(['/moeds/new']);
  };

  onEdit(item: IMoedDto) {
    this._router.navigate([`/moeds/edit/${item?.id}`]);
  };

  onDelete(item: IMoedDto) {
    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: item.name
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._moedsService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
