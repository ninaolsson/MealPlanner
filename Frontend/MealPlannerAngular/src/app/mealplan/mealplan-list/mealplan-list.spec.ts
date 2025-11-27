import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MealplanList } from './mealplan-list';

describe('MealplanList', () => {
  let component: MealplanList;
  let fixture: ComponentFixture<MealplanList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MealplanList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MealplanList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
