FROM node:22-alpine AS build

ENV PNPM_HOME=/pnpm
ENV PATH=$PNPM_HOME:$PATH

WORKDIR /app

RUN corepack enable

COPY package.json pnpm-lock.yaml pnpm-workspace.yaml nx.json tsconfig.json ./
COPY apps/web/package.json apps/web/package.json
COPY packages/config/package.json packages/config/package.json
COPY packages/env/package.json packages/env/package.json
COPY packages/ui/package.json packages/ui/package.json
COPY libs/generated/api-types/package.json libs/generated/api-types/package.json

RUN pnpm install --frozen-lockfile --ignore-scripts

COPY apps/web apps/web
COPY packages/config packages/config
COPY packages/env packages/env
COPY packages/ui packages/ui
COPY libs/generated/api-types libs/generated/api-types

ARG VITE_API_BASE_URL=http://localhost:5086
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL

RUN pnpm --filter web build

FROM nginx:1.27-alpine AS runtime

COPY apps/web/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/apps/web/dist /usr/share/nginx/html

EXPOSE 80

