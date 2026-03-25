/**
 * When VITE_API_URL is unset, paths stay relative so the Vite dev proxy can
 * forward /api to the local backend. In production (Dokploy), set VITE_API_URL
 * to the API origin, e.g. https://api.example.com — no trailing slash.
 */
export function apiUrl(path: string): string {
  const raw = import.meta.env.VITE_API_URL;
  const base =
    typeof raw === 'string' && raw.length > 0 ? raw.replace(/\/+$/, '') : '';
  const segment = path.startsWith('/') ? path : `/${path}`;
  return base ? `${base}${segment}` : segment;
}
