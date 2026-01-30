import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from './api.service';

@Component({
  standalone: true,
  selector: 'order-page',
  imports: [CommonModule, FormsModule],
  template: `
    <h2>Place Order</h2>
    <label>Customer Name <input [(ngModel)]="customerName" name="customerName" /></label>
    <div>
      <label>Product ID <input type="number" [(ngModel)]="productId" name="productId" /></label>
      <label>Qty <input type="number" [(ngModel)]="qty" name="qty" /></label>
      <button (click)="submit()">Submit</button>
    </div>
  `
})
export class OrderPageComponent {
  customerName = '';
  productId = 1;
  qty = 1;
  constructor(private api: ApiService) {}
  submit(){
    const order = { customerName: this.customerName, items: [{ productId: this.productId, qty: this.qty }] };
    this.api.placeOrder(order).subscribe({
      next: () => { alert('Order placed'); location.hash = '#/orders'; },
      error: (err) => alert(err.error?.title || 'Failed')
    });
  }
}
