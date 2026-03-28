import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { HexGridService } from './hex-grid.service';

describe('HexGridService', () => {
  let service: HexGridService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(HexGridService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
