import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ICheckboxTreeItem } from './checkbox-tree-item';

@Component({
  selector: 'app-checkbox-tree',
  templateUrl: './checkbox-tree.component.html',
  styleUrl: './checkbox-tree.component.scss'
})
export class CheckboxTreeComponent implements OnInit, OnChanges {
  @Input() items: ICheckboxTreeItem[] = [];
  @Input() selectedIds: string[] = [];
  @Output() selectedIdsChange = new EventEmitter<string[]>(); // Emit event
  private internalSelectedIds: Set<string> = new Set<string>();

  ngOnInit(): void {
    this.initializeSelectedIds();
  }
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedIds'] && changes['selectedIds'].currentValue) {
      this.initializeSelectedIds();
    }
  }

  initializeSelectedIds(): void {
    if (!this.selectedIds?.length) {
      return;
    }
    this.internalSelectedIds = new Set(this.selectedIds);
    this.emitSelectedIds();
  }

  toggleParentSelection(parent: ICheckboxTreeItem, isChecked: boolean): void {
    if (isChecked) {
      // Add parent id and remove all children ids
      this.internalSelectedIds.add(parent.id);
      parent.children.forEach(child => this.internalSelectedIds.delete(child.id));
    } else {
      // Remove parent id
      this.internalSelectedIds.delete(parent.id);
    }
    this.emitSelectedIds();
  }

  toggleChildSelection(parent: ICheckboxTreeItem, child: ICheckboxTreeItem, isChecked: boolean): void {
    if (isChecked) {
      // Remove parent id since a specific child is selected
      this.internalSelectedIds.delete(parent.id);
      this.internalSelectedIds.add(child.id);
    } else {
      if (this.internalSelectedIds.has(parent.id)) {
        this.internalSelectedIds.delete(parent.id);
        parent.children.filter(c => c.id !== child.id).forEach(child => this.internalSelectedIds.add(child.id));
      } else {
        // Uncheck the child id
        this.internalSelectedIds.delete(child.id);
      }
    }

    // Check if all children are selected
    const allChildrenSelected = parent.children.every(child => this.internalSelectedIds.has(child.id));

    if (allChildrenSelected) {
      // Remove all child ids and add only parent id
      parent.children.forEach(child => this.internalSelectedIds.delete(child.id));
      this.internalSelectedIds.add(parent.id);
    }

    this.emitSelectedIds();
  }

  isParentPartiallyComplete(parent: ICheckboxTreeItem): boolean {
    if (this.internalSelectedIds.has(parent.id)) {
      return false;
    }

    // Count how many children are selected
    const totalChildren = parent.children.length;
    const selectedChildrenCount = parent.children.filter(child => this.internalSelectedIds.has(child.id)).length;

    // Check if at least one child is selected, but not all of them
    return selectedChildrenCount > 0 && selectedChildrenCount < totalChildren;
  }

  isParentChecked(parent: ICheckboxTreeItem): boolean {
    return this.internalSelectedIds.has(parent.id);
  }

  isChildChecked(child: ICheckboxTreeItem, parentId: string): boolean {
    return this.internalSelectedIds.has(child.id) || this.internalSelectedIds.has(parentId);
  }

  getSelectedIds(): string[] {
    return Array.from(this.internalSelectedIds);
  }

  emitSelectedIds(): void {
    this.selectedIdsChange.emit(this.getSelectedIds());
  }
}