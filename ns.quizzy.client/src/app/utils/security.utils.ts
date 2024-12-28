import * as CryptoJS from 'crypto-js';

export class SecurityUtils {
  private static readonly secretKey = "Lt8buvZRiEJN6wxd7atUtFGtH9BzUw4v";

  public static encrypt(value: any): string {
    return CryptoJS.AES.encrypt(JSON.stringify(value), this.secretKey).toString();
  }

  public static decrypt(encryptedValue: string): any {
    try {
      const bytes = CryptoJS.AES.decrypt(encryptedValue, this.secretKey);
      const decryptedValue = bytes.toString(CryptoJS.enc.Utf8);
      return JSON.parse(decryptedValue);
    } catch (error) {
      console.error('Error decrypting value:', error);
      return null;
    }
  }
}
