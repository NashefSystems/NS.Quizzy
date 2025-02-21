export enum MESSAGE_TYPES {
    CONSOLE = 'console',
    CHECK_BIOMETRIC = 'checkBiometric',
    STORE_DATA = 'storeData',
    READ_DATA = 'readData',
};

export enum MESSAGE_RESPONSE_EVENTS {
    ON_BIO_PUBLIC_KEY_RESPONSE = 'onBioPublicKeyResponse',
    ON_CHECK_BIOMETRIC_RESPONSE = 'onCheckBiometricResponse',
    ON_READ_DATA_RESPONSE = 'onReadDataResponse',
};
