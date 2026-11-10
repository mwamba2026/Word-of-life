# Words of Life

A full-stack Christian outreach ministry website built with Vue and ASP.NET Core. The application stores accounts, feedback, support requests, and content in MySQL through the .NET API.

## Structure

- `frontend/` — Vue 3 + Vite app
- `backend/FaithOutreach.Api/` — ASP.NET Core Web API

## Start the application

Double-click `Start Words of Life.cmd`. It creates the deployment package if needed, starts the complete application on `http://localhost:5000`, and opens the website in your browser. If the application is already running, it simply opens it.

Before starting, set `ConnectionStrings__DefaultConnection` to a MySQL connection string. The API creates its tables in the selected database on first start.

## Manual start

### Frontend

```bash
cd frontend
npm.cmd install
npm run dev
```

### Backend

```bash
cd backend/FaithOutreach.Api
dotnet restore
dotnet run --urls http://localhost:5000
```

Open `http://localhost:5173`. The Vite development server forwards `/api` requests to the backend.

## Operational checks

- API health: `http://localhost:5000/api/health`
- API documentation in development: `http://localhost:5000/swagger`
- Production frontend build: run `npm.cmd run build` inside `frontend/`
- Backend build: run `dotnet build` inside `backend/FaithOutreach.Api/`

## Publish and host

Double-click `Publish Words of Life.cmd` to create a single deployable application in `publish/`. See `HOSTING.md` for server requirements, database persistence, deployment options, and the production checklist.


## Content and design notes

- The home and Bible pages share seven KJV passages that rotate by each visitor's local calendar day. The cycle repeats; this is not a newly published daily devotional.
- Watch links lead to credited programs on Amazing Facts. That publisher controls playback and availability.
- Hymns are public-domain words for reading and singing; no audio recordings are included.
- The prayer form stores requests in MySQL. The app does not email a team or provide a staff inbox. Arrange a secure review process before inviting public submissions. No personal reply is guaranteed.
- Blue marks navigation and primary actions (obedience), gold marks faithful continuation and reflection (loyalty), and white gives calm space for reading and prayer (peace).
- Headings use Fraunces and body text uses Source Sans 3, with local fallbacks when Google Fonts is unavailable.
- News and events were not added because no verified events or publishing schedule were supplied.

The older root-level static prototype pages were removed from this improved copy. Start the Vue/.NET app with `Start Words of Life.cmd`.


## MySQL configuration

Use MySQL 8.0 or 8.4. Create an empty database and a dedicated user with access to it. Set the connection string as an environment variable, not in a committed file:

`ConnectionStrings__DefaultConnection=Server=YOUR_HOST;Port=3306;Database=words_of_life;User=YOUR_USER;Password=YOUR_PASSWORD;`

For a server version other than 8.0.36, set `Database__MySqlVersion` to its version (for example `8.4.0`). Then start `publish/FaithOutreach.Api.exe`. The existing SQLite file is kept in `database-backup/` as a backup; its records have not been migrated to MySQL.


## Android, iOS, and desktop

The Vue site is responsive and installable as a progressive web app over HTTPS. On Android or desktop, use the browser's Install action. On iPhone/iPad in Safari, use Share → Add to Home Screen. The installed shell and previously visited static assets can load offline; accounts, notes, and forms need the .NET API and MySQL connection. This package does not include separate Play Store or App Store binaries.

The Listen section links to Amazing Facts Christian audiobooks with attribution. The audio files are not copied into this project.

The Read/Study page links to three featured Amazing Facts Bible Study Guides and the full 27-lesson catalog. Their text remains on the publisher site.

## Staff content and account notes

The Vue sections load public resources from `GET /api/resources`, with the bundled catalog as a fallback for a frontend-only preview. A designated administrator can open `/staff/content` to add, edit, publish, and remove readings, teaching links, audiobook links, study-guide links, hymns, and reflections. The editor accepts text, HTTPS links, and permitted audio/video uploads. Keep external media on the original publisher site unless redistribution rights are granted.

Set `Admin__Email` and a strong `Admin__BootstrapKey` on the server. Register and sign in as that email, visit `/staff/content`, enter the bootstrap key once, then sign in again to use the editor. Remove the bootstrap key from the server environment after setup. The backend does not verify email ownership, so create the designated account before opening public registration.

Notes are private to the signed-in account and stored through the API. Older notes kept only in a browser's local storage are not automatically imported into a MySQL account.

The API can override the About mission with `Site__Mission` and the giving details with `Site__Donation__MpesaPaybill`, `Site__Donation__MpesaAccount`, `Site__Donation__BankName`, `Site__Donation__BankAccountName`, `Site__Donation__BankAccountNumber`, and `Site__Donation__ContactEmail`. Empty payment details stay hidden.

## Audio, video, and podcasts

The administrator content editor now accepts MP3/M4A audio (up to 50 MB) and MP4/WebM video (up to 250 MB) for teaching and podcast entries; audiobooks accept audio. Files are uploaded to `POST /api/media` and served at `/media/...` with browser playback. A new Podcasts section displays published episodes. The media endpoint requires the Admin role. Publisher links remain available as an alternative. Upload only recordings you have permission to publish.

Set `Media__StoragePath` to a persistent directory outside the deployed `publish` folder, writable by the API process. Without it, files are stored in `publish/media` (or the active API content root) and may be lost if that directory is replaced during deployment. MySQL stores the content entry and media URL; media bytes stay on disk. Back up both. Deleting a content entry does not automatically remove its media file.

## Private staging security

Registration is closed unless `Registration__InviteCode` is set on the server. Give this code only to designated reviewers, and create the administrator account first. The backend rate-limits login, registration, public forms, and uploads; repeated failed passwords lock an account for 15 minutes. Cookie-backed API writes use an anti-forgery token. In non-Development environments, cookies require HTTPS. Set `AllowedHosts` to the exact review domain; the default permits only localhost. The browser and API should share one origin.

Uploaded media is playable only while a published content entry references it. Unpublishing or deleting the entry removes public access, but the file remains on disk for later cleanup. Store media outside the published directory. Staging still needs a private access gate at the hosting layer, a trusted HTTPS reverse proxy, and live MySQL security tests before sharing a link.
