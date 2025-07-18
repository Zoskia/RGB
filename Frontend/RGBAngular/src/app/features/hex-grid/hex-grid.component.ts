import { Component } from '@angular/core';
import { defineHex, Grid, rectangle } from 'honeycomb-grid';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-hex-grid',
  standalone: true,
  imports: [NgFor],
  templateUrl: './hex-grid.component.html',
  styleUrls: ['./hex-grid.component.css'],
})
export class HexGridComponent {
  hexes: any; // oder konkreter: `hexes: Array<{ corners: { x: number, y: number }[] }>;`

  constructor() {
    const Hex = defineHex({ dimensions: 30, origin: 'topLeft' });
    const grid = new Grid(Hex, rectangle({ width: 5, height: 5 }));
    this.hexes = Array.from(grid);
  }

  setCellColor(hex: any): void {
    console.log('Hex clicked:', hex);
  }

  getPolygonPoints(hex: any): string {
    return hex.corners.map((pt: any) => `${pt.x},${pt.y}`).join(' ');
  }
}
