import { Component, inject, OnInit } from '@angular/core';
import { HeaderService } from '../../../services/header.service';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { LoginComponent } from '../../login/login.component';
import { MainMenuComponent } from '../../main-menu/main-menu.component';
import { AccountService } from '../../../services/backend/account.service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  private readonly _headerService = inject(HeaderService);
  private readonly _dialogService = inject(DialogService);
  private readonly _accountService = inject(AccountService);

  title: string = "";
  userLoggedIn = false;

  ngOnInit(): void {
    this._headerService.getHeaderTitle()
      .subscribe({
        next: (value) => this.title = value
      });

    this._accountService.userChange.subscribe(user => this.userLoggedIn = !!(user?.id));
  }

  onMenuClick() {
    const userLoggedIn = this.userLoggedIn;
    const dialogPayload: OpenDialogPayload = {
      component: userLoggedIn ? MainMenuComponent : LoginComponent,
      isFullDialog: true
    };
    this._dialogService.openDialog(dialogPayload).then(res => {
      if (!userLoggedIn && this.userLoggedIn) {
        this.onMenuClick()
      }
    });
  }
}
