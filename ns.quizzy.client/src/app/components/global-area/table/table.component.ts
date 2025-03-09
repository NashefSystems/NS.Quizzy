import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumnInfo } from './table-column-info';

enum DataStatus {
  Loading,
  Empty,
  List
}
@Component({
  selector: 'app-table',
  standalone: false,
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent {
  DataStatus = DataStatus;
  @Input() emptyStatusFeatureEnabled: boolean = true;
  @Input() items: any[] | null = null;
  @Input() columns: TableColumnInfo[] = [];
  @Output() add = new EventEmitter<void>();
  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();

  displayedColumns: string[] = [];
  dataSource: MatTableDataSource<any>;

  constructor() {
    this.dataSource = new MatTableDataSource();
  }

  ngOnChanges(): void {
    this.displayedColumns = this.columns.map(x => x.key);
    const columnInfos: { [key: string]: TableColumnInfo } = Object.fromEntries(this.columns.filter(x => !!x.converter).map(x => [x.key, x]));
    this.dataSource.data = this.items?.map(x => {
      let res = { ...x };
      Object.values(columnInfos).forEach(cInfo => {
        if (cInfo.key in res && cInfo.converter) {
          res[cInfo.key] = cInfo.converter(x); // Apply the converter
        }
      });
      return res;
    }) ?? [];
  }

  getColumnInfo(key: string) {
    return this.columns.find(x => x.key === key);
  }

  addNewItem(): void {
    this.add.emit();
  }

  editItem(element: any): void {
    this.edit.emit(element);
  }

  deleteItem(element: any): void {
    this.delete.emit(element);
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value.trim().toLowerCase();
    this.dataSource.filter = filterValue;
  }

  getDataStatus(): DataStatus {
    if (this.items === null || this.items === undefined) {
      return DataStatus.Loading;
    }
    
    const isEmptyStatus = this.dataSource.filteredData.length === 0;
    if (this.emptyStatusFeatureEnabled && isEmptyStatus) {
      return DataStatus.Empty;
    }

    return DataStatus.List;
  }

  get displayedColumnsWithActions(): string[] {
    return [...this.displayedColumns, 'actions']; // Include 'actions' at the beginning
  }
}