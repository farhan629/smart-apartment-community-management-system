import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityBookingHistory } from './amenity-booking-history';

describe('AmenityBookingHistory', () => {
  let component: AmenityBookingHistory;
  let fixture: ComponentFixture<AmenityBookingHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityBookingHistory],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityBookingHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
