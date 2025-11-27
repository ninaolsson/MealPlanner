import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealplanWeekView } from './mealplan-week-view';

describe('MealplanWeekView', () => {
  let component: MealplanWeekView;
  let fixture: ComponentFixture<MealplanWeekView>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealplanWeekView]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealplanWeekView);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
