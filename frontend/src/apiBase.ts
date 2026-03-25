/**
 * Production: prefer `VITE_API_HOST` (hostname only). Scheme follows the page
 * (`window.location.protocol`) so API calls match https vs http.
 *
 * `VITE_API_URL` may be a full origin (`https://api.example.com`) or hostname
 * only. Hostname-only values MUST NOT be passed to fetch without a scheme —
 * the browser treats them as relative URLs and you get paths like
 * `/api.example.com/api/users` on the wrong host.
 *
 * Local dev: leave both unset — paths stay `/api/...` and Vite proxies them.
 */
export function apiUrl(path: string): string {
  const segment = path.startsWith('/') ? path : `/${path}`;

  const origin = resolveApiOrigin();
  if (origin) {
    return `${origin}${segment}`;
  }

  return segment;
}

function resolveApiOrigin(): string | null {
  const hostOnly = import.meta.env.VITE_API_HOST?.trim();
  if (hostOnly) {
    return hostToOrigin(stripSlashes(hostOnly));
  }

  const raw = import.meta.env.VITE_API_URL?.trim();
  if (!raw) {
    return null;
  }

  const base = stripSlashes(raw);

  if (/^https?:\/\//i.test(base)) {
    return base;
  }

  // Hostname without scheme (common misconfig) — same as VITE_API_HOST
  return hostToOrigin(base.replace(/^\/+/, ''));
}

function stripSlashes(s: string): string {
  return s.replace(/\/+$/, '');
}

function hostToOrigin(host: string): string {
  const protocol =
    typeof window !== 'undefined' ? window.location.protocol : 'https:';
  return `${protocol}//${host}`;
}
