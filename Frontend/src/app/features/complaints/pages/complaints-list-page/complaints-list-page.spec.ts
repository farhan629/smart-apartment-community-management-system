import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComplaintsListPage } from './complaints-list-page';

describe('ComplaintsListPage', () => {
  let component: ComplaintsListPage;
  let fixture: ComponentFixture<ComplaintsListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ComplaintsListPage],
    }).compileComponents();

    fixture = TestBed.createComponent(ComplaintsListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
