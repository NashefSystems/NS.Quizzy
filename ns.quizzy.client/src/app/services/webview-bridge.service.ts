import { Injectable } from '@angular/core';
import { IGetBiometricAvailabilityResponse, IGetMobileSerialNumberResponse, IGetNotificationTokenResponse, IGetPlatformInfoResponse, IReadDataPayload, IRequestMessage, IResponseMessage, IShowNotificationPayload, IStoreDataPayload, IVerifyBiometricSignaturePayload, IVerifyBiometricSignatureResponse, IWriteToConsolePayload, MESSAGE_ACTIONS } from '../models/webview-bridge.models';
import { v4 as uuidv4 } from 'uuid';

@Injectable({
  providedIn: 'root'
})
export class WebviewBridgeService {

  async writeToConsoleAsync(message: string, logLevel: 'log' | 'info' | 'warn' | 'error' = 'info') {
    try {
      const payload: IWriteToConsolePayload = {
        level: logLevel,
        message: message
      };
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.WRITE_TO_CONSOLE, payload);
      return res.isSuccess;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | writeToConsoleAsync exception:", err);
      }
      return false;
    }
  }

  async storeDataAsync(key: string, value: any) {
    try {
      const payload: IStoreDataPayload = {
        key: key,
        value: value
      };
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.STORE_DATA, payload);
      return res.isSuccess;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | storeDataAsync exception:", err);
      }
      return false;
    }
  }

  async readDataAsync(key: string) {
    try {
      const payload: IReadDataPayload = {
        key: key,
      };
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.READ_DATA, payload);
      return res.data;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | readDataAsync exception:", err);
      }
      return null;
    }
  }

  async getBiometricAvailabilityAsync() {
    try {
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.GET_BIOMETRIC_AVAILABILITY);
      const resData = res.data as IGetBiometricAvailabilityResponse;
      return resData;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | getBiometricAvailabilityAsync exception:", err);
      }
      return null;
    }
  }

  async verifyBiometricSignatureAsync(promptMessage: string | null = null) {
    try {
      const payload: IVerifyBiometricSignaturePayload = {
        promptMessage: promptMessage
      };
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.VERIFY_BIOMETRIC_SIGNATURE, payload);
      const resData = res.data as IVerifyBiometricSignatureResponse;
      return resData;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | verifyBiometricSignatureAsync exception:", err);
      }
      return null;
    }
  }

  async getNotificationTokenAsync() {
    try {
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.GET_NOTIFICATION_TOKEN);
      const resData = res.data as IGetNotificationTokenResponse;
      return resData;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | getNotificationTokenAsync exception:", err);
      }
      return null;
    }
  }

  async getMobileSerialNumberAsync() {
    try {
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.GET_MOBILE_SERIAL_NUMBER);
      const resData = res.data as IGetMobileSerialNumberResponse;
      return resData;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | getMobileSerialNumberAsync exception:", err);
      }
      return null;
    }
  }

  async getPlatformInfoAsync() {
    try {
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.GET_PLATFORM_INFO);
      const resData = res.data as IGetPlatformInfoResponse;
      return resData;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | getPlatformInfoAsync exception:", err);
      }
      return null;
    }
  }

  async showNotificationAsync(payload: IShowNotificationPayload) {
    try {
      const res = await this.sendMessageToNative(MESSAGE_ACTIONS.SHOW_NOTIFICATION, payload);
      const resData = res.data as IGetPlatformInfoResponse;
      return resData;
    } catch (err: any) {
      if (err.isException) {
        console.error("WebviewBridgeService | showNotificationAsync exception:", err);
      }
      return null;
    }
  }

  private getReactNativeWebView() {
    const _window = (window as any);
    return _window.ReactNativeWebView;
  }

  nativeAppIsAvailable() {
    const reactNativeWebView = this.getReactNativeWebView();
    return !!reactNativeWebView;
  }

  private getEventType = (requestId: string) => `onAppResponse${requestId}`.replaceAll('-', '');

  sendMessageToNative(action: MESSAGE_ACTIONS, payload: any = null): Promise<IResponseMessage> {
    return new Promise((resolve, reject) => {
      let rid = '';
      try {
        rid = uuidv4();
        const eventType = this.getEventType(rid);
        const _window = window as any;
        const requestMsg: IRequestMessage = {
          requestId: rid,
          action: action,
          payload: payload
        };

        const _reactNativeWebView = this.getReactNativeWebView();
        if (!_reactNativeWebView) {
          const errorResponse: IResponseMessage = {
            isException: false,
            isSuccess: false,
            action: requestMsg.action,
            requestId: requestMsg.requestId,
            data: null,
            error: `window.ReactNativeWebView is null`
          }
          reject(errorResponse);
          return;
        }

        const listener = (responseMsg: IResponseMessage) => {
          try {
            console.log("event listener:\n[responseMsg] ", responseMsg);
            if (!responseMsg) {
              const errorResponse: IResponseMessage = {
                isException: false,
                isSuccess: false,
                action: requestMsg.action,
                requestId: requestMsg.requestId,
                data: null,
                error: `Response message is null`
              }
              reject(errorResponse);
              return;
            }
            if (responseMsg?.requestId === rid) {
              if (responseMsg.isSuccess) {
                resolve(responseMsg);
                return;
              } else {
                reject(responseMsg);
                return;
              }
            }
          } catch (err: any) {
            console.error("sendMessageToNative | Listener exception: ", err);
            const exceptionResponse: IResponseMessage = {
              isException: true,
              isSuccess: false,
              action: requestMsg.action,
              requestId: requestMsg.requestId,
              data: null,
              error: err
            }
            reject(exceptionResponse);
            return;
          }
        };
        _window[eventType] = listener;
        const eventRNWebView = JSON.stringify(requestMsg);
        _reactNativeWebView.postMessage(eventRNWebView);

      } catch (err: any) {
        console.error("sendMessageToNative | Global exception: ", err);
        const exceptionResponse: IResponseMessage = {
          isException: true,
          isSuccess: false,
          action: action,
          requestId: rid,
          data: null,
          error: err
        }
        reject(exceptionResponse);
      }
    });
  }
}
