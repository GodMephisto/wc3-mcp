---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/state/auth.ts"
ref: HEAD
---

import { createExternalStore } from "./createStore";

interface AuthUser {
  username: string;
  email: string;
}

interface AuthState {
  loading: boolean;
  user: AuthUser | null;
}

const authStore = createExternalStore<AuthState>({ loading: false, user: null });

export const useAuth = authStore.useValue;

export async function login(_email: string, _password: string): Promise<void> {
  throw new Error("Auth not configured");
}

export async function register(_username: string, _email: string, _password: string): Promise<void> {
  throw new Error("Auth not configured");
}

export function logout(): void {
  authStore.set({ loading: false, user: null });
}
