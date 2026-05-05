# Étape 1 : build Angular
FROM node:22-alpine AS build
WORKDIR /app

# Copier uniquement les fichiers de dépendances d'abord (cache npm)
COPY package*.json ./
RUN npm ci

# Copier le reste du code
COPY . .

# Build Angular en mode prod
RUN npm run build:prod

# Étape 2 : image finale NGINX
FROM nginx:1.27-alpine

# Config NGINX custom
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copier les fichiers buildés Angular
COPY --from=build /app/dist/resumemanagerwebappangular /usr/share/nginx/html

# Azure App Service Container écoute sur 8080
EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]