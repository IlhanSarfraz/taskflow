import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { BoardService } from '../../services/board.service';
import { BoardDetails } from '../../models/board-details.model';

import {
  DragDropModule,
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem
} from '@angular/cdk/drag-drop';

import { FormsModule } from '@angular/forms';
import { TaskSummary } from '../../models/task-summary.model';

@Component({
  selector: 'app-board-details',
  standalone: true,
  imports: [
    CommonModule,
    DragDropModule,
    FormsModule
  ],
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

  newColumnName = '';
  showCreateColumn = false;

  editingColumnId: string | null = null;
  editedColumnName = '';
  selectedTaskId: string | null = null;
  columnMenuOpenId: string | null = null;

  get totalTaskCount(): number {
    return this.board?.column.reduce((sum, col) => sum + col.tasks.length, 0) ?? 0;
  }

  get columnCount(): number {
    return this.board?.column.length ?? 0;
  }

  getPriorityLabel(priority: number): string {
    const labels: Record<number, string> = {
      1: 'Low',
      2: 'Medium',
      3: 'High',
      4: 'Critical'
    };
    return labels[priority] ?? 'Medium';
  }

  getPriorityClasses(priority: number): string {
    const classes: Record<number, string> = {
      1: 'bg-green-950 text-green-400',
      2: 'bg-amber-950 text-amber-400',
      3: 'bg-red-950 text-red-400',
      4: 'bg-red-950 text-red-400'
    };
    return classes[priority] ?? 'bg-amber-950 text-amber-400';
  }

  selectTask(task: TaskSummary): void {
    this.selectedTaskId = task.id;
    this.openTask(task.id);
  }

  toggleColumnMenu(columnId: string, event: Event): void {
    event.stopPropagation();
    this.columnMenuOpenId = this.columnMenuOpenId === columnId ? null : columnId;
  }

  closeColumnMenu(): void {
    this.columnMenuOpenId = null;
  }

  ngOnInit(): void {
    const boardId = this.route.snapshot.paramMap.get('boardId')!;
    this.loadBoard(boardId);
  }

  loadBoard(boardId: string): void {
    this.loading = true;

    this.boardService.getBoardById(boardId).subscribe({
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

  // =========================
  // TASK MOVE
  // =========================
  onDrop(event: CdkDragDrop<any[]>): void {

    const task = event.item.data;

    if (!task) return;

    const targetColumnId = event.container.id;

    // SAME COLUMN → REORDER TASKS
    if (event.previousContainer === event.container) {

      moveItemInArray(
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      const orderedTaskIds = event.container.data.map(t => t.id);

      this.boardService.reorderTasks(targetColumnId, orderedTaskIds)
        .subscribe({
          next: () => console.log('Tasks reordered'),
          error: (err) => {
            console.error(err);
            this.loadBoard(this.board!.id);
          }
        });

      return;
    }

    // MOVE BETWEEN COLUMNS
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );

    const sourceColumnId = event.previousContainer.id;

    this.boardService.moveTask(task.id, targetColumnId)
      .subscribe({
        next: () => {
          // After the move, also fix the order within the target column
          const orderedTaskIds = event.container.data.map((t: any) => t.id);

          this.boardService.reorderTasks(targetColumnId, orderedTaskIds)
            .subscribe({
              next: () => console.log('Task moved and reordered'),
              error: (err) => {
                console.error(err);
                this.loadBoard(this.board!.id);
              }
            });
        },
        error: (err) => {
          console.error(err);
          this.loadBoard(this.board!.id);
        }
      });
  }

  // =========================
  // COLUMN DRAG
  // =========================
  dropColumn(event: CdkDragDrop<any[]>): void {

    if (!this.board) return;

    moveItemInArray(
      this.board.column,
      event.previousIndex,
      event.currentIndex
    );

    const orderedColumnIds = this.board.column.map(x => x.id);

    this.boardService.reorderColumns(
      this.board.id,
      {
        boardId: this.board.id,
        orderedColumnIds
      }
    ).subscribe({
      error: err => console.error(err)
    });
  }

  // =========================
  // TASK NAVIGATION
  // =========================
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

  // =========================
  // COLUMN CRUD
  // =========================
  createColumn(): void {

    if (!this.board || !this.newColumnName.trim()) return;

    const nextOrder = this.board.column.length;

    this.boardService.createColumn(
      this.board.id,
      {
        boardId: this.board.id,
        name: this.newColumnName,
        order: nextOrder
      }
    ).subscribe({
      next: () => {
        this.newColumnName = '';
        this.showCreateColumn = false;
        this.loadBoard(this.board!.id);
      },
      error: err => console.error(err)
    });
  }

  startRenameColumn(columnId: string, currentName: string): void {
    this.editingColumnId = columnId;
    this.editedColumnName = currentName;
  }

  saveColumnName(columnId: string): void {

    this.boardService.renameColumn(
      columnId,
      {
        columnId,
        name: this.editedColumnName
      }
    ).subscribe({
      next: () => {
        this.editingColumnId = null;
        this.loadBoard(this.board!.id);
      },
      error: err => console.error(err)
    });
  }

  deleteColumn(columnId: string): void {

    if (!confirm('Delete this column?')) return;

    this.boardService.deleteColumn(columnId)
      .subscribe({
        next: () => this.loadBoard(this.board!.id),
        error: err => console.error(err)
      });
  }
}