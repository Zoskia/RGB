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
