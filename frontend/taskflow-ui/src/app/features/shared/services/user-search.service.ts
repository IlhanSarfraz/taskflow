import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserSummary } from '../../projects/models/user-summary.model';
import { ApiService } from '../../../core/services/api.service';

@Injectable({
  providedIn: 'root'
})
export class UserSearchService {
  private api = inject(ApiService);

  search(term: string): Observable<UserSummary[]> {
    return this.api.get<UserSummary[]>(`users/search?search=${encodeURIComponent(term)}`);
  }
}