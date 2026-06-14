import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { Observable } from "rxjs";

import { BoardSummary } from "../models/board-summary.model";
import { BoardDetails } from "../models/board-details.model";
import { CreateBoardRequest } from "../models/create-board-request.model";

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

  createBoard(request: CreateBoardRequest): Observable<any> {
    return this.api.post(
      'Boards',
      request
    );
  }

  moveTask(taskId: string, targetColumnId: string) {
    return this.api.put(
      `Tasks/${taskId}/move`,
      { targetColumnId }
    );
  }
}