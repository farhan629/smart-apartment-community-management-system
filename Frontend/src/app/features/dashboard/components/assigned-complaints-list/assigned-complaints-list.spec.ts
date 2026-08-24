import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignedComplaintsList } from './assigned-complaints-list';

describe('AssignedComplaintsList', () => {
  let component: AssignedComplaintsList;
  let fixture: ComponentFixture<AssignedComplaintsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignedComplaintsList],
    }).compileComponents();

    fixture = TestBed.createComponent(AssignedComplaintsList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
