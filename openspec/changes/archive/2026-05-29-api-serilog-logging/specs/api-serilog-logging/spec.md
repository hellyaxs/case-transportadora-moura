## ADDED Requirements

### Requirement: Serilog host bootstrap

The API host SHALL configure Serilog as the primary logging provider before the application builder runs, and SHALL flush logs on shutdown.

#### Scenario: Startup logging

- **WHEN** the API process starts
- **THEN** Serilog is initialized with a minimum level from configuration or environment
- **AND** startup failures (including migration errors) are written to configured sinks

#### Scenario: Graceful shutdown

- **WHEN** the API process terminates normally or with an unhandled exception after Serilog is configured
- **THEN** buffered log events are flushed before exit

### Requirement: Logging module composition

The API SHALL expose a `Logging` module registered through `AddLoggingModule` and applied in the HTTP pipeline through `UseLoggingModule` (or equivalent extension), consistent with other bounded context modules.

#### Scenario: Module registration

- **WHEN** `AddApiModules` is invoked during service configuration
- **THEN** logging services and Serilog configuration are registered without breaking existing Collections or Auth modules

#### Scenario: Pipeline integration

- **WHEN** the HTTP pipeline is built
- **THEN** request logging middleware runs for API requests after CORS and before endpoint execution

### Requirement: Configurable sinks and levels

Log output SHALL be configurable via `appsettings.json`, environment-specific settings, and environment variables without code changes.

#### Scenario: Console sink default

- **WHEN** no file path environment variable is set
- **THEN** logs are written to the console sink at the configured minimum level

#### Scenario: File sink in development

- **WHEN** `LOG_FILE_PATH` is set and the environment is Development
- **THEN** logs are additionally written to a rolling file at that path

#### Scenario: Override log level

- **WHEN** `LOG_LEVEL` is set to a valid Serilog level name
- **THEN** the global minimum level matches that value

### Requirement: Structured request logging

Each completed HTTP request SHALL emit a structured log entry including method, path, status code, and elapsed milliseconds.

#### Scenario: Successful API request

- **WHEN** a client calls a protected or public API endpoint and receives a response
- **THEN** a single request log entry is emitted with status code and duration

#### Scenario: Correlation identifier

- **WHEN** a request is processed
- **THEN** the log entry includes the ASP.NET Core trace identifier for correlation

### Requirement: Sensitive data exclusion

The logging system MUST NOT write authentication secrets, JWT tokens, passwords, or full connection strings to any sink.

#### Scenario: Login request

- **WHEN** a login attempt is processed
- **THEN** logs may record username or user id and success/failure
- **AND** MUST NOT record the password or raw token value

#### Scenario: Authorization header

- **WHEN** request logging runs for authenticated endpoints
- **THEN** the `Authorization` header and auth cookies are not logged

### Requirement: Business and error logging via ILogger

Application code in Collections and Auth modules SHALL use `ILogger<T>` for operational events; Serilog SHALL receive those events through the Microsoft logging bridge.

#### Scenario: Authentication failure

- **WHEN** login fails due to invalid credentials
- **THEN** a warning-level structured log is written without sensitive fields

#### Scenario: Unexpected failure

- **WHEN** an unhandled exception escapes a use case
- **THEN** an error-level log is written with exception type and message before the HTTP error response

### Requirement: Documentation and local environment

Architecture documentation and API environment examples SHALL describe logging variables and conventions in English for technical keys.

#### Scenario: Developer onboarding

- **WHEN** a developer reads `doc/arquitetura.md` and the API `.env.example`
- **THEN** they find documented `LOG_LEVEL`, optional `LOG_FILE_PATH`, and the Logging module layout
