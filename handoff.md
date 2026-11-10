# Words of Life handoff

**Updated:** 11 November 2026
**Repository:** https://github.com/mwamba2026/Word-of-life
**Current source commit:** `bb96ffb` (10 November 2026)

## Current state

The complete improved source is on the repository's `main` branch. The Vue 3/Vite app is in `frontend/`; the ASP.NET Core 8 API is in `backend/FaithOutreach.Api/`. In particular, `frontend/index.html` loads `frontend/src/main.js`, and that file plus its imports are committed. The earlier incomplete root-level app files and unrelated .NET desktop workflow were replaced. `LICENSE` was retained.

The repository is public. Uploading the source did **not** deploy the working website. GitHub Pages cannot run this .NET API or its MySQL-backed accounts, notes, forms, content editing, and uploads. Use a host that runs the published ASP.NET Core app, with HTTPS, persistent storage, and MySQL. See `README.md` and `HOSTING.md`.

## Verification completed

- Copied all 46 frontend and backend source files from `C:\Users\User\Desktop\Words of Life Improved` into a separate clone; no source file was missing. Four files differ only by end-of-file whitespace cleanup.
- `npm ci` and `npm run build` passed in the clean clone; Vite transformed 64 modules.
- `dotnet build backend/FaithOutreach.Api/FaithOutreach.Api.csproj -c Release` passed with zero warnings and errors.
- `git diff --check` passed. The commit contains no database files, generated build output, or common private-key/token formats. It includes `.github/workflows/verify.yml` to build both projects on future pushes and pull requests. The remote workflow result was not checked.
- After the push, remote `main` resolved to `bb96ffb`, and Git confirmed `frontend/src/main.js`, `backend/FaithOutreach.Api/Program.cs`, and the verification workflow are present.

## Remaining before public site launch

1. Upgrade the ASP.NET Core app from .NET 8 to a supported release before permanent public deployment. Microsoft ended .NET 8 support on 10 November 2026: https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core. Choose and provision hosting for the app and a reachable MySQL 8 database. Keep credentials in host environment settings, not the repository. The app currently creates its schema with `EnsureCreated`; introduce EF migrations before storing durable production data.
2. Configure HTTPS, the exact `AllowedHosts`, a trusted reverse proxy, persistent data-protection keys, and `Media__StoragePath` outside the deploy directory. Back up MySQL and media files. Configure request limits for permitted uploads.
3. Set up the administrator account before opening registration, assign the admin role with the one-time bootstrap key, and remove that key afterward. Administrator email verification/MFA and media malware scanning are not implemented.
4. Run end-to-end tests on the actual host and MySQL: health, registration, sign-in, private notes, feedback, prayer/support, staff publishing, upload, and playback. The 1 November handoff reports passing local MySQL form tests; online staging and browser media playback were not verified.
5. Arrange a private staff process for reviewing prayer/support requests; the app stores them but has no automatic alert or protected inbox. Confirm public contact, donation details, privacy terms, and rights for any hosted media.
6. Review frontend dependencies. `npm audit --omit=dev` reported a high-severity `source-map-js` advisory on 10 November; the attempted audit-fix preview did not complete. Reassess and update the lockfile safely before production deployment.

## Local source and prior notes

The Desktop working copy is `C:\Users\User\Desktop\Words of Life Improved`, with `Words of Life Improved.zip` as its original package. The ZIP and Desktop source files were unchanged by the GitHub sync; `frontend/dist` was regenerated during verification. `session2 handoff.md` in that working copy, dated 1 November 2026, explains feature and security details and supersedes the older 9 October `session_handoff.md`. This file supersedes them for repository and launch status.

The GitHub sync was performed through a separate temporary clone using the repository owner account.
