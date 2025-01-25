import { Component, inject, OnInit } from '@angular/core';
import { DialogService } from '../../services/dialog.service';
import { FilterDialogComponent } from './filter-dialog/filter-dialog.component';
import { ClassesService } from '../../services/classes.service';
import { SubjectsService } from '../../services/backend/subjects.service';
import { IClassDto } from '../../models/backend/class.dto';
import { ISubjectDto } from '../../models/backend/subject.dto';
import { FilterDialogData, FilterDialogResult } from './filter-dialog/filter-dialog-data';
import { forkJoin } from 'rxjs';
import { OpenDialogPayload } from '../../models/dialog/open-dialog.payload';

@Component({
  selector: 'app-exams',
  standalone: false,
  templateUrl: './exams.component.html',
  styleUrl: './exams.component.scss'
})
export class ExamsComponent implements OnInit {
  private readonly dialogService = inject(DialogService);
  private readonly classesService = inject(ClassesService);
  private readonly subjectsService = inject(SubjectsService);

  classes: IClassDto[];
  subjects: ISubjectDto[];

  filterClassIds: string[] = [];
  filterSubjectIds: string[] = [];

  testValue: any;

  ngOnInit(): void {
    forkJoin({
      classes: this.classesService.getAll(),
      subjects: this.subjectsService.get()
    }).subscribe({
      next: ({ classes, subjects }) => {
        this.classes = classes;
        this.subjects = subjects;
        //  this.openFilter();
      }
    })
  }

  openFilter() {
    const dialogData: FilterDialogData = {
      classes: this.classes,
      subjects: this.subjects,
      oldResult: {
        classIds: this.filterClassIds,
        subjectIds: this.filterSubjectIds,
      }
    }
    const dialogPayload: OpenDialogPayload = {
      component: FilterDialogComponent,
      data: dialogData
    };
    this.dialogService.openDialog(dialogPayload)
      .then(result => {
        var res = result as FilterDialogResult;
        this.filterClassIds = [...res.classIds];
        this.filterSubjectIds = [...res.subjectIds];
        this.testValue = {
          time: new Date(),
          filterClassIds: this.filterClassIds,
          filterSubjectIds: this.filterSubjectIds
        }
      });
  }
}
