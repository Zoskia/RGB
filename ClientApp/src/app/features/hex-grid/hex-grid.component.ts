import { Component, HostListener } from '@angular/core';
import { NgFor } from '@angular/common';
import { defineHex, Grid, rectangle } from 'honeycomb-grid';
import { HexGridService } from './hex-grid.service';
import { CellModel } from '../../models/cell.model';
import { ActivatedRoute } from '@angular/router';

type RenderHex = {
  corners: { x: number; y: number }[];
  q: number;
  r: number;
  fill: string;
};

@Component({
  selector: 'app-hex-grid',
  standalone: true,
  imports: [NgFor],
  templateUrl: './hex-grid.component.html',
  styleUrls: ['./hex-grid.component.css'],
})
export class HexGridComponent {
  hexes: Array<RenderHex> = [];

  offsetX = 0;
  offsetY = 0;
  private lastX = 0;
  private lastY = 0;
  private dragging = false;
  private hasDragged = false;

  private readonly defaultFill = '#CCCCCC';

  constructor(
    private hexGridService: HexGridService,
    private route: ActivatedRoute
  ) {
    const Hex = defineHex({ dimensions: 50, origin: 'topLeft' });
    const grid = new Grid(Hex, rectangle({ width: 100, height: 100 }));

    let minX = Infinity,
      minY = Infinity;
    let maxX = -Infinity,
      maxY = -Infinity;

    grid.forEach((hex) => {
      this.hexes.push({
        corners: hex.corners,
        q: hex.q,
        r: hex.r,
        fill: this.defaultFill,
      });

      hex.corners.forEach(({ x, y }) => {
        if (x < minX) minX = x;
        if (x > maxX) maxX = x;
        if (y < minY) minY = y;
        if (y > maxY) maxY = y;
      });
    });

    const gridCenterX = (minX + maxX) / 2;
    const gridCenterY = (minY + maxY) / 2;
    const viewCenterX = 800 / 2;
    const viewCenterY = 600 / 2;
    this.offsetX = viewCenterX - gridCenterX;
    this.offsetY = viewCenterY - gridCenterY;
  }

  ngOnInit(): void {
    const teamParam = this.route.snapshot.paramMap.get('team');
    const team = Number(teamParam);
    if (Number.isNaN(team)) {
      console.error('Invalid team route param:', teamParam);
      return;
    }

    this.hexGridService.getGrid(team).subscribe({
      next: (cells: CellModel[]) => {
        const cellMap = new Map<string, string>();
        for (const c of cells) {
          cellMap.set(`${c.q},${c.r}`, c.hexColor);
        }

        for (const h of this.hexes) {
          h.fill = cellMap.get(`${h.q},${h.r}`) ?? this.defaultFill;
        }
      },
      error: (err) => {
        console.error('Grid load failed:', err);
      },
    });
  }

  getPolygonPoints(hex: { corners: { x: number; y: number }[] }): string {
    return hex.corners.map((p) => `${p.x},${p.y}`).join(' ');
  }

  @HostListener('mousedown', ['$event'])
  onMouseDown(e: MouseEvent) {
    this.dragging = true;
    this.lastX = e.clientX;
    this.lastY = e.clientY;
    this.hasDragged = false;
  }

  @HostListener('document:mouseup')
  onMouseUp() {
    this.dragging = false;
  }

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(e: MouseEvent) {
    if (!this.dragging) return;
    this.offsetX += e.clientX - this.lastX;
    this.offsetY += e.clientY - this.lastY;
    this.lastX = e.clientX;
    this.lastY = e.clientY;
    this.hasDragged = true;
  }

  onClickNotDrag() {
    if (!this.hasDragged) {
      window.alert();
    }
  }
}
