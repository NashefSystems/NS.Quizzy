import { Component, inject, OnInit } from '@angular/core';
import { GlobalService } from '../../../services/global.service';
import { IUpdateAppCheckResponse } from '../../../models/check-update-is-nedded.response';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';
import { IOpenURLPayload } from '../../../models/webview-bridge.models';
import { AppNotificationsService } from '../../../services/notifications.service';

@Component({
  selector: 'app-force-update',
  standalone: false,

  templateUrl: './force-update.component.html',
  styleUrl: './force-update.component.scss'
})
export class ForceUpdateComponent implements OnInit {
  private readonly _globalService = inject(GlobalService);
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  updateAppCheckResponse: IUpdateAppCheckResponse | null = null;

  ngOnInit() {
    this._globalService.updateAppCheck().then(updateAppCheckResponse => this.updateAppCheckResponse = updateAppCheckResponse);
  }

  async openStore() {
    if (this.updateAppCheckResponse?.storeURL) {
      const payload: IOpenURLPayload = {
        URL: this.updateAppCheckResponse.storeURL
      };
      try {
        await this._webviewBridgeService.openURLAsync(payload);
      } catch (err) {
        console.error("ForceUpdateComponent exception: ", err);
      }
    }
  }
}
