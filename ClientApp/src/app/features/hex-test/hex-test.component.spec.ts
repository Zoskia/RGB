import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HexTestComponent } from './hex-test.component';

describe('HexTestComponent', () => {
  let component: HexTestComponent;
  let fixture: ComponentFixture<HexTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HexTestComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HexTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
