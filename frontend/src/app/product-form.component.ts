import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from './api.service';

@Component({
  standalone: true,
  selector: 'product-form',
  imports: [CommonModule, FormsModule],
  template: `
    <h2>Create Product</h2>
    <form (ngSubmit)="save()">
      <label>Name <input [(ngModel)]="model.name" name="name" required /></label><br>
      <label>Price <input type="number" [(ngModel)]="model.price" name="price" required /></label><br>
      <label>Stock <input type="number" [(ngModel)]="model.stockQty" name="stockQty" required /></label><br>
      <label>Image <input type="file" (change)="onFile($event)" /></label><br>
      <button type="submit">Save</button>
    </form>
  `
})
export class ProductFormComponent {
  model:any = { name:'', price:0, stockQty:0 };
  file?: File;
  constructor(private api: ApiService) {}
  onFile(e:any){ this.file = e.target.files[0]; }
  save(){
    this.api.createProduct(this.model).subscribe((p:any) => {
      if(this.file){ this.api.uploadImage(p.productId, this.file!).subscribe(); }
      alert('Saved!');
      location.hash = '#/';
    });
  }
}
