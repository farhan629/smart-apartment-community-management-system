import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComplaintComments } from './complaint-comments';

describe('ComplaintComments', () => {
  let component: ComplaintComments;
  let fixture: ComponentFixture<ComplaintComments>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComplaintComments],
    }).compileComponents();

    fixture = TestBed.createComponent(ComplaintComments);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
