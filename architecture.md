# Faith Outreach Architecture

FAITH OUTREACH

- Frontend: Vue.js
- Backend: ASP.NET Core
- Data Access: Entity Framework Core
- Database: SQL Server

## Layer flow

Frontend (Vue.js)
  - Songs
  - Sermons
  - Bible
  - Write-ups
  - Notes
  - Support
  - Feedback
  - Donate

Backend (ASP.NET Core)
  - Services
  - API endpoints

Entity Framework
  - Maps application models to SQL Server

SQL Server
  - User Data
  - Content Data
  - Notes
  - Songs / Sermons / Bible data

## Project direction

The app is structured as a standard Microsoft stack workflow:

Vue frontend -> ASP.NET Core API -> Entity Framework -> SQL Server

This keeps the app easy to extend while supporting user authentication, content management, and ministry resources.
