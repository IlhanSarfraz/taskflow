import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { BoardService } from '../../services/board.service';
import { BoardDetails } from '../../models/board-details.model';

import { FormsModule } from '@angular/forms';
import { TaskSummary } from '../../models/task-summary.model';
import { DEFAULT_COLUMN_NAMES } from '../../../../constants/board.constants';

import {
  DragDropModule,
  CdkDragDrop,
  moveItemInArray,
  transferArrayItem
} from '@angular/cdk/drag-drop';

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
export class BoardDetailsComponent implements OnInit {

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

  // ─── Computed ───────────────────────────────────────────────────────────────

  get totalTaskCount(): number {
    return this.board?.column.reduce((sum, col) => sum + col.tasks.length, 0) ?? 0;
  }

  get columnCount(): number {
    return this.board?.column.length ?? 0;
  }

  // ─── Default column guard ────────────────────────────────────────────────────

  isDefaultColumn(name: string): boolean {
    return DEFAULT_COLUMN_NAMES.includes(name as any);
  }

  // ─── Priority helpers ────────────────────────────────────────────────────────

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

  // ─── Task selection ──────────────────────────────────────────────────────────

  selectTask(task: TaskSummary): void {
    this.selectedTaskId = task.id;
    this.openTask(task.id);
  }

  // ─── Column menu ─────────────────────────────────────────────────────────────

  toggleColumnMenu(columnId: string, event: Event): void {
    event.stopPropagation();
    this.columnMenuOpenId = this.columnMenuOpenId === columnId ? null : columnId;
  }

  closeColumnMenu(): void {
    this.columnMenuOpenId = null;
  }

  // ─── Lifecycle ───────────────────────────────────────────────────────────────

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

  // ─── Task drag & drop ────────────────────────────────────────────────────────

  onDrop(event: CdkDragDrop<any[]>): void {
    const task = event.item.data;
    if (!task) return;

    const targetColumnId = event.container.id;

    // Same column → reorder
    if (event.previousContainer === event.container) {
      moveItemInArray(
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      const orderedTaskIds = event.container.data.map((t: any) => t.id);

      this.boardService.reorderTasks(targetColumnId, orderedTaskIds).subscribe({
        next: () => {},
        error: (err) => {
          console.error(err);
          this.loadBoard(this.board!.id);
        }
      });

      return;
    }

    // Cross-column → move then reorder target
    transferArrayItem(
      event.previousContainer.data,
      event.container.data,
      event.previousIndex,
      event.currentIndex
    );

    this.boardService.moveTask(task.id, targetColumnId).subscribe({
      next: () => {
        const orderedTaskIds = event.container.data.map((t: any) => t.id);

        this.boardService.reorderTasks(targetColumnId, orderedTaskIds).subscribe({
          next: () => {},
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

  // ─── Column drag & drop ──────────────────────────────────────────────────────

  dropColumn(event: CdkDragDrop<any[]>): void {
    if (!this.board) return;

    moveItemInArray(
      this.board.column,
      event.previousIndex,
      event.currentIndex
    );

    const orderedColumnIds = this.board.column.map(x => x.id);

    this.boardService.reorderColumns(this.board.id, {
      boardId: this.board.id,
      orderedColumnIds
    }).subscribe({
      error: (err) => console.error(err)
    });
  }

  // ─── Navigation ──────────────────────────────────────────────────────────────

  openCreateTask(columnId: string): void {
    this.router.navigate([
      '/boards', this.board?.id,
      'projects', this.board?.projectId,
      'columns', columnId,
      'tasks', 'create'
    ]);
  }

  openTask(taskId: string): void {
    this.router.navigate(['/tasks', taskId]);
  }

  // ─── Column CRUD ─────────────────────────────────────────────────────────────

  createColumn(): void {
    if (!this.board || !this.newColumnName.trim()) return;

    const nextOrder = this.board.column.length;

    this.boardService.createColumn(this.board.id, {
      boardId: this.board.id,
      name: this.newColumnName,
      order: nextOrder
    }).subscribe({
      next: () => {
        this.newColumnName = '';
        this.showCreateColumn = false;
        this.loadBoard(this.board!.id);
      },
      error: (err) => console.error(err)
    });
  }

  startRenameColumn(columnId: string, currentName: string): void {
    if (this.isDefaultColumn(currentName)) return; // guard
    this.editingColumnId = columnId;
    this.editedColumnName = currentName;
  }

  saveColumnName(columnId: string): void {
    if (!this.editedColumnName.trim()) return;

    this.boardService.renameColumn(columnId, {
      columnId,
      name: this.editedColumnName
    }).subscribe({
      next: () => {
        this.editingColumnId = null;
        this.loadBoard(this.board!.id);
      },
      error: (err) => console.error(err)
    });
  }

  deleteColumn(columnId: string): void {
    if (!confirm('Delete this column?')) return;

    this.boardService.deleteColumn(columnId).subscribe({
      next: () => this.loadBoard(this.board!.id),
      error: (err) => console.error(err)
    });
  }

  markAsDoneColumn(columnId: string): void {
    if (!this.board) return;

    this.boardService
      .setDoneColumn(this.board.id, columnId)
      .subscribe({
        next: () => {
          this.board!.column.forEach(c => {
            c.isDoneColumn = c.id === columnId;
          });

          this.cdr.markForCheck();
        },
        error: err => console.error(err)
      });
  }
}