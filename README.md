# Foresight AI: Predictive Wealth Engine 📈

**Foresight AI** is a sophisticated financial forecasting platform that moves beyond simple compound interest calculators. By combining **Monte Carlo simulations** with **Generative AI insights**, it provides users with a probabilistic view of their financial future.

![Architecture](https://img.shields.io/badge/Architecture-Microservices-blue)
![Backend](https://img.shields.io/badge/.NET-9.0-purple)
![Frontend](https://img.shields.io/badge/React-19-blue)
![Database](https://img.shields.io/badge/PostgreSQL-Neon-orange)
![Docker](https://img.shields.io/badge/Docker-Enabled-blue)

---

## 🚀 Key Features

- **Monte Carlo Engine**: Simulates 1,000+ market scenarios to calculate P10 (Worst Case), P50 (Median), and P90 (Best Case) wealth trajectories.
- **AI Financial Advisor**: Integrates with Google Gemini to provide personalized, actionable financial advice based on simulation results.
- **Dynamic Visualization**: Interactive Chart.js graphs with localized currency support (INR, USD, etc.).
- **Containerized Stack**: Fully dockerized for seamless deployment and local development.
- **Secure Auth**: JWT-based authentication system for personalized user profiles.

---

## 🛠️ Tech Stack

- **Backend**: ASP.NET Core 9.0 (Minimal APIs, MediatR, EF Core)
- **Frontend**: React 19, Vite, Chart.js, Vanilla CSS
- **Database**: PostgreSQL (Cloud-hosted via Neon.tech)
- **Infrastructure**: Docker & Docker Compose
- **AI**: Google Gemini API

---

## 🚦 Getting Started (Docker)

The easiest way to run the entire stack locally is using Docker:

1. **Clone the repo**:
   ```bash
   git clone https://github.com/vpndev18/Foresight-AI.git
   cd Foresight-AI
   ```

2. **Setup Secrets**:
   Copy `FinancialTwin.API/appsettings.json.example` to `FinancialTwin.API/appsettings.json` and fill in your credentials (DB, AI API Key, JWT Key).

3. **Run with Docker Compose**:
   ```bash
   docker compose up --build
   ```

4. **Access the Apps**:
   - Frontend: `http://localhost:3000`
   - API Swagger: `http://localhost:8080/swagger`

---

## 📂 Architecture Note

This project follows a feature-based folder structure (Vertical Slice Architecture) within the .NET API to maintain high cohesion and keep logic organized as the application scales.

---
