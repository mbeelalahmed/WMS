# WMS 

## Overview

This is a .NET Core Web API for integrating with the GAC Warehouse Management System (WMS).
The solution is built using Clean Architecture, MediatR, and minimal dependencies, ensuring separation of concerns and high testability.

 - Setup database connection string via `appsettings.json`
 - Run ```dotnet ef migrations add InitialCreate```
 - Run ```dotnet ef database update```

# LegacyFilePollingJob

## Overview

LegacyFilePollingJob is a .NET background service that periodically scans a folder for legacy XML files containing purchase orders. It parses each file, processes the data, and sends it to an external WMS (Warehouse Management System). Successfully processed files are renamed to avoid duplication.

## Features

- Polls a directory on a configurable schedule
- Parses XML purchase order files
- Pushes parsed data to a WMS service
- Handles errors with retry logic
- Supports parallel file processing
- Configurable via `appsettings.json`

## Configuration

All settings are defined in `appsettings.json` under the `FileIntegration` section:

```json
{
  "FileIntegration": {
    "InputFolderPath": "<give address of file location>",
    "PollingCronExpression": "*/10 * * * * *",
    "WmsBaseUrl": "https://localhost:7016",
    "PurchaseOrdersEndpoint": "/api/PurchaseOrders"
  }
}
```

## Startup Project

- Select both projects as start up projects and run
