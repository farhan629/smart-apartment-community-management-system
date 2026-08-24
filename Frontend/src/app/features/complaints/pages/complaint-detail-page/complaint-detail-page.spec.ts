import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComplaintDetailPage } from './complaint-detail-page';

describe('ComplaintDetailPage', () => {
  let component: ComplaintDetailPage;
  let fixture: ComponentFixture<ComplaintDetailPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComplaintDetailPage],
    }).compileComponents();

    fixture = TestBed.createComponent(ComplaintDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
