---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/AuthModal/AuthModal.tsx"
ref: HEAD
---

import { useState } from "react";
import InputBox from "../InputBox/InputBox";
import GlueMenuButton from "../GlueMenuButton/GlueMenuButton";
import { login, logout, register, useAuth } from "../../state/auth";

type Mode = "login" | "register";

function AuthForm({ mode, onClose }: { mode: Mode; onClose: () => void }) {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit() {
    setError(null);
    setLoading(true);
    try {
      if (mode === "login") {
        await login(email, password);
      } else {
        await register(username, email, password);
      }
      onClose();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Something went wrong");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-form">
      {mode === "register" && (
        <div className="auth-field">
          <label className="auth-label">Username</label>
          <InputBox value={username} onChange={setUsername} placeholder="Username" />
        </div>
      )}
      <div className="auth-field">
        <label className="auth-label">Email</label>
        <InputBox value={email} onChange={setEmail} placeholder="Email" />
      </div>
      <div className="auth-field">
        <label className="auth-label">Password</label>
        <InputBox value={password} onChange={setPassword} placeholder="Password" />
      </div>
      {error && <p className="auth-error">{error}</p>}
      <div className="auth-actions">
        <GlueMenuButton onClick={handleSubmit} disabled={loading} variant="single">
          {loading ? "..." : mode === "login" ? "Login" : "Register"}
        </GlueMenuButton>
        <GlueMenuButton onClick={onClose} variant="single">Cancel</GlueMenuButton>
      </div>
    </div>
  );
}

interface Props {
  onClose: () => void;
}

export default function AuthModal({ onClose }: Props) {
  const [mode, setMode] = useState<Mode>("login");

  return (
    <div className="auth-modal-backdrop" onClick={onClose}>
      <div className="auth-modal" onClick={(e) => e.stopPropagation()}>
        <div className="auth-tabs">
          <button
            className={`auth-tab${mode === "login" ? " auth-tab--active" : ""}`}
            onClick={() => setMode("login")}
          >
            Login
          </button>
          <button
            className={`auth-tab${mode === "register" ? " auth-tab--active" : ""}`}
            onClick={() => setMode("register")}
          >
            Register
          </button>
        </div>
        <AuthForm mode={mode} onClose={onClose} />
      </div>
    </div>
  );
}

export function AuthButton() {
  const auth = useAuth();
  const [open, setOpen] = useState(false);

  if (auth.loading) return null;

  if (auth.user) {
    return (
      <div className="auth-user-info">
        <span className="auth-username">{auth.user.username}</span>
        <button className="auth-logout-btn" onClick={logout}>
          Logout
        </button>
      </div>
    );
  }

  return (
    <>
      <GlueMenuButton onClick={() => setOpen(true)} variant="single">Login</GlueMenuButton>
      {open && <AuthModal onClose={() => setOpen(false)} />}
    </>
  );
}
