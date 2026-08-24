import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComplaintEscalation } from './complaint-escalation';

describe('ComplaintEscalation', () => {
  let component: ComplaintEscalation;
  let fixture: ComponentFixture<ComplaintEscalation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComplaintEscalation],
    }).compileComponents();

    fixture = TestBed.createComponent(ComplaintEscalation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
