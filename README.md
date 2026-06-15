# EventosVivos — Sistema de Reservas

Sistema de gestión de eventos y reservas desarrollado como prueba técnica fullstack con .NET 10 y Angular 19.
---
## Demo en producción

ACCEDER AQUÍ
_________________________________
https://eventosvivos.vercel.app/
_________________________________

| Servicio | URL |
|---|---|
| Frontend | https://vercel.com/eduardo-t-s/eventosvivos |
| Backend API | https://railway.com/project/cc31ffd8-e0fb-4913-803a-8f8b33f2f890/service/3dff43c5-0f8a-4d21-a085-024994ed7fae?environmentId=84b3f89a-5553-4eca-89d4-fac79aa7264a |
---

##  Arquitectura

### Backend — Clean Architecture

EventosVivos/

├── EventosVivos.Domain/           # Entidades, enums, interfaces, excepciones de dominio

├── EventosVivos.Application/      # Use cases (CQRS + MediatR), DTOs, validaciones

├── EventosVivos.Infrastructure/   # EF Core (SQLite), repositorios

├── EventosVivos.Api/              # Controllers REST, middleware, configuración

├── EventosVivos.UnitTests/        # Pruebas unitarias de dominio

└── EventosVivos.IntegrationTests/ # Pruebas de integración HTTP end-to-end

**Decisiones arquitectónicas:**

- **Clean Architecture** — el dominio no tiene dependencias externas. Las reglas de negocio viven en las entidades (`Evento.Create()`, `Reserva.ConfirmarPago()`), no en servicios.
- **CQRS con MediatR** — cada caso de uso es una clase aislada (Command/Query + Handler). Agregar un nuevo RF no toca código existente.
- **Reglas en la entidad** — `DomainException` fluye hacia arriba y el `ExceptionMiddleware` la convierte en HTTP 422.
- **SQLite** — base de datos embebida, cero configuración de servidor, ideal para demos y pruebas.

### Frontend — Angular 19

eventosvivos-frontend/src/app/

├── core/

│   ├── models/        # Interfaces TypeScript

│   ├── services/      # HTTP services

│   └── guards/        # Auth guard

├── features/

│   ├── auth/          # Selección de rol

│   ├── eventos/       # Lista, crear, reporte

│   └── reservas/      # Crear, gestión

└── app.routes.ts      # Lazy loading

**Decisiones:**
- Standalone components con signals (Angular 19)
- Feature-based folder structure
- Lazy loading por ruta
- Interceptor HTTP para token de autenticación

---

##  Tecnologías

### Backend
| Tecnología | Versión | Uso |
|---|---|---|
| .NET | 10 | Framework principal |
| ASP.NET Core | 10 | API REST |
| Entity Framework Core | 10 | ORM |
| SQLite | — | Base de datos embebida |
| MediatR | 12 | CQRS / Mediator pattern |
| FluentValidation | 11 | Validación de DTOs |
| xUnit | 2.9 | Framework de testing |
| FluentAssertions | 8.4 | Assertions expresivas en tests |

### Frontend
| Tecnología | Versión | Uso |
|---|---|---|
| Angular | 19 | Framework SPA |
| RxJS | 7 | Programación reactiva |

### DevOps
| Tecnología | Uso |
|---|---|
| GitHub Actions | CI/CD pipelines |
| Railway | Despliegue backend |
| Vercel | Despliegue frontend |
| Docker | Containerización backend |

---

## Ejecutar localmente

### Prerequisitos
- .NET 10 SDK → https://dotnet.microsoft.com/download
- Node.js 22+ → https://nodejs.org
- Angular CLI → `npm install -g @angular/cli`

###  Clonar el repositorio

```bash
git clone https://github.com/EduardoTrujillo0410/eventosvivos.git
cd eventosvivos
```

###  Ejecutar el Backend

- API disponible en: `https://localhost:44393`

> La base de datos SQLite (`eventosvivos.db`) se crea automáticamente al arrancar con los 3 venues precargados.

###  Ejecutar el Frontend

```bash
cd eventosvivos-frontend
npm install
ng serve
```

- Aplicación disponible en: `http://localhost:4200`

> El proxy de Angular redirige `/api` a `https://localhost:44393` automáticamente. No se necesita configuración adicional.

---

##  Requerimientos implementados

### Funcionales
| RF | Descripción | Estado |
|---|---|---|
| RF-01 | Crear evento con validaciones completas | ✅ |
| RF-02 | Listar eventos con filtros opcionales y paginación | ✅ |
| RF-03 | Reservar entradas con validaciones de negocio | ✅ |
| RF-04 | Confirmar pago y generar código EV-XXXXXX único | ✅ |
| RF-05 | Cancelar reserva y liberar entradas | ✅ |
| RF-06 | Reporte de ocupación con ingresos y porcentaje | ✅ |

### Reglas de Negocio
| RN | Descripción | Estado |
|---|---|---|
| RN-01 | Capacidad del evento no excede la del venue | ✅ |
| RN-02 | No superposición de horarios en el mismo venue | ✅ |
| RN-03 | Eventos en fin de semana no inician después de las 22:00 | ✅ |
| RN-04 | No se permiten reservas con menos de 1 hora para el inicio | ✅ |
| RN-05 | Precio > $100 limita a máximo 10 entradas por transacción | ✅ |
| RN-06 | Evento se marca completado automáticamente al vencer | ✅ |
| RN-07 | Cancelación con penalización si faltan menos de 48 horas | ✅ |

---

##  Seguridad implementada

- **SQL Injection** — EF Core con LINQ parametrizado, nunca concatenación directa de strings en queries
- **Rate Limiting** — 100 req/min en endpoints generales, 10 req/min en reservas (por IP)
- **Paginación obligatoria** — máximo 50 registros por página con valores por defecto seguros
- **Validación de inputs** — FluentValidation en todos los commands con mensajes claros
- **CORS** — configurado explícitamente para el dominio del frontend en producción

---

##  CI/CD Pipeline

Push a main

│

▼

GitHub Actions CI

├──  Build & Test Backend

└──  Build Frontend

│

▼ si todo pasa

├──  Deploy Backend → Railway

└──  Deploy Frontend → Vercel

Ver pipelines: [GitHub Actions](https://github.com/EduardoTrujillo0410/eventosvivos/actions)

---

## Roles de usuario

| Rol | Permisos |
|---|---|
| **Administrador** | Crear eventos, confirmar pagos, ver reportes, gestionar reservas |
| **Cliente** | Ver eventos, hacer reservas, cancelar reservas |

> La selección de rol se realiza en la pantalla de inicio. No se requiere registro.

---

*Desarrollado por Eduardo Trujillo Santos — www.linkedin.com/in/eduardo-trujillo-santos-b5225327b
