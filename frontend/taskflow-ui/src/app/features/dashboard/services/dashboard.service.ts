import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { DashboardOverview } from '../models/dashboard-overview.model';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly api = inject(ApiService);

  getOverview(): Observable<DashboardOverview> {
    return this.api.get<DashboardOverview>('dashboard/get-dashboard');
  }
}