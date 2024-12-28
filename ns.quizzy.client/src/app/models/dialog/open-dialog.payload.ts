import { ComponentType } from '@angular/cdk/portal';

export class OpenDialogPayload {
    component: ComponentType<any>;
    data?: any;
    disableClose?: boolean;
    isFullDialog?: boolean;
}