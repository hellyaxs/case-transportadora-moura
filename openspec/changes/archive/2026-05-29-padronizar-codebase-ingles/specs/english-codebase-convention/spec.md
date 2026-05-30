## ADDED Requirements

### Requirement: Code identifiers must be written in English

All source code identifiers (classes, interfaces, records, enums, methods, properties, parameters, namespaces, files and folders for code) SHALL use English names.

#### Scenario: New backend class created

- **WHEN** a developer adds a new C# type under `apps/api/src`
- **THEN** the type name and file name are written in English

#### Scenario: New frontend module file created

- **WHEN** a developer adds a new TypeScript file under `apps/web/src`
- **THEN** the file name and exported symbols use English naming

### Requirement: Database schema uses English table and column names

The relational schema SHALL use English names for tables and columns exposed by EF Core mappings.

#### Scenario: Collection entity persisted

- **WHEN** the collections module is mapped to PostgreSQL
- **THEN** table and column names use English identifiers such as `Collections`, `Customers`, `ExpectedPickupDate`

### Requirement: HTTP contracts use English routes and JSON fields

Public REST routes and JSON property names SHALL use English naming aligned with the official glossary.

#### Scenario: Collection list endpoint

- **WHEN** a client calls the collection listing endpoint
- **THEN** the route uses `/api/collections` and response fields use English names such as `customerName` and `expectedPickupDate`

### Requirement: Technical error codes use English slugs

Machine-readable business error codes returned by the API SHALL use English snake_case identifiers.

#### Scenario: Missing customer on create

- **WHEN** a collection is created with an unknown customer id
- **THEN** the API returns an error code such as `customer_not_found`

### Requirement: Permanent Cursor rules enforce English in code

The repository SHALL include Cursor rules that require English for executable code while allowing Portuguese in OpenSpec specs and product documentation.

#### Scenario: Contributor adds code with Portuguese identifier

- **WHEN** a pull request introduces a Portuguese class or property name in source code
- **THEN** project rules flag the violation and expect English naming before merge

### Requirement: Portuguese remains allowed in specs and docs

OpenSpec artifacts under `openspec/` and product documentation under `doc/` MAY remain written in Portuguese.

#### Scenario: Product spec updated

- **WHEN** a product requirement is documented in `openspec/specs`
- **THEN** it may describe business concepts in Portuguese while referencing English technical terms in a glossary when needed
