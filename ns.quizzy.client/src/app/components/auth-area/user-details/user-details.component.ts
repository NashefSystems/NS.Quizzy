import { Component, inject, OnInit } from '@angular/core';
import { AccountService } from '../../../services/backend/account.service';
import { UserDetailsDto, UserRoles } from '../../../models/backend/user-details.dto';

@Component({
  selector: 'app-user-details',
  standalone: false,

  templateUrl: './user-details.component.html',
  styleUrl: './user-details.component.scss'
})
export class UserDetailsComponent implements OnInit {
  private readonly _accountService = inject(AccountService);
  user: UserDetailsDto | null = null;


  ngOnInit(): void {
    this._accountService.userChange.subscribe(user => this.user = user);
  }
  getFontColor() {
    const role = this.user?.role;
    switch (role) {
      case UserRoles.ADMIN:
        return 'darkgreen';
      case UserRoles.DEVELOPER:
        return 'darkorange';
      case UserRoles.SUPERADMIN:
        return 'darkred';
      case UserRoles.TEACHER:
        return 'darkblue';
      default:
      case UserRoles.STUDENT:
        return 'gray';
    }
  }
  getRole() {
    const role = this.user?.role;
    if (!role) {
      return '';
    }
    return `USER_ROLES.${role.toUpperCase()}`;
  }
}
