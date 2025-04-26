import { Component, inject, OnInit } from '@angular/core';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { MainMenuComponent } from '../../global-area/main-menu/main-menu.component';
import { AccountService } from '../../../services/backend/account.service';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, Observable } from 'rxjs';
import { NotificationsService } from '../../../services/backend/notifications.service';
import { AppSettingsService } from '../../../services/app-settings.service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  private readonly _dialogService = inject(DialogService);
  private readonly _accountService = inject(AccountService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _appSettingsService = inject(AppSettingsService);

  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);

  title: string = "";
  notificationsBadge: string = "";
  headerIsHide: boolean = false;
  userLoggedIn: boolean = false;

  ngOnInit(): void {
    this.getPageTitle().subscribe({
      next: (value) => {
        if (!value) {
          return;
        }
        this.title = value;
      }
    });

    this.getHeaderIsHide().subscribe({
      next: (value) => {
        const val = !!value;
        this.headerIsHide = val;
      }
    });

    this._accountService.userChange.subscribe(user => this.userLoggedIn = !!(user?.id));

    this.fetchNumberOfMyNewNotifications();
    this._appSettingsService.reCalculateUnReadNotifications$.subscribe(source => {
      console.log(`[${source}] Get unread notification quantity`);
      this.fetchNumberOfMyNewNotifications();
    });
  }

  fetchNumberOfMyNewNotifications() {
    this._notificationsService.getNumberOfMyNewNotifications().subscribe(value => this.notificationsBadge = (value && value > 0) ? value.toString() : "");
  }

  getPageTitle(): Observable<string | undefined> {
    return this._router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.getChildRoute(this._activatedRoute)),
      map(route => route.snapshot.data['page_title'])
    );
  }

  getHeaderIsHide(): Observable<boolean> {
    return this._router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.getChildRoute(this._activatedRoute)),
      map(route => route.snapshot.data['header_is_hide'] as boolean)
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
      component: MainMenuComponent,
      isFullDialog: true
    };
    this._dialogService.openDialog(dialogPayload).then(res => {
      if (!userLoggedIn && this.userLoggedIn) {
        this.onMenuClick()
      }
    });
  }
}
