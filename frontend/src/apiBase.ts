/**
 * Production (Cloudflare / tunnel): prefer `VITE_API_HOST` (hostname only). The
 * browser uses the same scheme as the page (`window.location.protocol`), so API
 * calls stay https when the site is https and avoid mixed-content mistakes from
 * a wrong `VITE_API_URL` scheme.
 *
 * Override with full `VITE_API_URL` if you need an explicit origin (e.g. tests).
 *
 * Local dev: leave both unset — paths stay relative and Vite proxies `/api`.
 */
export function apiUrl(path: string): string {
  const segment = path.startsWith('/') ? path : `/${path}`;

  const host = import.meta.env.VITE_API_HOST?.trim();
  if (host) {
    const h = host.replace(/\/+$/, '');
    const protocol =
      typeof window !== 'undefined' ? window.location.protocol : 'https:';
    return `${protocol}//${h}${segment}`;
  }

  const raw = import.meta.env.VITE_API_URL?.trim();
  if (raw) {
    const base = raw.replace(/\/+$/, '');
    return `${base}${segment}`;
  }

  return segment;
}
