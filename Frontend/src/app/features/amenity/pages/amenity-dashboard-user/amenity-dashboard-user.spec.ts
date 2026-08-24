import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AmenityDashboardUser } from './amenity-dashboard-user';

describe('AmenityDashboardUser', () => {
  let component: AmenityDashboardUser;
  let fixture: ComponentFixture<AmenityDashboardUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AmenityDashboardUser],
    }).compileComponents();

    fixture = TestBed.createComponent(AmenityDashboardUser);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
