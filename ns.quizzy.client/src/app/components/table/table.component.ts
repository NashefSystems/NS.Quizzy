import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { TableColumnInfo } from './table-column-info';

@Component({
  selector: 'app-table',
  standalone: false,

  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent {
  @Input() emptyStatusFeatureEnabled: boolean = true;
  @Input() items: any[] = [];
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
    this.dataSource.data = this.items;
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

  showEmptyStatus() {
    if (!this.emptyStatusFeatureEnabled) {
      return false;
    }
    const isEmptyStatus = this.dataSource.filteredData.length === 0;
    return isEmptyStatus;
  }

  get displayedColumnsWithActions(): string[] {
    return [...this.displayedColumns, 'actions']; // Include 'actions' at the beginning
  }
}