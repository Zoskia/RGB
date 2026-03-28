import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

import { HexGridComponent } from './hex-grid.component';

describe('HexGridComponent', () => {
  let component: HexGridComponent;
  let fixture: ComponentFixture<HexGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HexGridComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(HexGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
