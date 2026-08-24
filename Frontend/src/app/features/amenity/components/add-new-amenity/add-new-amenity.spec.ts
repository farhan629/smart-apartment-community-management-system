import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewAmenity } from './add-new-amenity';

describe('AddNewAmenity', () => {
  let component: AddNewAmenity;
  let fixture: ComponentFixture<AddNewAmenity>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddNewAmenity],
    }).compileComponents();

    fixture = TestBed.createComponent(AddNewAmenity);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
