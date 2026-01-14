# EF Core Extensions Examples

## Prerequisites
- .NET 10.0 SDK
- SQL Server (local, docker or remote instance)
- IDE (Visual Studio, JetBrains Rider or Visual Studio Code) or .NET CLI

## Resources

### 1. Identity, AutoMapOutputDirection options
- [Detailed on Insert Identity](https://entityframework-extensions.net/identity)
- [List of Identity options](https://entityframework-extensions.net/bulk-extensions)
- [Insert Keep Identity](https://entityframework-extensions.net/insert-keep-identity)

### 2. BatchSize, BatchTimeout and BatchDelayInterval options
- [Batch Options](https://entityframework-extensions.net/batch)

### 3. AutoTruncate option
- [Bulk Insert](https://entityframework-extensions.net/bulk-insert)

### 4. Primary Key / Input / Output / Ignore options for different methods
- [Primary Key](https://entityframework-extensions.net/primary-key)
- [Input Output Ignore](https://entityframework-extensions.net/input-output-ignore)

### 5. Coalesce / Coalesce Destination Options
- [Coalesce](https://entityframework-extensions.net/coalesce)
- [Coalesce Destination](https://entityframework-extensions.net/coalesce-destination)

### 6. Matched and Condition Option
- [Matched and Condition](https://entityframework-extensions.net/matched-and-condition)
- [Matched and One Not Condition](https://entityframework-extensions.net/matched-and-one-not-condition)

### 7. IncludeGraph and IncludeGraphBuilder options
- [Include Graph](https://entityframework-extensions.net/include-graph)

### 8. Events in Entity Framework Extensions
- [Events](https://entityframework-extensions.net/events)

### 9. Audit options
- [Audit](https://entityframework-extensions.net/audit)

### 10. Log options
- [Logging](https://entityframework-extensions.net/logging)

### 11. RowsAffected option
- [Rows Affected](https://entityframework-extensions.net/rows-affected)

### 12. Provider-Specific Options
- [Bulk Insert - Providers Specific](https://entityframework-extensions.net/bulk-insert)

### 13. Licensing information
- [Pricing](https://entityframework-extensions.net/pricing)

## Getting Started

### 1. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```


Replace the following placeholders:
- `YOUR_SERVER` - Your SQL Server instance address (e.g., `localhost`, `127.0.0.1,1433`)
- `YOUR_DATABASE` - Database name
- `YOUR_USER` - SQL Server username
- `YOUR_PASSWORD` - SQL Server password
- Include `TrustServerCertificate` as needed

### 2. Run the Application

#### Option A: Using Visual Studio or Rider
1. Open the solution in your IDE
2. Press `F5` or click the Run button
3. The browser will automatically open with Swagger UI

#### Option B: Using .NET CLI
Navigate to the project directory and run:

```bash
dotnet run
```


### 3. Access the Application

The application will be available at:
- **HTTPS**: https://localhost:5000/swagger
- **HTTP**: http://localhost:5001/swagger

When running from Visual Studio or Rider, the browser will automatically open to the Swagger page.

### 4. Verify the Application is Running

Check the console logs for messages indicating a successful startup:
- Look for "Now listening on: http://localhost:5001" or "Now listening on: https://localhost:5000"
- Verify no error messages appear during startup
- Confirm database connection is established

### 5. Test the Endpoints

#### Using Swagger UI
1. Navigate to the Swagger page (automatically opened or manually visit the URLs above)
2. Expand the available endpoints
3. Click "Try it out" on any endpoint
4. Fill in the required parameters
5. Click "Execute" to test the endpoint

#### Using Postman
1. Import or manually create requests for the available endpoints
2. Set the base URL to `http://localhost:5001` or `https://localhost:5000`
3. Send requests and verify responses

## Troubleshooting

- If the application fails to start, check the console logs for error messages
- Verify your SQL Server instance is running and accessible
- Ensure the connection string is correctly configured
- Check that port 5000 (HTTPS) and 5001 (HTTP) are not being used by other applications

## Running Benchmarks

This repo includes a BenchmarkDotNet project that measures EF Core vs bulk operations.

- Project path: `ReturningIdentityValue/Benchmarks`
- Framework: `net9.0`

### 1. Configure the database connection

Update the connection string in `ReturningIdentityValue/Benchmarks/appsettings.json` (and optionally `appsettings.Development.json`) to point to your SQL Server:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

Make sure the target database is reachable and the user has permissions to create tables and insert data.

### 2. Build (Release)

From the repository root:

```bash
dotnet build -c Release
```

### 3. Run the benchmarks (Release)

From the repository root, run the Benchmarks project in Release configuration:

```bash
# Run all benchmarks
dotnet run -c Release --project ReturningIdentityValue/Benchmarks
```

BenchmarkDotNet places results under `ReturningIdentityValue/Benchmarks/BenchmarkDotNet.Artifacts/results` as `.md` and `.html` reports.

### Tips for stable results
- Close other heavy applications; keep the machine idle during runs.
- Prefer running on AC power and a performance power plan.
- Use the same SQL Server instance for all runs; ensure it's not under external load.
- Results are comparative; look at ratios between baselines and alternatives.
