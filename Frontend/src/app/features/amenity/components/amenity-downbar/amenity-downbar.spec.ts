import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityDownbar } from './amenity-downbar';

describe('AmenityDownbar', () => {
  let component: AmenityDownbar;
  let fixture: ComponentFixture<AmenityDownbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityDownbar],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityDownbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
