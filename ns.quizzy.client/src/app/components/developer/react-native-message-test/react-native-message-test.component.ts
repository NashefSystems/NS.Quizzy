import { Component, inject } from '@angular/core';
import { IResponseMessage, MESSAGE_ACTIONS } from '../../../models/webview-bridge.models';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';

@Component({
  selector: 'app-react-native-message-test',
  standalone: false,
  templateUrl: './react-native-message-test.component.html',
  styleUrl: './react-native-message-test.component.scss'
})
export class ReactNativeMessageTestComponent {
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
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
  ];

  onReactNativeMessage(action: MESSAGE_ACTIONS) {
    this.result = null;
    this.resultSource = '';
    let requestPayload = null;

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
