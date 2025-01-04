import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-table',
  standalone: false,

  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent {
  @Input() emptyStatusFeatureEnabled: boolean = true;
  @Input() items: any[] = [];
  @Input() columns: { [key: string]: string } = {};
  @Output() add = new EventEmitter<void>();
  @Output() edit = new EventEmitter<any>();
  @Output() delete = new EventEmitter<any>();

  displayedColumns: string[] = [];
  dataSource: MatTableDataSource<any>;

  constructor() {
    this.dataSource = new MatTableDataSource();
  }

  ngOnChanges(): void {
    this.displayedColumns = [...Object.keys(this.columns)];
    this.dataSource.data = this.items;
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