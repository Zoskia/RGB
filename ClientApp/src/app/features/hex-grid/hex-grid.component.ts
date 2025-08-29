import { Component, HostListener } from '@angular/core';
import { NgFor } from '@angular/common';
import { defineHex, Grid, rectangle } from 'honeycomb-grid';
import { HexGridService } from './hex-grid.service';
import { CellModel } from '../../models/cell.model';
import { ActivatedRoute } from '@angular/router';

type RenderHex = {
  // Keep the original corners for SVG rendering
  corners: { x: number; y: number }[];
  // Added: axial coordinates to map DB cells to hexes
  q: number;
  r: number;
  // Added: resolved fill color from DB or default
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
  // Stores each hex’s pixel corners for rendering
  hexes: Array<RenderHex> = [];

  // Current pan offsets applied to the SVG group
  offsetX = 0;
  offsetY = 0;
  private lastX = 0;
  private lastY = 0;
  private dragging = false;

  // Added: default color used when no DB entry exists for (q,r)
  private readonly defaultFill = '#CCCCCC';

  constructor(
    private hexGridService: HexGridService,
    private route: ActivatedRoute
  ) {
    // Create a hex “class” with 50px radius, pixel origin at top-left
    const Hex = defineHex({ dimensions: 50, origin: 'topLeft' });

    // Build a rectangular grid (100×100) using the built-in rectangle traverser
    // rectangle({width, height}) defines the grid’s layout in hex coordinates
    const grid = new Grid(Hex, rectangle({ width: 100, height: 100 }));

    // Compute min/max pixel extents of all hex corners
    let minX = Infinity,
      minY = Infinity;
    let maxX = -Infinity,
      maxY = -Infinity;

    grid.forEach((hex) => {
      // Honeycomb’s grid.forEach iterates each hex in the grid
      this.hexes.push({
        corners: hex.corners, // hex.corners contains 6 pixel points for SVG rendering
        q: hex.q,
        r: hex.r,
        fill: this.defaultFill,
      });

      // hex.corners contains 6 pixel points for SVG rendering
      hex.corners.forEach(({ x, y }) => {
        if (x < minX) minX = x;
        if (x > maxX) maxX = x;
        if (y < minY) minY = y;
        if (y > maxY) maxY = y;
      });
    });

    // Center grid in viewport
    const gridCenterX = (minX + maxX) / 2;
    const gridCenterY = (minY + maxY) / 2;
    const viewCenterX = 800 / 2;
    const viewCenterY = 600 / 2;
    this.offsetX = viewCenterX - gridCenterX;
    this.offsetY = viewCenterY - gridCenterY;
  }

  ngOnInit(): void {
    // Added: read ':team' from the route and load the grid for that team
    const teamParam = this.route.snapshot.paramMap.get('team');
    const team = Number(teamParam);
    if (Number.isNaN(team)) {
      console.error('Invalid team route param:', teamParam);
      return;
    }

    this.hexGridService.getGrid(team).subscribe({
      next: (cells: CellModel[]) => {
        // Added: map DB cells to generated hexes via (q,r) and set fill color
        const cellMap = new Map<string, string>(); // key: "q,r" -> hexColor
        for (const c of cells) {
          cellMap.set(`${c.q},${c.r}`, c.hexColor);
        }

        for (const h of this.hexes) {
          h.fill = cellMap.get(`${h.q},${h.r}`) ?? this.defaultFill;
        }
      },
      error: (err) => {
        console.error('Grid load failed:', err);
        // Keep default gray if request fails
      },
    });
  }

  // Return SVG-ready "points" string from pixel corners
  getPolygonPoints(hex: { corners: { x: number; y: number }[] }): string {
    return hex.corners.map((p) => `${p.x},${p.y}`).join(' ');
  }

  // MOUSE EVENTS
  @HostListener('mousedown', ['$event'])
  onMouseDown(e: MouseEvent) {
    this.dragging = true;
    this.lastX = e.clientX;
    this.lastY = e.clientY;
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
  }
}
