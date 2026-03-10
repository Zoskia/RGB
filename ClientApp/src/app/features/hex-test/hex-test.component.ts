import { Component } from '@angular/core';

@Component({
  selector: 'app-hex-test',
  standalone: true,
  template: `
    <section class="hex-test">
      <h1>Hex Test</h1>
      <p>This route is kept as a placeholder component.</p>
    </section>
  `,
  styles: [
    `
      .hex-test {
        padding: 2rem;
        font-family: "Segoe UI", sans-serif;
      }
    `,
  ],
})
export class HexTestComponent {}
