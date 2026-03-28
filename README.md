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

## Local JWT Secret Setup

JWT signing key is not stored in `appsettings.json`.
Set it once locally via .NET User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "dev-jwt-key-please-change-later-2026"
```

---

> This is an experimental project for learning and demonstration purposes.
