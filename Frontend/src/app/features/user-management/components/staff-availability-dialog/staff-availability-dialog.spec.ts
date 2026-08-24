import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAvailabilityDialog } from './staff-availability-dialog';

describe('StaffAvailabilityDialog', () => {
  let component: StaffAvailabilityDialog;
  let fixture: ComponentFixture<StaffAvailabilityDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffAvailabilityDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(StaffAvailabilityDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
