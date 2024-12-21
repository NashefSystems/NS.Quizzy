import { Component, inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FilterDialogData, FilterDialogResult } from './filter-dialog-data';
import { ClassDto } from '../../../../models/backend-models/class.dto';
import { ICheckboxTreeItem } from '../../checkbox-tree/checkbox-tree-item';

@Component({
  selector: 'app-filter-dialog',
  templateUrl: './filter-dialog.component.html',
  styleUrl: './filter-dialog.component.scss'
})
export class FilterDialogComponent implements OnInit {
  private readonly dialogRef = inject(MatDialogRef<FilterDialogComponent>);
  readonly data = inject<FilterDialogData>(MAT_DIALOG_DATA);

  classTree: ICheckboxTreeItem[] = [];
  subjectTree: ICheckboxTreeItem[] = [];
  readonly result: FilterDialogResult = { ...this.data.oldResult };

  ngOnInit(): void {
    console.log("dialog data", { data: this.data, result: this.result })
    this.classTree = this.data.classes.map(c => this.getCheckboxTreeItem(c));
    this.subjectTree = this.data.subjects.map(c => ({ id: c.id, text: c.name, children: [] }));
  }

  getCheckboxTreeItem(item: ClassDto): ICheckboxTreeItem {
    return {
      id: item.id,
      text: item.name,
      children: item?.children?.map(c => this.getCheckboxTreeItem(c)) ?? []
    };
  }

  onCloseClick(): void {
    this.dialogRef.close(null);
  }
}
