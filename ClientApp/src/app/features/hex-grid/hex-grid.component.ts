import { Component, DestroyRef, HostListener } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { defineHex, Grid, rectangle } from 'honeycomb-grid';
import { HexGridService } from './hex-grid.service';
import { CellModel } from '../../models/cell.model';
import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterLinkActive,
} from '@angular/router';
import {
  isCorrectColorSpectrum,
  isValidHexColor,
  toPickerHexColor,
} from '../../shared/utils/color.utils';
import { TeamColor } from '../../enums/team-color.enum';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UpdateColorDto } from '../../dtos/update-color.dto';
import { LoginService } from '../auth/login/login.service';

// View model used by the template to render a single polygon cell.
type RenderHex = {
  corners: { x: number; y: number }[];
  q: number;
  r: number;
  fill: string;
};

@Component({
  selector: 'app-hex-grid',
  standalone: true,
  imports: [NgFor, FormsModule, NgIf, RouterLink, RouterLinkActive],
  templateUrl: './hex-grid.component.html',
  styleUrls: ['./hex-grid.component.css'],
})
export class HexGridComponent {
  // Precomputed polygons for the SVG grid.
  hexes: Array<RenderHex> = [];

  // Pan offset applied to the SVG <g> transform.
  offsetX = 0;
  offsetY = 0;

  // Drag state to support panning and to distinguish click vs drag.
  private lastX = 0;
  private lastY = 0;
  private dragStartX = 0;
  private dragStartY = 0;
  private dragging = false;
  private hasDragged = false;
  private readonly dragThresholdPx = 4;

  private readonly defaultFill = '#CCCCCC';

  // Overlay state and selected cell information.
  selectedHex: RenderHex | null = null;
  overlayOpen = false;
  selectedColor = '#cccccc';
  currentTeam: TeamColor | null = null;
  spectrumErrorOpen = false;

  // Track initial and latest picker values to preserve the last live selection on ESC.
  private colorAtOpen = this.defaultFill.toLowerCase();
  private lastLiveColor = this.defaultFill.toLowerCase();

  /**
   * Creates the in-memory hex grid once and centers it in the viewport.
   */
  constructor(
    private hexGridService: HexGridService,
    private loginService: LoginService,
    private route: ActivatedRoute,
    private router: Router,
    private destroyRef: DestroyRef,
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

  /**
   * Loads cell colors for the team selected in the route param.
   */
  ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((params) => {
        const teamParam = params.get('team');
        const team = Number(teamParam);

        if (!this.isValidTeam(team)) {
          console.error('Invalid team route param:', teamParam);
          return;
        }

        this.currentTeam = team;
        this.closeOverlay();
        this.loadGrid(team);
      });
  }

  onLogout(): void {
    this.loginService.logout();
    this.router.navigate(['/login']);
  }

  /**
   * Converts corner coordinates into the SVG points attribute format.
   */
  getPolygonPoints(hex: { corners: { x: number; y: number }[] }): string {
    return hex.corners.map((p) => `${p.x},${p.y}`).join(' ');
  }

  /**
   * Starts dragging when pressing inside the viewport.
   * Also closes the overlay when clicking outside viewport and overlay.
   */
  @HostListener('document:mousedown', ['$event'])
  onMouseDown(e: MouseEvent) {
    const target = e.target as HTMLElement | null;
    const insideViewport = !!target?.closest('.viewport');
    const insideOverlay = !!target?.closest('.color-overlay');

    if (this.overlayOpen && !insideViewport && !insideOverlay) {
      this.closeOverlay(true);
    }

    if (!insideViewport) {
      this.dragging = false;
      return;
    }

    this.dragging = true;
    this.lastX = e.clientX;
    this.lastY = e.clientY;
    this.dragStartX = e.clientX;
    this.dragStartY = e.clientY;
    this.hasDragged = false;
  }

