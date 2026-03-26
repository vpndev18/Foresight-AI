/**
 * Format a number as currency using Intl.NumberFormat
 */
export function formatCurrency(amount, currency = 'USD') {
  return new Intl.NumberFormat(undefined, {
    style: 'currency',
    currency: currency,
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  }).format(amount);
}

/**
 * Format a number with commas (for generic numbers)
 */
export function formatNumber(num) {
  return new Intl.NumberFormat(undefined).format(num);
}

/**
 * Format a percentage
 */
export function formatPercent(value) {
  return `${(value * 100).toFixed(1)}%`;
}

/**
 * Format a date string nicely
 */
export function formatDate(dateStr) {
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

/**
 * Get user's initials from name
 */
export function getInitials(name) {
  return name
    .split(' ')
    .map((w) => w[0])
    .join('')
    .toUpperCase()
    .slice(0, 2);
}
