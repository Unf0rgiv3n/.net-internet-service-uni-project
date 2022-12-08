from node:19.2-alpine
workdir /app
copy package*.json /app/
RUN npm install --quiet
copy . .
cmd ["npm", "run", "serve"]