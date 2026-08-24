import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityBookingPage } from './amenity-booking-page';

describe('AmenityBookingPage', () => {
  let component: AmenityBookingPage;
  let fixture: ComponentFixture<AmenityBookingPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityBookingPage],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityBookingPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
