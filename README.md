# Netflix Clone Auth Service

A microservice for handling user authentication in the Netflix-clone project. This service provides secure user registration, login, and token management using JWT and refresh tokens.

## Features

- **User Registration**: Register new users with email, display name, and password. Passwords are securely hashed.
- **User Login**: Authenticate users via email and password, returning access and refresh tokens.
- **JWT Token Management**: Issues short-lived access tokens and long-lived refresh tokens for secure API access.
- **Refresh Token Persistence**: Stores refresh token identifiers (JTI) in a response cache (e.g., Redis) to manage token lifecycle.
- **Idempotent Operations**: Ensures request idempotency using request IDs.

## Tech Stack

- **Framework**: .NET 8 (ASP.NET Core)
- **Architecture**: Clean Architecture with CQRS (using MediatR)
- **Endpoints**: Carter for minimal API routing
- **Database**: Entity Framework Core (for migrations and some operations) and Dapper (for efficient queries) with RDBMS (e.g., SQL Server)
- **Caching**: Response cache service (Redis integration)
- **Authentication**: JWT (JSON Web Tokens)
- **Password Hashing**: Secure password hashing service
- **Validation**: FluentValidation for request validation
- **Logging & Middleware**: Custom middleware for request ID, Swagger, etc.

## Configuration

The service uses `appsettings.json` for configuration. Key sections include:

- **DatabaseSettings**: Connection string for the database.
- **AuthSettings**: JWT configuration (issuer, audience, secrets, token expiry).
- **RedisSettings**: Redis connection settings.

Example `appsettings.json`:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "Server=localhost;Database=NetflixCloneAuth;Trusted_Connection=True;"
  },
  "AuthSettings": {
    "Issuer": "netflix-clone",
    "Audience": "netflix-clone",
    "AccessSecretToken": "your-long-random-access-secret-key-here",
    "RefreshSecretToken": "your-long-random-refresh-secret-key-here",
    "AccessTokenExpMinute": 15,
    "RefreshTokenExpMinute": 43200
  },
  "RedisSettings": {
    "ConnectionString": "localhost:6379"
  }
}
```
