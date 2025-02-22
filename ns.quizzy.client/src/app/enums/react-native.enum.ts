export enum MESSAGE_TYPES {
    CONSOLE = 'console',
    STORE_DATA = 'storeData',
    READ_DATA = 'readData',
    CHECK_BIOMETRIC_AVAILABILITY = "checkBiometricAvailability",
    VERIFY_BIOMETRIC_SIGNATURE = "verifyBiometricSignature",
};

export enum MESSAGE_RESPONSE_EVENTS {
    ON_READ_DATA_RESPONSE = 'onReadDataResponse',
    ON_CHECK_BIOMETRIC_AVAILABILITY_RESPONSE = "onCheckBiometricAvailabilityResponse",
    ON_VERIFY_BIOMETRIC_SIGNATURE_RESPONSE = "onVerifyBiometricSignatureResponse",
};

