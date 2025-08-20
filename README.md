# KittyWorks.Carcassone

Initial hotseat prototype of the board game **Carcassone** built with
ASP.NET Core 9, Razor Pages, Bootstrap and vanilla JavaScript. Game
state is stored using EF Core 9's in-memory provider.

## Features

- Two player hotseat play.
- Each turn posts back to the server to place the next tile.
- Very small tile set with minimal rule checking (adjacent placement).

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/).

## Run

```bash
dotnet restore
dotnet run
```

Then open `http://localhost:5000` (or the URL shown in the console) in a
browser, enter two player names and start placing tiles.

## Development Notes

- Dependencies are managed with NuGet and pinned to stable releases (EF Core InMemory 9.0.8).
- No favicon is included because binary assets are not stored in this repository.
