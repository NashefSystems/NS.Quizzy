import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../../services/backend/account.service';
import { MatDialogRef } from '@angular/material/dialog';
import { IMenuItemInfo } from './menu-item-info';
import { Router } from '@angular/router';
import { UserDetailsDto, UserRoles } from '../../../models/backend/user-details.dto';
import { UserRolesService } from '../../../services/user-roles.service';
import { ClientAppSettingsService } from '../../../services/backend/client-app-settings.service';

@Component({
  selector: 'app-main-menu',
  standalone: false,
  templateUrl: './main-menu.component.html',
  styleUrl: './main-menu.component.scss'
})
export class MainMenuComponent implements OnInit {
  private readonly _accountService = inject(AccountService);
  private readonly _userRolesService = inject(UserRolesService);
  private readonly _router = inject(Router);
  private readonly _dialogRef = inject(MatDialogRef<MainMenuComponent>);
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);

  menuItems: IMenuItemInfo[];
  appVersion: string = "";

  ngOnInit(): void {
    this._accountService.userChange.subscribe(x => this.setMenuItems(x));
    this._clientAppSettingsService.get().subscribe({ next: result => this.appVersion = result?.AppVersion })
  }

  setMenuItems(userDetails: UserDetailsDto | null) {
    if (!userDetails) {
      this.menuItems = [];
      return;
    }

    this.menuItems = [
      { text: 'MAIN_MENU.ITEMS.EXAM_SCHEDULE', icon: 'calendar_month', routerLink: 'exam-schedule' },
    ];

    if (this._userRolesService.checkRoleRequirement(userDetails, UserRoles.ADMIN)) {
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.EXAM_LIST', icon: 'rule', routerLink: 'exams' });
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.QUESTIONNAIRE_LIST', icon: 'dynamic_form', routerLink: 'questionnaires' });
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.GRADE_LIST', icon: 'groups', routerLink: 'grades' });
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.CLASS_LIST', icon: 'school', routerLink: 'classes' });
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.EXAM_TYPE_LIST', icon: 'format_size', routerLink: 'exam-types' });
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.SUBJECT_LIST', icon: 'category', routerLink: 'subjects' });
      this.menuItems.push({ text: 'MAIN_MENU.ITEMS.MOED_LIST', icon: 'timeline', routerLink: 'moeds' });
    }

    if (this._userRolesService.checkRoleRequirement(userDetails, UserRoles.DEVELOPER)) {
      this.menuItems.push({ text: 'Developer', icon: 'code_blocks', routerLink: 'developer' });
    }
  }

  logout() {
    this._accountService.logout().subscribe({
      next: () => {
        this._dialogRef.close();
        this._router.navigate(['/']);
      }
    });
  }
}
