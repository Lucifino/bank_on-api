# bank_on-api

frontend deployment:
https://kind-sand-099505e10.2.azurestaticapps.net/admin

backend deployment:
https://bankon-api.azurewebsites.net

graphql layer:
https://bankon-api.azurewebsites.net/graphql

api layer:
https://bankon-api.azurewebsites.net/api

exposed api:
https://bankon-api.azurewebsites.net/api/BankOn/SaveFinanceRequestAndRedirect

local frontend:
http://localhost:4200/admin

local backend:
https://localhost:7177
https://localhost:5177

local graph layer:
https://localhost:7177/graphql/a
https://localhost:5177/graphql/

local swagger:
https://localhost:7177/swagger/index.html
https://localhost:5177/swagger/index.html

Folders:

Models
Broken down into Classes and Entities. Entities are the base objects of the solution and follow the entity framework design. They are scaffolded from the db.
Classes are objects used as necessary inputs or outputs but have no effect on the database or the entities.

Migrations
Allows scaffolding from code to db to set up the db for local deployment

Helpers
CorsMiddleware (unused) to allow any origin, header, or method. FileContentResultTypeAttribute(unused), for resolving content types passing through controllers.
GraphQlNamingConvention, used to set the contracts for graphql to use pascal case. NodaTimeClockService, flexible way of dealing with date and time data types.

GraphQL
Contains the queries and mutations for the solution.

Controllers
Shows controllers for REST functionality. Exposes only one api as required by the specs for the consumption of third parties.

Adapters
Forms the logic for the controllers. Data is passed through files in the adapter.cs and then returned to the controllers.
