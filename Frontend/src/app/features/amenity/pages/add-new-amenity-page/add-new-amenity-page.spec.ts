import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddNewAmenityPage } from './add-new-amenity-page';

describe('AddNewAmenityPage', () => {
  let component: AddNewAmenityPage;
  let fixture: ComponentFixture<AddNewAmenityPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddNewAmenityPage],
    }).compileComponents();

    fixture = TestBed.createComponent(AddNewAmenityPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
