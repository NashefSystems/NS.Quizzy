import { Component, inject, OnInit } from '@angular/core';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { LoginComponent } from '../../login/login.component';
import { MainMenuComponent } from '../../main-menu/main-menu.component';
import { AccountService } from '../../../services/backend/account.service';
import { AppTranslateService } from '../../../services/app-translate.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, Observable } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  private readonly _dialogService = inject(DialogService);
  private readonly _accountService = inject(AccountService);
  private readonly _appTranslateService = inject(AppTranslateService);

  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);

  title: string = "";
  userLoggedIn = false;

  ngOnInit(): void {
    this.getPageTitle().subscribe({
      next: (value) => {
        if (!value) {
          return;
        }
        this.title = this._appTranslateService.translate(value);
      }
    });


    this._accountService.userChange.subscribe(user => this.userLoggedIn = !!(user?.id));
  }

  getPageTitle(): Observable<string | undefined> {
    return this._router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.getChildRoute(this._activatedRoute)),
      map(route => route.snapshot.data['page_title'])
    );
  }

  private getChildRoute(route: ActivatedRoute): ActivatedRoute {
    while (route.firstChild) {
      route = route.firstChild;
    }
    return route;
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
