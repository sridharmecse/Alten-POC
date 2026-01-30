import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './api.service';

@Component({
  standalone: true,
  selector: 'order-history',
  imports: [CommonModule],
  template: `
    <h2>Order History</h2>
    <div *ngFor="let o of orders" style="border:1px solid #ddd;padding:8px;margin:8px 0;">
      <div><strong>#{{o.orderId}}</strong> - {{o.customerName}} - {{o.status}}</div>
      <ul>
        <li *ngFor="let i of o.items">Product {{i.productId}} × {{i.qty}}</li>
      </ul>
    </div>
  `
})
export class OrderHistoryComponent implements OnInit {
  orders:any[] = [];
  constructor(private api: ApiService) {}
  ngOnInit(){ this.api.listOrders().subscribe(os => this.orders = os); }
}