  /**
   * Applies the latest preview color, persists it, then closes the overlay.
   */
  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (!this.overlayOpen) return;
    if (this.selectedHex && isValidHexColor(this.lastLiveColor)) {
      this.selectedHex.fill = this.lastLiveColor;
      this.selectedColor = this.lastLiveColor;
    }
    this.closeOverlay(true);
  }

  /**
   * Ends dragging when mouse button is released.
   */
  @HostListener('document:mouseup')
  onMouseUp() {
    this.dragging = false;
  }

  /**
   * Applies viewport panning while dragging, with a small dead zone threshold.
   */
  @HostListener('document:mousemove', ['$event'])
  onMouseMove(e: MouseEvent) {
    if (!this.dragging) return;

    const totalDx = e.clientX - this.dragStartX;
    const totalDy = e.clientY - this.dragStartY;
    if (
      !this.hasDragged &&
      Math.hypot(totalDx, totalDy) < this.dragThresholdPx
    ) {
      return;
    }

    this.offsetX += e.clientX - this.lastX;
    this.offsetY += e.clientY - this.lastY;
    this.lastX = e.clientX;
    this.lastY = e.clientY;
    this.hasDragged = true;
  }

  /**
   * Opens the color overlay for the clicked hex cell.
   */
  onHexClick(hex: RenderHex, event: MouseEvent): void {
    if (this.hasDragged) return;
    event.stopPropagation();

    const target = event.target as SVGElement | null;
    const attrFill = target?.getAttribute('fill');
    const computedFill = target ? getComputedStyle(target).fill : null;

    // Resolve picker color from model first, then SVG attribute/computed style.
    this.selectedHex = hex;
    this.selectedColor =
      toPickerHexColor(hex.fill) ??
      toPickerHexColor(attrFill) ??
      toPickerHexColor(computedFill) ??
      this.defaultFill.toLowerCase();
    this.colorAtOpen = this.selectedColor;
    this.lastLiveColor = this.selectedColor;
    this.spectrumErrorOpen = false;
    this.overlayOpen = true;
  }

  /**
   * Applies color changes in real time while the native picker is open.
   */
  applyColorLive(color: string): void {
    const normalizedColor = toPickerHexColor(color);
    if (!normalizedColor || !isValidHexColor(normalizedColor)) {
      this.closeOverlay();
      return;
    }

    // Native picker ESC can emit the opening color again; keep the latest live color and close.
    const cancelledBackToOpenColor =
      normalizedColor === this.colorAtOpen &&
      this.lastLiveColor !== this.colorAtOpen;
    if (cancelledBackToOpenColor) {
      if (this.selectedHex) {
        this.selectedHex.fill = this.lastLiveColor;
      }
      this.selectedColor = this.lastLiveColor;
      this.closeOverlay(true);
      return;
    }

    if (
      this.currentTeam !== null &&
      !isCorrectColorSpectrum(this.currentTeam, normalizedColor)
    ) {
      this.spectrumErrorOpen = true;
      return;
    }

    this.spectrumErrorOpen = false;
    this.lastLiveColor = normalizedColor;
    this.selectedColor = normalizedColor;
    if (!this.selectedHex) return;
    this.selectedHex.fill = normalizedColor;
  }

  /**
   * Closes the overlay and optionally persists the current selection.
   */
  closeOverlay(persist = false): void {
    if (persist) {
      const persisted = this.persistSelectedHexColor();
      if (!persisted) {
        return;
      }
    }

    this.overlayOpen = false;
    this.selectedHex = null;
    this.spectrumErrorOpen = false;
  }

  /**
   * Persists the selected cell color for the active team.
   */
  private persistSelectedHexColor(): boolean {
    if (!this.selectedHex || this.currentTeam === null) {
      return true;
    }

    const normalizedColor = toPickerHexColor(this.selectedHex.fill);
    if (
      !normalizedColor ||
      !isCorrectColorSpectrum(this.currentTeam, normalizedColor)
    ) {
      this.spectrumErrorOpen = true;
      return false;
    }

    this.selectedHex.fill = normalizedColor;

    const updateCell: UpdateColorDto = {
      q: this.selectedHex.q,
      r: this.selectedHex.r,
      hexColor: normalizedColor,
      teamColor: this.currentTeam,
    };

    this.hexGridService.updateCellColor(updateCell).subscribe({
      error: (err) => {
        console.error('Cell color update failed:', err);
      },
    });

    return true;
  }

  onSpectrumErrorClose(): void {
    this.spectrumErrorOpen = false;
    this.restoreSelectedHexColorFromDb();
  }

  private restoreSelectedHexColorFromDb(): void {
    if (!this.selectedHex || this.currentTeam === null) {
      return;
    }

    // Optimistically reset to the opening value, then sync with the latest DB value.
    this.selectedHex.fill = this.colorAtOpen;
    this.selectedColor = this.colorAtOpen;
    this.lastLiveColor = this.colorAtOpen;

    const selectedQ = this.selectedHex.q;
    const selectedR = this.selectedHex.r;

    this.hexGridService.getGrid(this.currentTeam).subscribe({
      next: (cells: CellModel[]) => {
        const dbColor =
          cells.find((c) => c.q === selectedQ && c.r === selectedR)?.hexColor ??
          this.colorAtOpen;
        const normalizedDbColor =
          toPickerHexColor(dbColor) ?? this.colorAtOpen.toLowerCase();

        if (!this.selectedHex) {
          return;
        }

        this.selectedHex.fill = normalizedDbColor;
        this.selectedColor = normalizedDbColor;
        this.lastLiveColor = normalizedDbColor;
        this.colorAtOpen = normalizedDbColor;
      },
      error: (err) => {
        console.error('Failed to restore color from DB:', err);
        if (!this.selectedHex) {
          return;
        }

        this.selectedHex.fill = this.colorAtOpen;
        this.selectedColor = this.colorAtOpen;
        this.lastLiveColor = this.colorAtOpen;
      },
    });
  }

  private loadGrid(team: TeamColor): void {
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

  private isValidTeam(team: number): team is TeamColor {
    return [TeamColor.Red, TeamColor.Green, TeamColor.Blue].includes(
      team as TeamColor,
    );
  }
}
