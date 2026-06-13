import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { BoardService } from '../../services/board.service';
import { BoardDetails } from '../../models/board-details.model';

import { DragDropModule, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-board-details',
  standalone: true,
  imports: [CommonModule, DragDropModule],
  templateUrl: './board-details.component.html',
  styleUrl: './board-details.component.scss',
})
export class BoardDetailsComponent {

  private readonly boardService = inject(BoardService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);

  board?: BoardDetails;
  loading = true;

  connectedColumns: string[] = [];

  ngOnInit(): void {
    const boardId = this.route.snapshot.paramMap.get('boardId')!;
    this.loadBoard(boardId);
  }

  loadBoard(boardId: string): void {
    this.loading = true;

    this.boardService.getBoardById(boardId)
      .subscribe({
        next: (data) => {
          this.board = data;
          this.loading = false;

          // IMPORTANT: connect all columns for cross-drop
          this.connectedColumns = data.column.map(c => c.id);

          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }

  onDrop(event: CdkDragDrop<any[]>, targetColumn: any): void {

    const task = event.item.data;

    if (!task) return;

    // same column reorder
    if (event.previousContainer === event.container) {
      moveItemInArray(
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );
      return;
    }

    // move between columns
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );

    // backend sync
    this.boardService.moveTask(task.id, targetColumn.id)
      .subscribe({
        next: () => console.log('Task moved'),
        error: (err) => {
          console.error(err);
          this.loadBoard(this.board!.id); // rollback safety
        }
      });
  }

  openCreateTask(columnId: string): void {
    this.router.navigate([
      '/boards',
      this.board?.id,
      'projects',
      this.board?.projectId,
      'columns',
      columnId,
      'tasks',
      'create'
    ]);
  }

  openTask(taskId: string): void {
    this.router.navigate(['/tasks', taskId]);
  }
}