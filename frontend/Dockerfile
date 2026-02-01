# ---- Build Angular app ----
FROM node:18 AS build

WORKDIR /app

# Copy dependency definitions first for better layer caching
COPY package.json package-lock.json ./
# Install dependencies
RUN npm ci

# Copy configuration and source
COPY angular.json tsconfig.json proxy.conf.json ./
COPY src ./src

# Build the Angular app in production mode (smaller JS, optimized)
RUN npm run build -- --configuration=production

# ---- NGINX stage ----
FROM nginx:alpine

# Copy built Angular app from the builder
COPY --from=build /app/dist/frontend /usr/share/nginx/html
# Use your custom nginx config, if it exists
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80