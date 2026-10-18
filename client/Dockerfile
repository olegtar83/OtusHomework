# Build stage
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy only package files first for caching
COPY package*.json ./

# Install dependencies
RUN npm ci

# Now copy the rest of the application code
COPY . .

# Build the application. Create React App writes the output to /app/build.
ENV CI=false
RUN npm run build

# Production stage
FROM nginx:alpine

# Copy built assets from build stage
COPY --from=build /app/build /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start Nginx server
CMD ["nginx", "-g", "daemon off;"]