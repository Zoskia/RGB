import { Component } from '@angular/core';
import { defineHex } from 'honeycomb-grid';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-hex-grid',
  standalone: true,
  imports: [NgFor],
  templateUrl: './hex-grid.component.html',
  styleUrls: ['./hex-grid.component.css'],
})
export class HexGridComponent {
  /**
   * Array of all hexagons to render.
   * Each entry has a `corners` array with 6 absolute pixel coordinates.
   */
  hexes: Array<{ corners: { x: number; y: number }[] }> = [];

  constructor() {
    // --------------------------------------------------
    // 1) Create a Hex factory with `defineHex`
    //
    //    • dimensions: radius in pixels (30px here)
    //    • origin: 'topLeft' means each Hex().corners is relative
    //      to the top-left corner of its bounding box
    //
    // See: https://abbekeultjes.nl/honeycomb/guide/creating-hexes.html
    // --------------------------------------------------
    const Hex = defineHex({
      dimensions: 50,
      origin: 'topLeft',
    });

    // --------------------------------------------------
    // 2) Measure one sample hex to get its true pixel width/height
    // --------------------------------------------------
    const sample = new Hex();
    const xs = sample.corners.map((pt) => pt.x);
    const ys = sample.corners.map((pt) => pt.y);

    // boxWidth/boxHeight = bounding‐box of one hex in pixels
    const boxWidth = Math.max(...xs) - Math.min(...xs);
    const boxHeight = Math.max(...ys) - Math.min(...ys);

    // In a “pointy-top” layout each row overlaps by 1/4 of boxHeight
    const rowHeight = boxHeight * 0.75;

    // --------------------------------------------------
    // 3) Grid dimensions
    // --------------------------------------------------
    const ROWS = 5; // total number of rows
    const MAX_COLS = 5; // number of cells in a “full” row

    // --------------------------------------------------
    // 4) Build each row manually:
    //
    //    • Rows 1,3,5 (r=0,2,4) get 4 cells
    //    • Rows 2,4   (r=1,3)   get 5 cells
    //    • 4-cell rows are offset horizontally by
    //      ( (MAX_COLS–4)/2 * boxWidth ) to center them
    // --------------------------------------------------
    for (let r = 0; r < ROWS; r++) {
      // determine if this is a 1-based odd row (1st, 3rd, 5th)
      const isOdd1Based = (r + 1) % 2 === 1;
      const cellsInRow = isOdd1Based ? 4 : 5;

      // horizontal pixel offset to center 4-cell rows under 5 columns
      const groupOffset = ((MAX_COLS - cellsInRow) / 2) * boxWidth;

      for (let i = 0; i < cellsInRow; i++) {
        // compute center position of this hex in pixels
        const cx = groupOffset + i * boxWidth;
        const cy = r * rowHeight;

        // get corners relative to bounding‐box top-left
        const relCorners = new Hex().corners;

        // translate to absolute pixel positions
        const absCorners = relCorners.map((pt) => ({
          x: pt.x + cx,
          y: pt.y + cy,
        }));

        this.hexes.push({ corners: absCorners });
      }
    }
  }

  /**
   * Click handler for a hex cell
   */
  setCellColor(hex: any): void {
    console.log('Hex clicked:', hex);
  }

  /**
   * Build the SVG <polygon> points attribute from hex corners
   */
  getPolygonPoints(hex: { corners: { x: number; y: number }[] }): string {
    return hex.corners.map((pt) => `${pt.x},${pt.y}`).join(' ');
  }
}
