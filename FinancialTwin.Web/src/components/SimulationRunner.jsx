import { useState } from 'react';
import { runSimulation } from '../services/api';

export default function SimulationRunner({ userId, currency, onClose, onComplete }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [form, setForm] = useState({
    title: '',
    monthlyContribution: '',
    expectedAnnualReturn: '',
    annualVolatility: '15',
  });

  function updateField(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
    setError('');
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const result = await runSimulation({
        userId,
        title: form.title,
        monthlyContribution: form.monthlyContribution,
        expectedAnnualReturn: parseFloat(form.expectedAnnualReturn) / 100, // convert % to decimal
        annualVolatility: parseFloat(form.annualVolatility) / 100,
      });
      onComplete(result);
    } catch (err) {
      setError(err.message || 'Failed to run simulation');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>🚀 New Simulation</h2>
          <button className="modal-close" onClick={onClose}>✕</button>
        </div>

        <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem', marginBottom: 24 }}>
          Run a "what if" scenario to see your 10-year financial trajectory with AI-powered insights.
        </p>

        {error && <div className="auth-error" style={{ marginBottom: 16 }}>{error}</div>}

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label className="form-label">Scenario Title</label>
            <input
              className="form-input"
              type="text"
              name="title"
              placeholder="e.g., Conservative Index Funds"
              value={form.title}
              onChange={updateField}
              required
            />
          </div>

          <div className="form-group">
            <label className="form-label">Monthly Contribution ({currency})</label>
            <input
              className="form-input mono"
              type="number"
              name="monthlyContribution"
              placeholder="500"
              value={form.monthlyContribution}
              onChange={updateField}
              required
              min="0"
              step="0.01"
            />
          </div>

          <div className="form-group">
            <label className="form-label">Expected Annual Return (%)</label>
            <input
              className="form-input mono"
              type="number"
              name="expectedAnnualReturn"
              placeholder="8"
              value={form.expectedAnnualReturn}
              onChange={updateField}
              required
              min="0"
              max="100"
              step="0.1"
            />
          </div>

          <div className="form-group">
            <label className="form-label">Market Volatility (%)</label>
            <input
              className="form-input mono"
              type="number"
              name="annualVolatility"
              placeholder="15"
              value={form.annualVolatility}
              onChange={updateField}
              required
              min="0"
              max="100"
              step="0.1"
            />
          </div>

          <div style={{ display: 'flex', gap: 12, marginTop: 8 }}>
            <button className="btn btn-secondary" type="button" onClick={onClose} style={{ flex: 1 }}>
              Cancel
            </button>
            <button className="btn btn-primary" type="submit" disabled={loading} style={{ flex: 2 }}>
              {loading ? (
                <>
                  <span className="spinner" /> Running Simulation...
                </>
              ) : (
                'Run Simulation →'
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
