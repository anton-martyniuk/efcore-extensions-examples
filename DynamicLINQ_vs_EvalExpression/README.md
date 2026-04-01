# DynamicLINQ vs Eval Expression Examples

## Prerequisites
- .NET 10.0 SDK
- IDE (Visual Studio, JetBrains Rider or Visual Studio Code) or .NET CLI

## Projects Overview

- **DynamicLinq.WebApi** — Web API demonstrating dynamic LINQ queries
- **EvalExpression.WebApi** — Web API demonstrating Eval expression usage
- **EvalExpression.Console** — Console application demonstrating Eval expression usage

---

## DynamicLINQ Web API

### 1. Run the Application

#### Option A: Using Visual Studio or Rider
1. Open `DynamicLINQ_vs_EvalExpression.sln` in your IDE
2. Set `DynamicLinq.WebApi` as the startup project
3. Press `F5` or click the Run button
4. The browser will automatically open with Swagger UI

#### Option B: Using .NET CLI
Navigate to the project directory and run:

```bash
cd DynamicLINQ_vs_EvalExpression/DynamicLinq.WebApi
dotnet run
```

### 2. Access the Application

The application will be available at:
- **HTTPS**: https://localhost:7092/swagger
- **HTTP**: http://localhost:5113/swagger

When running from Visual Studio or Rider, the browser will automatically open to the Swagger page.

### 3. Verify the Application is Running

Check the console logs for messages indicating a successful startup:
- Look for "Now listening on: http://localhost:5113" or "Now listening on: https://localhost:7092"
- Verify no error messages appear during startup

### 4. Test the Endpoints

#### Using Swagger UI
1. Navigate to the Swagger page (automatically opened or manually visit the URLs above)
2. Expand the available endpoints
3. Click "Try it out" on any endpoint
4. Fill in the required parameters
5. Click "Execute" to test the endpoint

#### Using HTTP Request Files
The project includes a `requests.http` file in the `DynamicLinq.WebApi` directory with pre-defined requests:
1. Open `DynamicLinq.WebApi/requests.http` in your IDE
2. **Visual Studio**: Click the green play button next to any request
3. **JetBrains Rider**: Click the green play button next to any request
4. **Visual Studio Code**: Install the [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension, then click "Send Request" above any request
5. View the response in the IDE's built-in HTTP response panel

#### Using Postman
1. Import or manually create requests for the available endpoints
2. Set the base URL to `http://localhost:5113` or `https://localhost:7092`
3. Send requests and verify responses

---

## Eval Expression Web API

### 1. Run the Application

#### Option A: Using Visual Studio or Rider
1. Open `DynamicLINQ_vs_EvalExpression.sln` in your IDE
2. Set `EvalExpression.WebApi` as the startup project
3. Press `F5` or click the Run button
4. The browser will automatically open with Swagger UI

#### Option B: Using .NET CLI
Navigate to the project directory and run:

```bash
cd DynamicLINQ_vs_EvalExpression/EvalExpression.WebApi
dotnet run
```

### 2. Access the Application

The application will be available at:
- **HTTPS**: https://localhost:7092/swagger
- **HTTP**: http://localhost:5113/swagger

When running from Visual Studio or Rider, the browser will automatically open to the Swagger page.

### 3. Verify the Application is Running

Check the console logs for messages indicating a successful startup:
- Look for "Now listening on: http://localhost:5113" or "Now listening on: https://localhost:7092"
- Verify no error messages appear during startup

### 4. Test the Endpoints

#### Using Swagger UI
1. Navigate to the Swagger page (automatically opened or manually visit the URLs above)
2. Expand the available endpoints
3. Click "Try it out" on any endpoint
4. Fill in the required parameters
5. Click "Execute" to test the endpoint

#### Using HTTP Request Files
The project includes a `requests.http` file in the `EvalExpression.WebApi` directory with pre-defined requests:
1. Open `EvalExpression.WebApi/requests.http` in your IDE
2. **Visual Studio**: Click the green play button next to any request
3. **JetBrains Rider**: Click the green play button next to any request
4. **Visual Studio Code**: Install the [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension, then click "Send Request" above any request
5. View the response in the IDE's built-in HTTP response panel

#### Using Postman
1. Import or manually create requests for the available endpoints
2. Set the base URL to `http://localhost:5113` or `https://localhost:7092`
3. Send requests and verify responses

---

## Eval Expression Console

### 1. Run the Application

#### Option A: Using Visual Studio or Rider
1. Open `DynamicLINQ_vs_EvalExpression.sln` in your IDE
2. Set `EvalExpression.Console` as the startup project
3. Press `F5` or click the Run button
4. The console output will appear in the terminal window

#### Option B: Using .NET CLI
Navigate to the project directory and run:

```bash
cd DynamicLINQ_vs_EvalExpression/EvalExpression.Console
dotnet run
```

### 2. Verify the Application is Running

Check the console output for:
- Generated email template results printed to the console
- Verify no error messages appear during execution

## Troubleshooting

- If the application fails to start, check the console logs for error messages
- Check that port 7092 (HTTPS) and 5113 (HTTP) are not being used by other applications
- Ensure the correct startup project is selected when running from an IDE
