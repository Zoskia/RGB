import { Component } from '@angular/core';

@Component({
  selector: 'app-hex-grid',
  standalone: true,
  templateUrl: './hex-grid.component.html',
  styleUrls: ['./hex-grid.component.css'],
})
export class HexGridComponent {
  hexagonPoints: string = '';

  constructor() {
    const size = 80;
    const centerX = 100;
    const centerY = 100;
    const points: string[] = [];

    for (let i = 0; i < 6; i++) {
      const angleDeg = 60 * i - 30;
      const angleRad = (Math.PI / 180) * angleDeg;
      const x = centerX + size * Math.cos(angleRad);
      const y = centerY + size * Math.sin(angleRad);
      points.push(`${x},${y}`);
    }

    this.hexagonPoints = points.join(' ');
  }
}
