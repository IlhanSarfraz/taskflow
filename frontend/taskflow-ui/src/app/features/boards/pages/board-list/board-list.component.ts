import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { BoardService } from '../../services/board.service';
import { BoardSummary } from '../../models/board-summary.model';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CreateBoardRequest } from '../../models/create-board-request.model';

@Component({
  selector: 'app-board-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './board-list.component.html'
})
export class BoardListComponent {

  private readonly boardService = inject(BoardService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);

  boards: BoardSummary[] = [];
  loading = true;

  projectId!: string;

  boardForm = this.fb.group({
    name: ['', Validators.required]
  });

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
    this.router.navigate(['/boards', boardId]);
  }

  createBoard(): void {
    if (this.boardForm.invalid) return;

    const request: CreateBoardRequest = {
      projectId: this.projectId,
      name: this.boardForm.value.name!
    };

    this.boardService.createBoard(request)
      .subscribe({
        next: () => {
          this.boardForm.reset();
          this.loadBoards(); // refresh list
        },
        error: (err) => console.error(err)
      });
  }
}