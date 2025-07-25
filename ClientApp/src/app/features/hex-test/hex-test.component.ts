import { Component } from '@angular/core';
import { defineHex, Grid, rectangle } from 'honeycomb-grid';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-hex-test',
  standalone: true,
  imports: [NgFor],
  templateUrl: './hex-test.component.html',
  styleUrl: './hex-test.component.css',
})
export class HexTestComponent {
  Hex = defineHex({ dimensions: 30, origin: 'topLeft' });

  grid = new Grid(this.Hex, rectangle({ width: 5, height: 5 }));

  getPolygonPoints(hex: any): string {
    return hex.corners
      .map((corner: any) => `${corner.x},${corner.y}`)
      .join(' ');
  }

  getHexCoordinates(hex: any) {
    console.log(`Q: ${hex.q}, R: ${hex.r}`);
  }
}
