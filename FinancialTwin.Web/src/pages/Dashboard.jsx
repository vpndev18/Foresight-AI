import { useState, useEffect } from 'react';
import { getCurrentUser, getSimulations, logout } from '../services/api';
import { formatCurrency, formatDate, getInitials } from '../utils/formatters';
import SimulationRunner from '../components/SimulationRunner';
import SimulationChart from '../components/SimulationChart';

export default function Dashboard({ onLogout }) {
  const [user, setUser] = useState(null);
  const [simulations, setSimulations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showSimModal, setShowSimModal] = useState(false);
  const [expandedSim, setExpandedSim] = useState(null);

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    setLoading(true);
    try {
      const [userData, simsData] = await Promise.all([
        getCurrentUser(),
        getSimulations(),
      ]);
      setUser(userData);
      setSimulations(simsData);
    } catch (err) {
      console.error('Failed to load data:', err);
    } finally {
      setLoading(false);
    }
  }

  function handleLogout() {
    logout();
    onLogout();
  }

  function handleSimulationComplete(newSim) {
    setSimulations([newSim, ...simulations]);
    setShowSimModal(false);
    setExpandedSim(newSim);
  }

  if (loading) {
    return (
      <div className="app-layout">
        <NavbarSkeleton />
        <div className="main-content">
          <div className="stats-grid">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="card loading-skeleton" style={{ height: 140 }} />
            ))}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="app-layout">
      {/* Navbar */}
      <nav className="navbar">
        <div className="navbar-brand">
          <div className="logo-icon">💹</div>
          Financial Twin
        </div>
        <div className="navbar-right">
          <div className="navbar-user">
            <span>Welcome, <strong>{user?.name?.split(' ')[0]}</strong></span>
            <div className="user-avatar">
              {user ? getInitials(user.name) : '?'}
            </div>
          </div>
          <button className="btn btn-ghost btn-sm" onClick={handleLogout}>
            Logout
          </button>
        </div>
      </nav>

      {/* Main Content */}
      <div className="main-content fade-in">
        {/* Stats Cards */}
        <div className="stats-grid">
          <div className="card stat-card green">
            <div className="stat-icon">💰</div>
            <div className="stat-label">Current Savings</div>
            <div className="stat-value text-green">
              {formatCurrency(user?.currentSavings || 0, user?.currency)}
            </div>
            <div className="stat-sub">Your portfolio balance</div>
          </div>

          <div className="card stat-card blue">
            <div className="stat-icon">📊</div>
            <div className="stat-label">Simulations Run</div>
            <div className="stat-value" style={{ color: 'var(--accent-blue)' }}>
              {simulations.length}
            </div>
            <div className="stat-sub">Total scenarios analyzed</div>
          </div>

          <div className="card stat-card purple">
            <div className="stat-icon">💱</div>
            <div className="stat-label">Currency</div>
            <div className="stat-value" style={{ color: 'var(--accent-purple)' }}>
              {user?.currency || 'USD'}
            </div>
            <div className="stat-sub">Preferred trading currency</div>
          </div>

          <div className="card stat-card yellow">
            <div className="stat-icon">👤</div>
            <div className="stat-label">Account</div>
            <div className="stat-value" style={{ color: 'var(--accent-yellow)', fontSize: '1.125rem' }}>
              {user?.email}
            </div>
            <div className="stat-sub">{user?.name}</div>
          </div>
        </div>

        {/* Run Simulation CTA */}
        <div className="section-header">
          <h2>📈 My Simulations</h2>
          <button className="btn btn-primary" onClick={() => setShowSimModal(true)}>
            + New Simulation
          </button>
        </div>

        {/* Expanded Simulation Result */}
        {expandedSim && (
          <div className="card sim-result fade-in-up" style={{ marginBottom: 24 }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
              <h3>🤖 {expandedSim.title}</h3>
              <button className="btn btn-ghost btn-sm" onClick={() => setExpandedSim(null)}>✕ Close</button>
            </div>
            <div className="ai-story">{expandedSim.aiGeneratedStory}</div>
            <SimulationChart simulation={expandedSim} currency={user?.currency || 'USD'} />
          </div>
        )}

        {/* Simulations Table */}
        {simulations.length === 0 ? (
          <div className="card">
            <div className="empty-state">
              <div className="empty-icon">🔮</div>
              <p>No simulations yet. Run your first "what if" scenario!</p>
              <button className="btn btn-primary" onClick={() => setShowSimModal(true)}>
                Run First Simulation
              </button>
            </div>
          </div>
        ) : (
          <div className="card" style={{ padding: 0, overflow: 'hidden' }}>
            <table className="data-table">
              <thead>
                <tr>
                  <th>Scenario</th>
                  <th>Date</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {simulations.map((sim) => (
                  <tr key={sim.id}>
                    <td>
                      <strong>{sim.title}</strong>
                    </td>
                    <td className="text-muted" style={{ fontSize: '0.8125rem' }}>
                      {formatDate(sim.createdAt)}
                    </td>
                    <td>
                      <button
                        className="btn btn-secondary btn-sm"
                        onClick={() => setExpandedSim(expandedSim?.id === sim.id ? null : sim)}
                      >
                        {expandedSim?.id === sim.id ? 'Hide' : 'View'} AI Insight
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Simulation Modal */}
      {showSimModal && (
        <SimulationRunner
          userId={user?.id}
          currency={user?.currency}
          onClose={() => setShowSimModal(false)}
          onComplete={handleSimulationComplete}
        />
      )}
    </div>
  );
}

function NavbarSkeleton() {
  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <div className="logo-icon">💹</div>
        Financial Twin
      </div>
      <div className="navbar-right">
        <div className="loading-skeleton" style={{ width: 120, height: 20 }} />
      </div>
    </nav>
  );
}
