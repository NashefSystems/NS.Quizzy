import { Component, inject } from '@angular/core';
import { AccountService } from '../../services/backend/account.service';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-main-menu',
  standalone: false,

  templateUrl: './main-menu.component.html',
  styleUrl: './main-menu.component.scss'
})
export class MainMenuComponent {
  private readonly _accountService = inject(AccountService);
  private readonly _dialogRef = inject(MatDialogRef<MainMenuComponent>);

  logout() {
    this._accountService.logout().subscribe({
      next: () => this._dialogRef.close()
    });
  }
}
