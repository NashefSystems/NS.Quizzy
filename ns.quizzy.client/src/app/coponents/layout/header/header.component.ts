import { Component, inject, Input, OnInit } from '@angular/core';
import { HeaderService } from '../../../services/header.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  private readonly headerService = inject(HeaderService);
  title: string = "";

  ngOnInit(): void {
    this.headerService.getHeaderTitle()
      .subscribe({
        next: (value) => this.title = value
      });
  }
}
