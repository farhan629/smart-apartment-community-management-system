import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityCancellationBookingPage } from './amenity-cancellation-booking-page';

describe('AmenityCancellationBookingPage', () => {
  let component: AmenityCancellationBookingPage;
  let fixture: ComponentFixture<AmenityCancellationBookingPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityCancellationBookingPage],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityCancellationBookingPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
