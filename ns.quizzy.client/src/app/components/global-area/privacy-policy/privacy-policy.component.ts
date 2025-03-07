import { Component, inject, OnInit } from '@angular/core';
import { ClientAppSettingsService } from '../../../services/backend/client-app-settings.service';

@Component({
  selector: 'app-privacy-policy',
  standalone: false,
  templateUrl: './privacy-policy.component.html',
  styleUrl: './privacy-policy.component.scss'
})
export class PrivacyPolicyComponent implements OnInit {
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);

  lastUpdated: string = 'מרץ 2025';
  email: string;

  ngOnInit(): void {
    this._clientAppSettingsService.get().subscribe(data => {
      this.email = data.Email;
    });
  }
}
