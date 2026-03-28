import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

const TOKEN_KEY = 'token';

export const AuthGuard: CanActivateFn = () => {
  const router = inject(Router);
  const token = localStorage.getItem(TOKEN_KEY);

  if (!token) {
    return router.createUrlTree(['/login']);
  }

  const exp = getTokenExpirationEpoch(token);
  const now = Math.floor(Date.now() / 1000);

  if (exp === null || exp <= now) {
    clearSession();
    return router.createUrlTree(['/login']);
  }

  return true;
};

function getTokenExpirationEpoch(token: string): number | null {
  const payload = readJwtPayload(token);
  if (!payload) return null;

  const expValue = (payload as { exp?: unknown }).exp;
  if (typeof expValue === 'number' && Number.isFinite(expValue)) {
    return expValue;
  }

  if (typeof expValue === 'string') {
    const parsed = Number(expValue);
    if (Number.isFinite(parsed)) {
      return parsed;
    }
  }

  return null;
}

function readJwtPayload(token: string): Record<string, unknown> | null {
  try {
    const parts = token.split('.');
    if (parts.length < 2) {
      return null;
    }

    const base64Url = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    const paddedBase64 = base64Url.padEnd(
      base64Url.length + ((4 - (base64Url.length % 4)) % 4),
      '='
    );

    const json = atob(paddedBase64);
    const parsed = JSON.parse(json);

    if (typeof parsed !== 'object' || parsed === null) {
      return null;
    }

    return parsed as Record<string, unknown>;
  } catch {
    return null;
  }
}

function clearSession(): void {
  localStorage.removeItem('token');
  localStorage.removeItem('username');
  localStorage.removeItem('team');
  localStorage.removeItem('isAdmin');
}
