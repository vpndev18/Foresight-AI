import { useState } from 'react';
import { login, register } from '../services/api';

export default function LoginPage({ onAuth }) {
  const [isLogin, setIsLogin] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const [form, setForm] = useState({
    name: '',
    email: '',
    password: '',
    currentSavings: '',
    currency: 'USD',
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
      if (isLogin) {
        await login({ email: form.email, password: form.password });
      } else {
        await register(form);
      }
      onAuth();
    } catch (err) {
      setError(err.message || 'Something went wrong');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-container">
        <div className="auth-card fade-in-up">
          <div className="auth-header">
            <div className="auth-logo">💹</div>
            <h1>Financial Twin</h1>
            <p>{isLogin ? 'Welcome back, sign in to continue' : 'Create your account to get started'}</p>
          </div>

          {error && <div className="auth-error">{error}</div>}

          <form className="auth-form" onSubmit={handleSubmit}>
            {!isLogin && (
              <div className="form-group">
                <label className="form-label">Full Name</label>
                <input
                  className="form-input"
                  type="text"
                  name="name"
                  placeholder="John Doe"
                  value={form.name}
                  onChange={updateField}
                  required
                />
              </div>
            )}

            <div className="form-group">
              <label className="form-label">Email Address</label>
              <input
                className="form-input"
                type="email"
                name="email"
                placeholder="you@example.com"
                value={form.email}
                onChange={updateField}
                required
              />
            </div>

            <div className="form-group">
              <label className="form-label">Password</label>
              <input
                className="form-input"
                type="password"
                name="password"
                placeholder="••••••••"
                value={form.password}
                onChange={updateField}
                required
                minLength={6}
              />
            </div>

            {!isLogin && (
              <>
                <div className="form-group">
                  <label className="form-label">Current Savings</label>
                  <input
                    className="form-input mono"
                    type="number"
                    name="currentSavings"
                    placeholder="10000"
                    value={form.currentSavings}
                    onChange={updateField}
                    required
                    min="0"
                    step="0.01"
                  />
                </div>

                <div className="form-group">
                  <label className="form-label">Preferred Currency</label>
                  <select
                    className="form-input"
                    name="currency"
                    value={form.currency}
                    onChange={updateField}
                  >
                    <option value="USD">🇺🇸 USD — US Dollar</option>
                    <option value="EUR">🇪🇺 EUR — Euro</option>
                    <option value="GBP">🇬🇧 GBP — British Pound</option>
                    <option value="INR">🇮🇳 INR — Indian Rupee</option>
                    <option value="JPY">🇯🇵 JPY — Japanese Yen</option>
                    <option value="CAD">🇨🇦 CAD — Canadian Dollar</option>
                    <option value="AUD">🇦🇺 AUD — Australian Dollar</option>
                  </select>
                </div>
              </>
            )}

            <button className="btn btn-primary btn-lg" type="submit" disabled={loading}>
              {loading ? (
                <span className="spinner" />
              ) : isLogin ? (
                'Sign In →'
              ) : (
                'Create Account →'
              )}
            </button>
          </form>

          <div className="auth-toggle">
            {isLogin ? (
              <>
                Don't have an account?{' '}
                <a onClick={() => { setIsLogin(false); setError(''); }}>Sign up</a>
              </>
            ) : (
              <>
                Already have an account?{' '}
                <a onClick={() => { setIsLogin(true); setError(''); }}>Sign in</a>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
