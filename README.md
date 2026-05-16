# WhatsApp SaaS Bot

> Plataforma multi-tenant de atención al cliente automatizada vía WhatsApp para PYMEs latinoamericanas.

![.NET](https://img.shields.io/badge/.NET_Core-10-512BD4?style=flat-square&logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-20-DD0031?style=flat-square&logo=angular)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=flat-square&logo=postgresql)
![Redis](https://img.shields.io/badge/Redis-7-DC382D?style=flat-square&logo=redis)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat-square&logo=docker)

---

## ¿Qué es?

WhatsApp SaaS Bot permite a cualquier PYME (restaurante, salón, tienda) automatizar su atención al cliente en WhatsApp sin necesidad de personal dedicado. Cada negocio tiene su propio bot configurado con su menú, horarios y respuestas personalizadas.

---

## Características principales

- 🤖 **Bot inteligente** — Responde preguntas frecuentes, toma pedidos y agenda citas automáticamente
- 👥 **Multi-tenant** — Cada negocio tiene sus datos completamente aislados
- 📊 **Dashboard en tiempo real** — El dueño ve todas las conversaciones y métricas en vivo
- 🔀 **Escalado humano** — Si el bot no sabe responder, notifica al dueño al instante
- 🌎 **Diseñado para LATAM** — Soporte para español, pagos con Stripe y MercadoPago

---

## Stack tecnológico

| Capa | Tecnología |
|------|-----------|
| Backend | .NET Core 10 — Clean Architecture + CQRS |
| Frontend | Angular 20 — Signals + Design System propio |
| Base de datos | PostgreSQL 16 (Supabase en producción) |
| Caché / Sesiones | Redis 7 |
| IA | Claude API (Anthropic) |
| Mensajería | WhatsApp Business API (Meta) |
| Hosting | Railway (backend) + Vercel (frontend) |

---

## Arquitectura

```
src/
├── WhatsAppSaaS.Domain/          # Entidades, Value Objects, Domain Events
├── WhatsAppSaaS.Application/     # CQRS Commands/Queries, Handlers, DTOs
├── WhatsAppSaaS.Infrastructure/  # DB, WhatsApp API, Claude API, Redis
├── WhatsAppSaaS.WebApi/          # Controllers, Middlewares, Webhooks
└── WhatsAppSaaS.Tests/           # Unit + Integration tests
```

---

## Desarrollo local

### Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js 22+](https://nodejs.org)
- [ngrok](https://ngrok.com) (para recibir webhooks de Meta en local)

### Levantar entorno local

```bash
# 1. Clonar el repositorio
git clone https://github.com/juanpablomato89/whatsapp-saas-bot.git
cd whatsapp-saas-bot

# 2. Levantar PostgreSQL + Redis con Docker
docker-compose up -d

# 3. Ejecutar migraciones
cd src/WhatsAppSaaS.WebApi
dotnet ef database update

# 4. Correr el backend
dotnet run

# 5. En otra terminal — exponer localhost a Meta
ngrok http 5000
```

### Variables de entorno

Copia `appsettings.Development.json.example` a `appsettings.Development.json` y completa:

```json
{
  "Meta": {
    "VerifyToken": "tu_token_aqui",
    "AppSecret": "tu_app_secret"
  },
  "Anthropic": {
    "ApiKey": "tu_api_key"
  }
}
```

---

## Roadmap

- [x] Fase 1 — Núcleo del sistema (multi-tenant, auth, webhook)
- [ ] Fase 2 — Motor del bot con IA
- [ ] Fase 3 — Dashboard Angular
- [ ] Fase 4 — Pedidos y citas
- [ ] Fase 5 — Pagos y onboarding

---

## Licencia

Propietario — todos los derechos reservados.
