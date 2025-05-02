import { Component, inject, OnInit } from '@angular/core';
import { IReadDataPayload, IResponseMessage, IShowNotificationPayload, IStoreDataPayload, IVerifyBiometricSignaturePayload, IWriteToConsolePayload, MESSAGE_ACTIONS } from '../../../models/webview-bridge.models';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';

@Component({
  selector: 'app-react-native-message-test',
  standalone: false,
  templateUrl: './react-native-message-test.component.html',
  styleUrl: './react-native-message-test.component.scss'
})
export class ReactNativeMessageTestComponent implements OnInit {
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  nativeAppIsAvailable = false;
  isLoading = false;
  result: any = null;
  resultSource: string = '';
  public readonly reactNativeActions = [
    MESSAGE_ACTIONS.WRITE_TO_CONSOLE,
    MESSAGE_ACTIONS.STORE_DATA,
    MESSAGE_ACTIONS.READ_DATA,
    MESSAGE_ACTIONS.GET_BIOMETRIC_AVAILABILITY,
    MESSAGE_ACTIONS.VERIFY_BIOMETRIC_SIGNATURE,
    MESSAGE_ACTIONS.GET_NOTIFICATION_TOKEN,
    MESSAGE_ACTIONS.GET_MOBILE_SERIAL_NUMBER,
    MESSAGE_ACTIONS.GET_PLATFORM_INFO,
    MESSAGE_ACTIONS.SHOW_NOTIFICATION,
  ];

  ngOnInit(): void {
    this.nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
  }

  onReactNativeMessage(action: MESSAGE_ACTIONS) {
    this.result = null;
    this.resultSource = '';
    let requestPayload = null;

    switch (action) {
      case MESSAGE_ACTIONS.WRITE_TO_CONSOLE:
        {
          const payload: IWriteToConsolePayload = {
            level: 'warn',
            message: 'test warning message from website'
          };
          requestPayload = payload;
          break;
        }
      case MESSAGE_ACTIONS.STORE_DATA:
        {
          const payload: IStoreDataPayload = {
            key: 'website-data-key',
            value: {
              a: 10,
              b: '15'
            }
          };
          requestPayload = payload;
          break;
        }
      case MESSAGE_ACTIONS.READ_DATA:
        {
          const payload: IReadDataPayload = {
            key: 'website-data-key',
          };
          requestPayload = payload;
          break;
        }
      case MESSAGE_ACTIONS.VERIFY_BIOMETRIC_SIGNATURE:
        {
          const payload: IVerifyBiometricSignaturePayload = {
            promptMessage: 'בדיקה'
          };
          requestPayload = payload;
          break;
        }
      case MESSAGE_ACTIONS.SHOW_NOTIFICATION:
        {
          const payload: IShowNotificationPayload = {
            title: 'בדיקה',
            body: 'הודעת בדיקה',
            data: {
              isTest: "true",
              source: "developer.react-native-message-test"
            },
          };
          requestPayload = payload;
          break;
        }
    }

    this._webviewBridgeService.sendMessageToNative(action, requestPayload)
      .then(x => {
        const response = x as IResponseMessage;
        this.resultSource = `then | response is null: ${!response}`;
        this.result = response;
      })
      .catch(e => {
        const response = e as IResponseMessage;
        this.resultSource = `catch | response is null: ${!response}`;
        this.result = response;
      });
  }
}
