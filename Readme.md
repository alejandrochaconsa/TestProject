# File & Directory Browser

This app is to enable file browsing, search, upload and download directly from your favorite browser.

## Stack
- Backend: .Net Core 8 API
- Frontend: Vanilla.js

## Running locally
1. `dotnet dev-certs https --trust` (one-time)
2. `dotnet run`
3. Open `https://localhost:7146`

## Configuration
The storage directory is configurable in `appsettings.json` under `FileStorage:Directory`. Defaults to `Storage/` at the project root.

## API Endpoints
- `GET /api/v1/items?path=` — list all contents of a directory
- `GET /api/v1/items/search?path=&query=` — search files and folders 
- `GET /api/v1/items/download?path=` — download a file
- `POST /api/v1/items/upload?path=` — upload a file
- `DELETE /api/v1/items?path=` — delete a file

A Postman collection is included at the repo Docs folder for testing.

## UI Look and Feel

![alt text](<Docs/Screenshot 2026-04-27 at 10.51.47 AM.png>)
