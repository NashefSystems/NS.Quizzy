export class StringUtils {
    public static toBase64(input: string): string {
        return btoa(input);
    }

    public static fromBase64(encoded: string): string {
        return atob(encoded);
    }
}