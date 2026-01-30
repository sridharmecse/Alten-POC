import { bootstrapApplication } from '@angular/platform-browser';
import { Routes, provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { ProductListComponent } from './app/product-list.component';
import { ProductFormComponent } from './app/product-form.component';
import { OrderPageComponent } from './app/order-page.component';
import { OrderHistoryComponent } from './app/order-history.component';

const routes: Routes = [
  { path: '', component: ProductListComponent },
  { path: 'products/new', component: ProductFormComponent },
  { path: 'orders/new', component: OrderPageComponent },
  { path: 'orders', component: OrderHistoryComponent }
];

bootstrapApplication(AppComponent, {
  providers: [provideRouter(routes), provideHttpClient()]
});
