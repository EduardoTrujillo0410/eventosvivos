import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReporteEvento } from './reporte-evento';

describe('ReporteEvento', () => {
  let component: ReporteEvento;
  let fixture: ComponentFixture<ReporteEvento>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReporteEvento],
    }).compileComponents();

    fixture = TestBed.createComponent(ReporteEvento);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
