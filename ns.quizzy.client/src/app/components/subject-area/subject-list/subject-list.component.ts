import { Component, inject, OnInit } from '@angular/core';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { SubjectsService } from '../../../services/backend/subjects.service';

@Component({
  selector: 'app-subject-list',
  standalone: false,

  templateUrl: './subject-list.component.html',
  styleUrl: './subject-list.component.scss'
})
export class SubjectListComponent implements OnInit {
  private readonly subjectsService = inject(SubjectsService);

  items: ISubjectDto[];
  columns: { [key: string]: string } = {
    'name': 'SUBJECT_AREA.NAME',
    'itemOrder': 'SUBJECT_AREA.ITEM_ORDER'
  };

  ngOnInit(): void {
    this.subjectsService.get().subscribe({
      next: (responseBody) => this.items = responseBody
    });
  }

  onAdd() {
    alert("on add");
  };

  onEdit(item: any) {
    alert("on edit - " + JSON.stringify(item));
  };

  onDelete(item: any) {
    alert("on delete - " + JSON.stringify(item));
  };
}
