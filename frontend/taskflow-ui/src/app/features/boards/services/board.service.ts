import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { Observable } from "rxjs";

import { BoardSummary } from "../models/board-summary.model";
import { BoardDetails } from "../models/board-details.model";

@Injectable({
  providedIn: 'root'
})
export class BoardService {

  private readonly api = inject(ApiService);

  getBoardsByProject(projectId: string): Observable<BoardSummary[]> {
    return this.api.get<BoardSummary[]>(
      `projects/${projectId}/boards`
    );
  }

  getBoardById(boardId: string): Observable<BoardDetails> {
    return this.api.get<BoardDetails>(
      `boards/${boardId}`
    );
  }
}