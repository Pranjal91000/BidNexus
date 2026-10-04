import { Component, type ErrorInfo, type ReactNode } from 'react';

interface State {
  error: Error | null;
}

/** Keeps one broken screen from blanking the whole app; offers a way back. */
export class ErrorBoundary extends Component<{ children: ReactNode; resetKey?: string }, State> {
  state: State = { error: null };

  static getDerivedStateFromError(error: Error): State {
    return { error };
  }

  componentDidCatch(error: Error, info: ErrorInfo) {
    console.error('Screen failed to render', error, info.componentStack);
  }

  componentDidUpdate(prev: { resetKey?: string }) {
    if (prev.resetKey !== this.props.resetKey && this.state.error) this.setState({ error: null });
  }

  render() {
    if (!this.state.error) return this.props.children;
    return (
      <main className="page">
        <div className="card empty">
          <span className="empty__title">This screen couldn't be shown</span>
          <span className="small">Something in the data was unexpected. Reload, or go back to the overview.</span>
          <div className="row" style={{ marginTop: 8 }}>
            <button type="button" className="btn btn--secondary" onClick={() => window.location.reload()}>Reload</button>
            <a className="btn btn--primary" href="#/overview">Overview</a>
          </div>
        </div>
      </main>
    );
  }
}
