import { TestBed } from '@angular/core/testing';

import { Mealplan } from './mealplan';

describe('Mealplan', () => {
  let service: Mealplan;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Mealplan);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
