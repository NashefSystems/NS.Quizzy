import { Injectable } from '@angular/core';
import { IRequestMessage, IResponseMessage, MESSAGE_ACTIONS } from '../models/webview-bridge.models';

@Injectable({
  providedIn: 'root'
})
export class WebviewBridgeService {
  private readonly EVENT_TYPE = 'message';

  sendMessageToNative(action: MESSAGE_ACTIONS, payload: any): Promise<IResponseMessage> {
    const requestMsg: IRequestMessage = {
      requestId: crypto.randomUUID(),
      action: action,
      payload: payload
    };

    return new Promise((resolve, reject) => {
      const _window = (window as any);
      if (!_window.ReactNativeWebView) {
        const errorResponse: IResponseMessage = {
          isException: false,
          isSuccess: false,
          action: requestMsg.action,
          requestId: requestMsg.requestId,
          data: null,
          error: `window.ReactNativeWebView is null`
        }
        reject(errorResponse);
      }

      const listener = (event: MessageEvent) => {
        try {
          const responseMsg = JSON.parse(event.data) as IResponseMessage;
          if (responseMsg?.requestId == requestMsg.requestId) {
            window.removeEventListener(this.EVENT_TYPE, listener);
            if (responseMsg.isSuccess) {
              resolve(responseMsg);
            } else {
              reject(responseMsg);
            }
          }
        } catch (err: any) {
          const exceptionResponse: IResponseMessage = {
            isException: true,
            isSuccess: false,
            action: requestMsg.action,
            requestId: requestMsg.requestId,
            data: null,
            error: err
          }
          reject(exceptionResponse);
        }
      };

      window.addEventListener(this.EVENT_TYPE, listener);
      const eventRNWebView = JSON.stringify(requestMsg);
      _window.ReactNativeWebView.postMessage(eventRNWebView);
    });
  }
}
