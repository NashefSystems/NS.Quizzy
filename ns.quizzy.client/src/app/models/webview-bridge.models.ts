// Global 
export enum MESSAGE_ACTIONS {
    WRITE_TO_CONSOLE = 'writeToConsole',
    STORE_DATA = 'storeData',
    READ_DATA = 'readData',
    GET_BIOMETRIC_AVAILABILITY = "getBiometricAvailability",
    VERIFY_BIOMETRIC_SIGNATURE = "verifyBiometricSignature",
    GET_NOTIFICATION_TOKEN = "getNotificationToken",
    GET_MOBILE_SERIAL_NUMBER = "getMobileSerialNumber",
    GET_PLATFORM_INFO = "getPlatformInfo",
}

export interface IRequestMessage {
    action: MESSAGE_ACTIONS;
    requestId: string;
    payload: any;
}

export interface IResponseMessage {
    action?: MESSAGE_ACTIONS;
    requestId?: string;
    isSuccess: boolean;
    isException: boolean;
    error?: any;
    data?: any;
}

// WRITE_TO_CONSOLE
export interface IWriteToConsolePayload {
    level: 'log' | 'info' | 'warn' | 'error';
    message: string;
}

// STORE_DATA
export interface IStoreDataPayload {
    key: string;
    value: any;
}

// READ_DATA
export interface IReadDataPayload {
    key: string;
}

export interface IReadDataResponse {
    value: any
}

// GET_BIOMETRIC_AVAILABILITY
export interface IGetBiometricAvailabilityResponse {
    isAvailable: boolean;
    biometricType?: 'TouchID' | 'FaceID' | 'Biometrics';
}

// VERIFY_BIOMETRIC_SIGNATURE
export interface IVerifyBiometricSignatureResponse {
    isVerify: boolean;
    token?: string;
}

// GET_NOTIFICATION_TOKEN
export interface IGetNotificationTokenResponse {
    token?: string;
}

// GET_MOBILE_SERIAL_NUMBER
export interface IGetMobileSerialNumberResponse {
    hasPermission: boolean,
    serialNumber: string | null,
}

// GET_PLATFORM_INFO
export interface IGetPlatformInfoResponse {
    os: "ios" | "android" | "windows" | "macos" | "web",
    version: string | number,
    isTV: boolean,
    isTesting: boolean,
    isIOS: boolean,
    isAndroid: boolean,
    isWindows: boolean,
    isMacOS: boolean,
    isWeb: boolean,
}
