import { TestBed } from '@angular/core/testing';

import { HexGridService } from './hex-grid.service';

describe('HexGridService', () => {
  let service: HexGridService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HexGridService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
