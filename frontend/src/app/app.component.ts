import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <nav style="display:flex;gap:1rem;margin:1rem 0;">
      <a href="#/">Products</a>
      <a href="#/products/new">Create Product</a>
      <a href="#/orders/new">Place Order</a>
      <a href="#/orders">Order History</a>
    </nav>
    <router-outlet></router-outlet>
  `
})
export class AppComponent {}
