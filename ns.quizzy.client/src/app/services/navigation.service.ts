import { Injectable } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { filter } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class NavigationService {
    private previousUrl: string | null = null;
    private currentUrl: string | null = null;

    constructor(private router: Router) {
        this.router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .subscribe((event: NavigationEnd) => {
                this.previousUrl = this.currentUrl;
                this.currentUrl = event.url;
            });
    }

    public getPreviousUrl(): string | null {
        return this.previousUrl;
    }

    public navigateBack(fallbackUrl: string = '/'): void {
        if (this.previousUrl) {
            this.router.navigateByUrl(this.previousUrl);
        } else {
            this.router.navigate([fallbackUrl]);
        }
    }
}