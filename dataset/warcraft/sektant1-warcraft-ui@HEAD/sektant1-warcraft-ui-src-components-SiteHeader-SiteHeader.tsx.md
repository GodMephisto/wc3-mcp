---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/SiteHeader/SiteHeader.tsx"
ref: HEAD
---

import GlueSmallButton from "../GlueSmallButton/GlueSmallButton";
import { useAuth } from "../../state/auth";
import { setPage } from "../../state/page";

export default function SiteHeader() {
  const auth = useAuth();

  return (
    <header className="site-header">
      <button
        type="button"
        className="site-header-logo"
        onClick={() => setPage("home")}
      >
        WC3BUILDS
      </button>

      <nav className="site-header-nav" aria-label="Site navigation">
        <button
          type="button"
          className="site-header-nav-link"
          onClick={() => setPage("home")}
        >
          Home
        </button>
        <button
          type="button"
          className="site-header-nav-link"
          onClick={() => setPage("build-orders")}
        >
          Build Orders
        </button>
      </nav>

      <div className="site-header-auth">
        {!auth.loading && (
          auth.user ? (
            <div className="site-header-user">
              <span className="site-header-username">
                <span className="site-header-sword" aria-hidden="true">⚔</span>
                {auth.user.username}
              </span>
              <GlueSmallButton onClick={() => setPage("login")}>
                Account
              </GlueSmallButton>
            </div>
          ) : (
            <GlueSmallButton onClick={() => setPage("login")}>
              Login
            </GlueSmallButton>
          )
        )}
      </div>
    </header>
  );
}
