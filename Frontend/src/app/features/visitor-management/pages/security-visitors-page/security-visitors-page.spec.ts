import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SecurityVisitorsPage } from './security-visitors-page';

describe('SecurityVisitorsPage', () => {
  let component: SecurityVisitorsPage;
  let fixture: ComponentFixture<SecurityVisitorsPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SecurityVisitorsPage],
    }).compileComponents();

    fixture = TestBed.createComponent(SecurityVisitorsPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
