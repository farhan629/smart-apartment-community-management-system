import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffStatCards } from './staff-stat-cards';

describe('StaffStatCards', () => {
  let component: StaffStatCards;
  let fixture: ComponentFixture<StaffStatCards>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffStatCards],
    }).compileComponents();

    fixture = TestBed.createComponent(StaffStatCards);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
