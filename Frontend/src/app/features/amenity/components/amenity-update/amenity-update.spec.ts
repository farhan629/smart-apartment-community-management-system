import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityUpdate } from './amenity-update';

describe('AmenityUpdate', () => {
  let component: AmenityUpdate;
  let fixture: ComponentFixture<AmenityUpdate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityUpdate],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityUpdate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
