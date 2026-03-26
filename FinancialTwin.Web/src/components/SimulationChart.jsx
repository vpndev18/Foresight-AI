import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Filler,
  Legend,
} from 'chart.js';
import { Line } from 'react-chartjs-2';
import { formatCurrency } from '../utils/formatters';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Filler,
  Legend
);

export default function SimulationChart({ simulation, currency = 'USD' }) {
  if (!simulation) return null;

  const { p10Trajectory = [], p50Trajectory = [], p90Trajectory = [] } = simulation;

  if (p50Trajectory.length === 0) return null;

  const labels = p50Trajectory.map((_, i) => `Year ${i}`);

  const data = {
    labels,
    datasets: [
      {
        label: 'Best Case (90th Percentile)',
        data: p90Trajectory,
        borderColor: 'rgba(75, 192, 192, 0.8)',
        backgroundColor: 'transparent',
        borderDash: [5, 5],
        tension: 0.4,
      },
      {
        label: 'Median Case',
        data: p50Trajectory,
        borderColor: 'rgba(54, 162, 235, 1)',
        backgroundColor: 'rgba(54, 162, 235, 0.1)',
        fill: true,
        tension: 0.4,
      },
      {
        label: 'Worst Case (10th Percentile)',
        data: p10Trajectory,
        borderColor: 'rgba(255, 99, 132, 0.8)',
        backgroundColor: 'transparent',
        borderDash: [5, 5],
        tension: 0.4,
      },
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        labels: { color: '#a0a0b0' }
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        callbacks: {
          label: function(context) {
            let label = context.dataset.label || '';
            if (label) {
              label += ': ';
            }
            if (context.parsed.y !== null) {
              label += formatCurrency(context.parsed.y, currency);
            }
            return label;
          }
        }
      }
    },
    scales: {
      y: {
        beginAtZero: true,
        grid: { color: 'rgba(255, 255, 255, 0.1)' },
        ticks: { 
          color: '#a0a0b0',
          callback: function(value) {
            return formatCurrency(value, currency);
          }
        }
      },
      x: {
        grid: { display: false },
        ticks: { color: '#a0a0b0' }
      }
    }
  };

  return (
    <div style={{ height: '350px', width: '100%', marginTop: '20px' }}>
      <Line options={options} data={data} />
    </div>
  );
}
