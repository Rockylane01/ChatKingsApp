/// <reference types="vite/client" />

interface ImportMetaEnv {
  /** Hostname only, e.g. api.example.com — scheme follows the page (recommended). */
  readonly VITE_API_HOST?: string;
  /** Full origin override, e.g. https://api.example.com */
  readonly VITE_API_URL?: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
