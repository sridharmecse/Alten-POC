# Frontend Dockerfile
FROM node:18 AS build
WORKDIR /app
COPY package.json angular.json tsconfig.json proxy.conf.json ./
COPY src ./src
RUN npm ci || npm install
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/frontend /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
