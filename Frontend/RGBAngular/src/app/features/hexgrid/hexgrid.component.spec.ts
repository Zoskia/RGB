import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HexgridComponent } from './hexgrid.component';

describe('HexgridComponent', () => {
  let component: HexgridComponent;
  let fixture: ComponentFixture<HexgridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HexgridComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HexgridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
