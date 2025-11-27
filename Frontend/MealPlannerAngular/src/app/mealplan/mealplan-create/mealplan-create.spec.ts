import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealplanCreate } from './mealplan-create';

describe('MealplanCreate', () => {
  let component: MealplanCreate;
  let fixture: ComponentFixture<MealplanCreate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealplanCreate]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealplanCreate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
