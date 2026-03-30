# RedGreenBlue

A small training project focused on practicing authentication, authorization, and frontend-backend interaction using a .NET 8 Web API and Angular 19.

## Overview

Users are divided into three groups: **Red**, **Green**, and **Blue**.  
There are 3 seperate color-grids (red, green, blue). Each user can view every grid but only interact with hexes of their own color — and only within their specific color spectrum.  
An **admin** user has full access to modify all hexes.

The frontend features a color-selectable **hexagon grid** built with https://abbekeultjes.nl/honeycomb/guide/creating-grids.html

---

## Tech Stack

- **.NET 8** – Web API backend
- **Angular 19** – Frontend
- **Honeycomb Grid** – Hexagon rendering
- **SQLite** - Simple SQL DB

## Database Startup Behavior (Dev vs Container/Prod)

The app now supports a practical startup strategy:

- **Development default:** migrations + seed run automatically.
- **Production/container default:** no automatic migration/seed unless explicitly enabled.

Use environment variables:

```bash
# Enable migration at startup
Database__RunMigrationsAtStartup=true

# Enable seeding at startup
Database__SeedAtStartup=true
```

### Default Seed Users (created only if missing)

- `Admin / pw123` (Admin, Team Red)
- `RedUser / pw123`
- `GreenUser / pw123`
- `BlueUser / pw123`

Notes:
- Seeding is idempotent (existing usernames are not recreated).
- Seeded grids are always fixed to `100 x 100` per team.
- For real production, default passwords must be replaced.

---

> This is an experimental project for learning and demonstration purposes.
