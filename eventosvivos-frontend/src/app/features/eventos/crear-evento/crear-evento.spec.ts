import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CrearEvento } from './crear-evento';

describe('CrearEvento', () => {
  let component: CrearEvento;
  let fixture: ComponentFixture<CrearEvento>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CrearEvento],
    }).compileComponents();

    fixture = TestBed.createComponent(CrearEvento);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
