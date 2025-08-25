# DeliveryDatePlanning
**I have permission to publish this code**

The project consists of two executable services: `BackgroundService` and `WebApi`.

*The purpose of the `BackgroundService`* is to calculate the delivery dates and status of invoices.

*The purpose of the `WebApi` service* is to provide methods for retrieving information about the calculation results.

# Implementation Features
Domain models are always kept in a valid state.  
RabbitMQ is used for sending and receiving messages.  
Application configuration is managed via Consul.  
Calculation results are stored in MongoDB.  
Collection indexing in MongoDB is configured from code at the start of the `BackgroundService`.

# General Application Workflow

On its first launch, the application creates indexes in the MongoDB documents collection.  
Then, the application connects to RMQ to perform the following tasks:
1. Calculate delivery dates and status for invoices.
2. Maintain a history of status changes for each enclose.
3. Monitor day changes in the recipient's city to keep calculations up to date:
   1. The invoice has not yet been handled over by the sender.
   2. Status is updated.
   3. The number of overdue days is updated.

### Calculating Delivery Dates and Invoice Status

To calculate dates and determine the delivery status of an invoice, the `DeliveryDateController` subscribes to a series of events concerning invoices or encloses (`ExchangeName`).
Depending on the event, the calculation is performed using one of two strategies: `DateAndStatusEstimationStrategy` or `OnlyStatusEstimationStrategy`.
The algorithms for date calculation and status determination are implemented using a modified "Chain of Responsibility" pattern: `DatesCalculationDefinerChain` or `StatusDefinerChain`.
Initial information about the invoice and the enclose is obtained by querying an SQL Server database; a document is then created in the MongoDB collection from the response, and all further data retrieval is performed from MongoDB only.
If the processing results in a new calculation outcome, the information is updated in MongoDB.

### Maintaining the Status Change History for Each Enclose

Status change history is maintained so it can be accounted for during invoice delivery date calculations.
The `EncloseStateController` subscribes to all enclose status events.
When an event is received, the information in MongoDB is updated.

### Monitoring Day Changes in the Recipient's City

Day change monitoring in the recipient's city is performed on a schedule by the `DayChangedController`.
The IDs of recipient cities where the day has changed are identified.
For each city, three corresponding composite queries are performed to update date calculations, status, and overdue days.
For each invoice that requires recalculation, an event is sent to the corresponding queue.

# Configuring Document Indexing in MongoDB

Indexes are created in the MongoDB documents collection to support the following queries:
1. Search by invoice key.
2. Search by enclose key.
3. Composite search when the day changes for invoices not yet handed over by the sender.
4. Composite search when the day changes to update status.
5. Composite search when the day changes to update overdue day count.

Thus, three corresponding indexes are created in the `MongoDbIndex` file:
1. `invoice-key`
2. `enclose-key`
3. `day-changed`

# Service Configuration

All main service configuration is managed in Consul. Only logger settings (common for all environments) are stored in ```appsettings```.
Consul connection is set up via three environment variables as follows:
```
CONSUL_KEY=applications//DeliveryDatePlanning//local_config
CONSUL_TOKEN=/*token-value*/
CONSUL_URL=http:////8.8.8.8:8500
```

Description of nodes in the Consul configuration:
- **Links** – connections to external services (*Points* and *TariffZones*)
- **RabbitMq** – connection parameters for the *RabbitMQ* message broker
- **ConnectionStrings** – connections to data stores (*DB* and *Mongo*)