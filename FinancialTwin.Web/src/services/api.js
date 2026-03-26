// Empty string so all /api/* requests are relative to the current host.
// In Docker: Nginx proxies /api/* → api:8080
// In Dev: Update vite.config.js proxy or use full URL for local dev
const API_BASE = '';

function getToken() {
  return localStorage.getItem('ft_token');
}

function setToken(token) {
  localStorage.setItem('ft_token', token);
}

function clearToken() {
  localStorage.removeItem('ft_token');
}

function isAuthenticated() {
  return !!getToken();
}

async function request(endpoint, options = {}) {
  const token = getToken();
  const headers = {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` }),
    ...options.headers,
  };

  const res = await fetch(`${API_BASE}${endpoint}`, {
    ...options,
    headers,
  });

  if (res.status === 401) {
    clearToken();
    window.location.reload();
    throw new Error('Session expired');
  }

  if (!res.ok) {
    const data = await res.json().catch(() => ({}));
    throw new Error(data.error || data.Error || `Request failed (${res.status})`);
  }

  if (res.status === 204) return null;
  return res.json();
}

// Auth
export async function register({ name, email, password, currentSavings, currency }) {
  const data = await request('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify({ name, email, password, currentSavings: Number(currentSavings), currency }),
  });
  setToken(data.token);
  return data;
}

export async function login({ email, password }) {
  const data = await request('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password }),
  });
  setToken(data.token);
  return data;
}

export function logout() {
  clearToken();
}

// User
export async function getCurrentUser() {
  return request('/api/users/me');
}

// Simulations
export async function getSimulations() {
  return request('/api/simulations');
}

export async function runSimulation({ userId, title, monthlyContribution, expectedAnnualReturn, annualVolatility }) {
  return request('/api/simulations', {
    method: 'POST',
    body: JSON.stringify({
      userId,
      title,
      monthlyContribution: Number(monthlyContribution),
      expectedAnnualReturn: Number(expectedAnnualReturn),
      annualVolatility: Number(annualVolatility),
    }),
  });
}

export { isAuthenticated };
