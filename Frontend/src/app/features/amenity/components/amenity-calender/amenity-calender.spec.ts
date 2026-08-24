import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityCalender } from './amenity-calender';

describe('AmenityCalender', () => {
  let component: AmenityCalender;
  let fixture: ComponentFixture<AmenityCalender>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityCalender],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityCalender);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
