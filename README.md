# Consilium Student Planner

A comprehensive student planning application with assignment tracking, todo lists, messaging, and more.

## Authentication

This application uses **Google OAuth 2.0** for authentication. Users sign in with their Google accounts for secure, seamless access.

## Quick Start

**See [QUICKSTART.md](QUICKSTART.md) for a 5-minute setup guide.**

1. Get Google OAuth credentials from [Google Cloud Console](https://console.cloud.google.com/)
2. Configure `.env` file with your credentials
3. Run `docker-compose up -d`
4. Open http://localhost:5173
5. Sign in with Google


## Technology Stack

### Backend
- ASP.NET Core 9.0 (C#)
- PostgreSQL Database
- Google.Apis.Auth for OAuth validation
- Dapper for database access

### Frontend
- React + TypeScript
- Vite build tool
- Google Sign-In integration
- Axios for API calls

### Infrastructure
- Docker & Docker Compose
- Environment-based configuration

## Features

- ✅ Google OAuth authentication
- ✅ Course and assignment management
- ✅ Todo list with nested items
- ✅ Inter-user messaging
- ✅ Profile management with Google profile pictures
- ✅ Real-time data synchronization

## Development

### Prerequisites
- Docker and Docker Compose
- Google OAuth credentials
- Modern web browser

### Running Locally

```bash
# Clone the repository
git clone <repository-url>
cd consilium-student-planner-app

# Configure environment
cp .env.example .env
# Edit .env with your Google OAuth credentials

# Start services
docker-compose up -d

# View logs
docker-compose logs -f
```

### Services

- **Web UI**: http://localhost:5173
- **API**: http://localhost:5202
- **Database**: localhost:5432

### Stopping Services

```bash
docker-compose down
```

## Security

- Google OAuth 2.0 authentication
- Server-side ID token validation
- Environment-based secret management
- Secure session handling
