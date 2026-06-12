import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { BoardService } from '../../services/board.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { BoardDetails } from '../../models/board-details.model';

@Component({
  selector: 'app-board-details',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './board-details.component.html',
  styleUrl: './board-details.component.scss',
})
export class BoardDetailsComponent {
  private readonly boardService = inject(BoardService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  board?: BoardDetails;
  loading = true;

  ngOnInit(): void{
    const boardId = this.route.snapshot.paramMap.get(`boardId`)!;
    this.loadBoard(boardId);
  }

  loadBoard(boardId: string): void {
    this.loading = true;

    this.boardService.getBoardById(boardId)
    .subscribe({
      next: (data) => {
        this.board = data;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  openCreateTask(columnId: string): void {
    this.router.navigate([
      '/boards',
      this.board?.id,
      `projects`,
      this.board?.projectId,
      'columns',
      columnId,
      'tasks',
      'create'
    ]);
  }
}
