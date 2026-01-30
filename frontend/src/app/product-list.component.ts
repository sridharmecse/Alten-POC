import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './api.service';

@Component({
  standalone: true,
  selector: 'product-list',
  imports: [CommonModule],
  template: `
    <h2>Products</h2>
    <div *ngFor="let p of products" style="border:1px solid #ddd;padding:8px;margin:8px 0;">
      <div><strong>{{p.name}}</strong> - ₹{{p.price}} (Stock: {{p.stockQty}})</div>
      <img *ngIf="p.imageUrl" [src]="p.imageUrl" style="max-height:80px;margin-top:4px;" />
    </div>
  `
})
export class ProductListComponent implements OnInit {
  products:any[] = [];
  constructor(private api: ApiService) {}
  ngOnInit(){ this.api.listProducts().subscribe(ps => this.products = ps); }
}
