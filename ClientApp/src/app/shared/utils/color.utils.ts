import { TeamColor } from '../../enums/team-color.enum';

// Converts supported color formats into #rrggbb for <input type="color">.
export function toPickerHexColor(
  color: string | null | undefined,
): string | null {
  const value = (color ?? '').trim();

  if (/^#[0-9A-Fa-f]{6}$/.test(value)) {
    return value.toLowerCase();
  }

  // Accept #RRGGBBAA and strip alpha channel for the native color picker.
  const hex8 = value.match(/^#([0-9A-Fa-f]{8})$/);
  if (hex8) {
    return `#${hex8[1].slice(0, 6)}`.toLowerCase();
  }

  // Accept shorthand #RGB and expand to #RRGGBB.
  const shortHex = value.match(/^#([0-9A-Fa-f]{3})$/);
  if (shortHex) {
    const [r, g, b] = shortHex[1].split('');
    return `#${r}${r}${g}${g}${b}${b}`.toLowerCase();
  }

  // Accept rgb()/rgba() and convert channel values to hex.
  const rgb = value.match(
    /^rgba?\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})(?:\s*,\s*(?:0|1|0?\.\d+))?\s*\)$/i,
  );
  if (rgb) {
    const [r, g, b] = [rgb[1], rgb[2], rgb[3]].map((v) => {
      const n = Number(v);
      const clamped = Math.max(0, Math.min(255, n));
      return clamped.toString(16).padStart(2, '0');
    });
    return `#${r}${g}${b}`;
  }

  return null;
}

// Native color inputs require exactly a 6-digit hex string.
export function isValidHexColor(color: string): boolean {
  return /^#[0-9A-Fa-f]{6}$/.test(color);
}

// Validating correct color spectrum for current grid

export function isCorrectColorSpectrum(
  teamColor: TeamColor,
  color: string,
): boolean {
  const normalizedColor = toPickerHexColor(color);
  if (!normalizedColor || !isValidHexColor(normalizedColor)) return false;

  const r = parseInt(normalizedColor.slice(1, 3), 16);
  const g = parseInt(normalizedColor.slice(3, 5), 16);
  const b = parseInt(normalizedColor.slice(5, 7), 16);

  // Require one strictly dominant channel; ties are invalid.
  let dominantTeam: TeamColor | null = null;
  if (r > g && r > b) dominantTeam = TeamColor.Red;
  if (g > r && g > b) dominantTeam = TeamColor.Green;
  if (b > r && b > g) dominantTeam = TeamColor.Blue;

  return dominantTeam !== null && dominantTeam === teamColor;
}
