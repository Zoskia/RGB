import { Component, OnInit } from '@angular/core';

interface HexCell {
  id: number;
  q: number; // axiale Spalten-Koordinate
  r: number; // axiale Zeilen-Koordinate
}

@Component({
  selector: 'app-hex-grid',
  templateUrl: './hexgrid.component.html',
  styleUrls: ['./hexgrid.component.scss'],
})
export class HexGridComponent implements OnInit {
  cells: HexCell[] = [];

  readonly radius = 30; // Radius eines Hexagons in px
  readonly mapRadius = 5; // so entsteht 11 Reihen: von -5 bis +5 in axialen Koordinaten

  ngOnInit(): void {
    this.generateGrid();
  }

  private generateGrid(): void {
    let idCounter = 0;
    for (let q = -this.mapRadius; q <= this.mapRadius; q++) {
      const r1 = Math.max(-this.mapRadius, -q - this.mapRadius);
      const r2 = Math.min(this.mapRadius, -q + this.mapRadius);
      for (let r = r1; r <= r2; r++) {
        this.cells.push({ id: idCounter++, q, r });
      }
    }
  }

  // Umrechnung axial → Pixel-Koordinaten
  getCenterX(q: number, r: number): number {
    // horizontaler Abstand = √3 * radius
    return this.radius * (Math.sqrt(3) * q + (Math.sqrt(3) / 2) * r);
  }

  getCenterY(q: number, r: number): number {
    // vertikaler Abstand = 1.5 * radius
    return this.radius * ((3 / 2) * r);
  }

  getHexPoints(cx: number, cy: number): string {
    const pts: string[] = [];
    for (let i = 0; i < 6; i++) {
      const angle = (Math.PI / 180) * (60 * i - 30);
      const x = cx + this.radius * Math.cos(angle);
      const y = cy + this.radius * Math.sin(angle);
      pts.push(`${x},${y}`);
    }
    return pts.join(' ');
  }

  getViewBox(): string {
    // ViewBox weit genug, damit Panning in alle Richtungen möglich wird
    const extent = this.radius * (this.mapRadius * 3 + 2);
    return `${-extent} ${-extent} ${extent * 2} ${extent * 2}`;
  }
}
