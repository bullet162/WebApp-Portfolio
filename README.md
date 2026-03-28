# WebApp Portfolio

A personal portfolio web app built with **Blazor Server**, **Entity Framework Core**, and **SQLite**.

## Features

- Hero section with avatar, bio, and resume link
- Experience timeline
- Projects showcase
- Interactive tech stack constellation — skills grouped by system, connected by glowing lines
- GitHub integration — pinned repos, all public repos, contribution graph, streaks
- Contact / hiring message form
- Admin panel — manage profile, skills, projects, experiences, messages, and security settings
- Password reset via email (SMTP)

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | ASP.NET Core / Blazor Server (.NET 10) |
| ORM | Entity Framework Core |
| Database | SQLite |
| Styling | Custom CSS (dark theme) |
| Auth | Custom session-based admin auth |
| Email | SMTP via `EmailService` |
| GitHub | GitHub GraphQL + REST API |

## Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/bullet162/WebApp-Portfolio.git
cd WebApp-Portfolio/BlazorPortfolio
```

### 2. Configure settings

Copy the example config and fill in your values:

```bash
cp appsettings.example.json appsettings.json
```

Key fields in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=portfolio.db"
  },
  "AdminEmail": "your@email.com",
  "Smtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "User": "your@email.com",
    "Pass": "yourpassword"
  },
  "GitHub": {
    "Token": "your_github_token"
  }
}
```

### 3. Apply migrations and run

```bash
dotnet ef database update
dotnet run
```

The app will be available at `https://localhost:5001`.

## Admin Panel

Navigate to `/admin/login` to access the admin dashboard where you can manage all portfolio content.

## License

MIT
