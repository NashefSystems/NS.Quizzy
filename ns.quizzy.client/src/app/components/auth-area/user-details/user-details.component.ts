import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../../services/backend/account.service';
import { UserDetailsDto, UserRoles } from '../../../models/backend/user-details.dto';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto } from '../../../models/backend/class.dto';

@Component({
  selector: 'app-user-details',
  standalone: false,

  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.scss'
})
export class UserDetailsComponent implements OnInit {
  private readonly _accountService = inject(AccountService);
  private readonly _classesService = inject(ClassesService);
  user: UserDetailsDto | null = null;
  classesDic: { [key: string]: IClassDto } = {};

  ngOnInit(): void {
    this._accountService.userChange.subscribe(user => this.user = user);
    this._classesService.get().subscribe(classes => {
      if (classes) {
        this.classesDic = Object.fromEntries(classes.map(x => [x.id, x]));
      }
    });
  }

  getFontColor() {
    const role = this.user?.role;
    switch (role) {
      case UserRoles.ADMIN:
        return 'darkgreen';
      case UserRoles.DEVELOPER:
        return 'darkmagenta';
      case UserRoles.SUPERADMIN:
        return 'darkred';
      case UserRoles.TEACHER:
        return 'darkblue';
      default:
      case UserRoles.STUDENT:
        return 'darkorange';
    }
  }

  getRole() {
    const role = this.user?.role;
    if (!role) {
      return '';
    }
    return `USER_ROLES.${role.toUpperCase()}`;
  }

  getClassName(classId: string) {
    const item = this.classesDic[classId];
    if (!item) {
      return null;
    }
    return `${item.name} (${item.fullCode})`;
  }
}
