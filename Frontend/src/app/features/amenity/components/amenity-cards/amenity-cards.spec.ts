import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityCards } from './amenity-cards';

describe('AmenityCards', () => {
  let component: AmenityCards;
  let fixture: ComponentFixture<AmenityCards>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityCards],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityCards);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
