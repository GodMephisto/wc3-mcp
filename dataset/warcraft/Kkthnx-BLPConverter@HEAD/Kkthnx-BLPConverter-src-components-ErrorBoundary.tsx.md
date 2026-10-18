---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/ErrorBoundary.tsx"
ref: HEAD
---

import { Component, type ErrorInfo, type ReactNode } from "react";
import { AlertTriangle } from "lucide-react";
import { withTranslation, type WithTranslation } from "react-i18next";

interface Props extends WithTranslation {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

class ErrorBoundaryBase extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, info: ErrorInfo): void {
    console.error("BLP Converter error:", error, info.componentStack);
  }

  render() {
    const { t } = this.props;

    if (this.state.hasError) {
      return (
        <div className="flex h-full flex-col items-center justify-center gap-4 bg-workspace-bg p-8">
          <AlertTriangle className="h-12 w-12 text-workspace-accent" />
          <h1 className="text-xl font-semibold text-white">
            {t("errorBoundary.title")}
          </h1>
          <p className="max-w-md text-center text-sm text-workspace-silver-muted">
            {this.state.error?.message ?? t("errorBoundary.fallback")}
          </p>
          <button
            type="button"
            onClick={() => this.setState({ hasError: false, error: null })}
            className="rounded-lg border border-workspace-accent/40 bg-workspace-accent/10 px-4 py-2 text-sm text-workspace-accent transition hover:bg-workspace-accent/20"
          >
            {t("errorBoundary.retry")}
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}

export const ErrorBoundary = withTranslation()(ErrorBoundaryBase);
