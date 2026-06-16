import { inject, Injectable } from "@angular/core";
import { ApiService } from "../../../core/services/api.service";
import { Observable } from "rxjs";

import { BoardSummary } from "../models/board-summary.model";
import { BoardDetails } from "../models/board-details.model";
import { CreateBoardRequest } from "../models/create-board-request.model";
import { CreateColumnRequest } from "../models/create-column-request.model";
import { ReorderColumnsRequest } from "../models/reorder-columns-request.model";
import { UpdateColumnRequest } from "../models/update-column-request.model";

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
      `Tasks/taskId=${taskId}/move`,
      { targetColumnId }
    );
  }

  createColumn(
    boardId: string,
    request: CreateColumnRequest
  ) {
    return this.api.post(
      `Boards/${boardId}/columns`,
      request
    );
  }

  reorderColumns(
    boardId: string,
    request: ReorderColumnsRequest
  ) {
    return this.api.put(
      `Boards/${boardId}/columns/reorder`,
      request
    );
  }

  deleteColumn(columnId: string) {
    return this.api.delete(
      `Boards/columns/${columnId}`
    );
  }

  renameColumn(
    columnId: string,
    request: UpdateColumnRequest
  ) {
    return this.api.put(
      `Boards/columns/${columnId}`,
      request
    );
  }

}