import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateComplaintPage } from './create-complaint-page';

describe('CreateComplaintPage', () => {
  let component: CreateComplaintPage;
  let fixture: ComponentFixture<CreateComplaintPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateComplaintPage],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateComplaintPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start with an invalid, untouched form', () => {
    expect(component.form.valid).toBeFalse();
  });
});