import { FormEvent, useCallback, useEffect, useState } from 'react';

type Decision = { applicationId: string; status: 'Successful' | 'Declined'; loanToValuePercent: number; reason: string; submittedAt: string };
type Portfolio = { totalApplications: number; successfulApplications: number; declinedApplications: number; totalLoansWritten: number; meanLoanToValuePercent: number };

const emptyPortfolio: Portfolio = { totalApplications: 0, successfulApplications: 0, declinedApplications: 0, totalLoansWritten: 0, meanLoanToValuePercent: 0 };
const currency = new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP', maximumFractionDigits: 0 });

export default function App() {
  const [loanAmount, setLoanAmount] = useState('');
  const [assetValue, setAssetValue] = useState('');
  const [creditScore, setCreditScore] = useState('');
  const [portfolio, setPortfolio] = useState<Portfolio>(emptyPortfolio);
  const [decision, setDecision] = useState<Decision | null>(null);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const loadPortfolio = useCallback(async () => {
    try {
      const response = await fetch('http://localhost:5000/api/portfolio');
      if (response.ok) setPortfolio(await response.json());
    } catch { /* API may not be running yet. */ }
  }, []);

  useEffect(() => { void loadPortfolio(); }, [loadPortfolio]);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setError(''); setDecision(null); setSubmitting(true);
    try {
      const response = await fetch('http://localhost:5000/api/applications', {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ loanAmount: Number(loanAmount), assetValue: Number(assetValue), creditScore: Number(creditScore) })
      });
      if (!response.ok) {
        const problem = await response.json();
        setError(Object.values(problem.errors ?? {}).flat().join(' ') || 'Unable to submit this application.');
        return;
      }
      setDecision(await response.json());
      await loadPortfolio();
    } catch { setError('Could not reach the API. Start the backend and try again.'); }
    finally { setSubmitting(false); }
  }

  return <main>
    <header>
  <p className="eyebrow">BLACKFINCH</p>

  <h1>Lending decision desk</h1>

  <p className="subtitle">
    Assess a secured loan application against the lending rules.
  </p>
</header>
    <section className="grid">
      <form onSubmit={submit} className="card">
        <h2>New application</h2>
        <label>Loan amount <span>GBP</span><input type="number" min="0" step="0.01" required value={loanAmount} onChange={e => setLoanAmount(e.target.value)} placeholder="e.g. 850000" /></label>
        <label>Asset value <span>GBP</span><input type="number" min="0.01" step="0.01" required value={assetValue} onChange={e => setAssetValue(e.target.value)} placeholder="e.g. 1200000" /></label>
        <label>Credit score <span>1 - 999</span><input type="number" min="1" max="999" required value={creditScore} onChange={e => setCreditScore(e.target.value)} placeholder="e.g. 820" /></label>
        {error && <p className="error" role="alert">{error}</p>}
        <button disabled={submitting}>{submitting ? 'Assessing…' : 'Assess application'}</button>
      </form>
     <aside className="card outcome" aria-live="polite">
  <h2>Decision</h2>

  {decision ? (
    <>
      <p
        className={`badge ${
          decision.status === "Successful"
            ? "approved"
            : "declined"
        }`}
      >
        {decision.status === "Successful"
          ? "✓ Successful"
          : "✕ Declined"}
      </p>

      <p className="decision-time">
        Decision generated:
        {" "}
        {new Date(
          decision.submittedAt
        ).toLocaleString()}
      </p>

      <p className="ltv">
        {decision.loanToValuePercent.toFixed(2)}%
        <small>LTV</small>
      </p>

      <p className="reason">
        {decision.reason}
      </p>
    </>
  ) : (
    <div className="empty-state">
      <div className="icon-circle">📄</div>

      <p className="muted">
        Submit an application to see
        its outcome and explanation.
      </p>
    </div>
  )}
</aside>
    </section>
    <section className="metrics" aria-label="Portfolio metrics">
      <Metric label="Applications" value={String(portfolio.totalApplications)} />
      <Metric label="Successful" value={String(portfolio.successfulApplications)} />
      <Metric label="Declined" value={String(portfolio.declinedApplications)} />
      <Metric label="Loans written" value={currency.format(portfolio.totalLoansWritten)} />
      <Metric label="Mean LTV" value={`${portfolio.meanLoanToValuePercent.toFixed(2)}%`} />
    </section>
  </main>;
}

function Metric({
  label,
  value
}: {
  label: string;
  value: string;
}) {
  return (
    <article className="metric">
      <p>{label}</p>
      <strong>{value}</strong>
    </article>
  );
}