# =============================================
# Stage 1: BUILD
# Uses Node.js to install dependencies and build
# the React/Vite app into static files
# =============================================
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files and install dependencies first
# (Cached as a separate layer — speeds up rebuilds)
COPY package.json package-lock.json ./
RUN npm ci

# Copy rest of the source and build for production
COPY . .
RUN npm run build

# =============================================
# Stage 2: RUNTIME
# Uses lightweight Nginx to serve the static files
# The final image contains NO Node.js or source code
# =============================================
FROM nginx:alpine AS final

# Copy the built static files from the build stage into Nginx's web root
COPY --from=build /app/dist /usr/share/nginx/html

# Copy the custom Nginx config (handles React Router SPA routing)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Nginx listens on port 80 by default
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
