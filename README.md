# EF Core Extensions Examples

## Prerequisites
- .NET 9.0 SDK
- SQL Server (local, docker or remote instance)
- IDE (Visual Studio, JetBrains Rider or Visual Studio Code) or .NET CLI

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