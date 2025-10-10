export interface IUpdateAppCheckResponse {
    updateRequired: boolean;
    platform?: 'ios' | 'android';
    storeURL?: string;
}