import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { BoardService } from '../../services/board.service';
import { BoardSummary } from '../../models/board-summary.model';

@Component({
  selector: 'app-board-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './board-list.component.html'
})
export class BoardListComponent {

  private readonly boardService = inject(BoardService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  boards: BoardSummary[] = [];
  loading = true;

  projectId!: string;

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.loadBoards();
  }

  loadBoards(): void {
    this.loading = true;

    this.boardService.getBoardsByProject(this.projectId)
      .subscribe({
        next: (data) => {
          this.boards = data;
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }

  openBoard(boardId: string): void {
    this.router.navigate([
      '/boards',
      boardId
    ]);
  }
}