import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignStaffDialog } from './assign-staff-dialog';

describe('AssignStaffDialog', () => {
  let component: AssignStaffDialog;
  let fixture: ComponentFixture<AssignStaffDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AssignStaffDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(AssignStaffDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
