import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityBooking } from './amenity-booking';

describe('AmenityBooking', () => {
  let component: AmenityBooking;
  let fixture: ComponentFixture<AmenityBooking>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityBooking],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityBooking);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
