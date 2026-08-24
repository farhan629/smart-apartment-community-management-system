import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LuminaLanding } from './lumina-landing';

describe('LuminaLanding', () => {
  let component: LuminaLanding;
  let fixture: ComponentFixture<LuminaLanding>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LuminaLanding],
    }).compileComponents();

    fixture = TestBed.createComponent(LuminaLanding);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
