import { Component } from '@angular/core';
import { defineHex, Grid, rectangle } from 'honeycomb-grid';

@Component({
  selector: 'app-hex-test',
  standalone: true, // wichtig für Angular 15+ Standalone Components
  templateUrl: './hex-test.component.html',
  styleUrl: './hex-test.component.css',
})
export class HexTestComponent {
  // Definiere ein Hexagon mit einem Radius von 50px
  Hex = defineHex({ dimensions: 50, origin: 'topLeft' });

  // Erstelle ein rechteckiges Raster mit 5x5 Hexagons (Breite x Höhe)
  grid = new Grid(this.Hex, rectangle({ width: 5, height: 5 }));

  // Berechne die Punkte (Ecken) eines Hexagons als SVG-Polygon-String
  getPolygonPoints(hex: any): string {
    return hex.corners
      .map((corner: any) => `${corner.x},${corner.y}`)
      .join(' ');
  }

  // Gib das Zentrum eines Hexagons für die Platzierung zurück
  getHexTransform(hex: any): string {
    return `translate(${hex.x},${hex.y})`;
  }
}
