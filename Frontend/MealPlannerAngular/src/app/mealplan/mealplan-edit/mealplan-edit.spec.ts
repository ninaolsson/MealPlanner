import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealplanEdit } from './mealplan-edit';

describe('MealplanEdit', () => {
  let component: MealplanEdit;
  let fixture: ComponentFixture<MealplanEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealplanEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealplanEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
