# Hosting Words of Life with MySQL

The published .NET 8 app serves the built Vue site and API from one address.

1. Provision MySQL 8.0 or 8.4 and create an empty database plus a dedicated user.
2. Set `ConnectionStrings__DefaultConnection` in the host's secret/environment settings. Example format: `Server=HOST;Port=3306;Database=words_of_life;User=USER;Password=PASSWORD;`.
3. Set `Database__MySqlVersion` if the server is not 8.0.36.
4. Run `Publish Words of Life.cmd`, deploy the contents of `publish`, and configure HTTPS and the listening port.
5. Check `/api/health`, then test registration, sign-in, notes, feedback, and support requests against the MySQL database.

The API creates its schema on first start. Back up MySQL regularly. The old SQLite data remains in `database-backup/faithoutreach.sqlite.db` and was not migrated. The prayer form saves requests but has no automatic staff alert or inbox; set up a secure review process before inviting public submissions.

For audio/video uploads, configure `Media__StoragePath` as a persistent writable directory outside `publish`, and back it up with MySQL. The upload limit is 50 MB for MP3/M4A and 250 MB for MP4/WebM; your reverse proxy must permit the same request size. Uploaded files are served at `/media/...`.

## Private staging access and security

Set `ASPNETCORE_ENVIRONMENT=Staging`, `AllowedHosts` to the exact staging host, and serve HTTPS through a trusted reverse proxy. Do not expose Kestrel or MySQL directly to the internet. Require a separate private review gate (for example, VPN or reverse-proxy authentication) before sharing the link. Set `Registration__InviteCode` only for invited reviewers, create the admin account first, and remove `Admin__BootstrapKey` after assigning the first admin. Protect environment secrets and persistent media/key storage; back them up.

The app now rate-limits common write endpoints and requires anti-forgery tokens. These controls need end-to-end testing against a live staging MySQL server. `EnsureCreated` remains in `Program.cs`, so later schema changes are not migration-safe; replace it with EF migrations before durable production data. The Windows publish script creates Windows output; publish separately for a Linux host.
