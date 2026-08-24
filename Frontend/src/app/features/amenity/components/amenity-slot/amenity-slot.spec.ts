import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenitySlot } from './amenity-slot';

describe('AmenitySlot', () => {
  let component: AmenitySlot;
  let fixture: ComponentFixture<AmenitySlot>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenitySlot],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenitySlot);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
