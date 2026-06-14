import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardDetailsComponent } from './board-details.component';

describe('BoardDetailsComponent', () => {
  let component: BoardDetailsComponent;
  let fixture: ComponentFixture<BoardDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardDetailsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BoardDetailsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
