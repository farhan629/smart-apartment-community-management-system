import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityDelete } from './amenity-delete';

describe('AmenityDelete', () => {
  let component: AmenityDelete;
  let fixture: ComponentFixture<AmenityDelete>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityDelete],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityDelete);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
