import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../services/backend/account.service';
import { MatDialogRef } from '@angular/material/dialog';
import { IMenuItemInfo } from './menu-item-info';

@Component({
  selector: 'app-main-menu',
  standalone: false,

  templateUrl: './main-menu.component.html',
  styleUrl: './main-menu.component.scss'
})
export class MainMenuComponent implements OnInit {
  private readonly _accountService = inject(AccountService);
  private readonly _dialogRef = inject(MatDialogRef<MainMenuComponent>);
  menuItems: IMenuItemInfo[];

  ngOnInit(): void {
    this.menuItems = [
      { text: 'MAIN_MENU.ITEMS.EXAM_SCHEDULE', icon: 'calendar_month', routerLink: '' },
      { text: 'MAIN_MENU.ITEMS.EXAM_LIST', icon: 'rule', routerLink: 'exams' },
      { text: 'MAIN_MENU.ITEMS.QUESTIONNAIRE_LIST', icon: 'dynamic_form', routerLink: 'questionnaires' },
      { text: 'MAIN_MENU.ITEMS.GRADE_LIST', icon: 'groups', routerLink: 'grades' },
      { text: 'MAIN_MENU.ITEMS.CLASS_LIST', icon: 'school', routerLink: 'classes' },
      { text: 'MAIN_MENU.ITEMS.EXAM_TYPE_LIST', icon: 'format_size', routerLink: 'exam-types' },
      { text: 'MAIN_MENU.ITEMS.SUBJECT_LIST', icon: 'category', routerLink: 'subjects' },
    ]
    this.menuItems.push({ text: 'Developer', icon: 'code_blocks', routerLink: 'developer' });
  }

  logout() {
    this._accountService.logout().subscribe({
      next: () => this._dialogRef.close()
    });
  }
}
