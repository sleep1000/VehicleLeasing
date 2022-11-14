/* eslint-disable no-undef */
const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:15952';

const context = [
  '/weatherforecast',
  '/_configuration',
  '/.well-known',
  '/Identity',
  '/connect',
  '/ApplyDatabaseMigrations',
  '/_framework',
  '/api',
  '/images',
];

const wsContext = [
  '/leasesHub',
];

module.exports = function (app) {
  const appProxy = createProxyMiddleware(context, {
    target,
    secure: false,
    headers: {
      Connection: 'Keep-Alive',
    },
  });

  const wsProxy = createProxyMiddleware(wsContext, {
    target,
    secure: false,
    ws: true,
  });

  app.use(appProxy);
  app.use(wsProxy);
};
