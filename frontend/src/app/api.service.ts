import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({providedIn: 'root'})
export class ApiService {
  constructor(private http: HttpClient) {}

  // Products
  listProducts() { return this.http.get<any[]>(`/api-products/products`); }
  getProduct(id:number) { return this.http.get<any>(`/api-products/products/${id}`); }
  createProduct(p:any) { return this.http.post(`/api-products/products`, p); }
  updateProduct(id:number, p:any) { return this.http.put(`/api-products/products/${id}`, p); }
  deleteProduct(id:number) { return this.http.delete(`/api-products/products/${id}`); }
  uploadImage(id:number, file:File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post(`/api-products/products/${id}/image`, form);
  }

  // Orders
  placeOrder(o:any) { return this.http.post(`/api-orders/orders`, o); }
  listOrders() { return this.http.get<any[]>(`/api-orders/orders`); }
}
