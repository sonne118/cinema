Cinema Reservation System



📖 Overview

This project is a high-performance, distributed Cinema Reservation System built with .NET 8. It demonstrates a production-grade implementation of Clean Architecture, Domain-Driven Design (DDD), and CQRS (Command Query Responsibility Segregation) principles. The system is designed for high scalability and resilience, separating write operations (business logic) from read operations (queries) using an event-driven approach.



🚀 Key Features \& Functionality

1\. Showtime Management

Create Showtimes: Administrators can schedule movies for specific auditoriums and times.

Validation: Ensures no conflicting screenings in the same auditorium.

2\. Reservation System

Seat Reservation: Users can select and reserve specific seats for a showtime.

Temporal Expiration: Implements a "hold" mechanism where reservations are valid for 10 minutes. If not confirmed within this window, they automatically expire to free up seats.

Confirmation: Users can confirm their pending reservations to finalize the booking.

3\. High-Performance Querying

Optimized Reads: Uses a dedicated Read Service backed by MongoDB to serve high-traffic queries (e.g., listing available showtimes) with low latency, independent of the transactional database load.

🏗️ Architecture \& Approaches

The solution is built as a set of microservices orchestrated via Docker Compose:



1\. CQRS (Command Query Responsibility Segregation)

Write Side (Commands): Handles all business logic and state mutations.

Tech: .NET 8 API, SQL Server, Entity Framework Core.

Flow: Client -> Gateway -> Load Balancer -> API -> Domain Logic -> SQL Server.

Read Side (Queries): Handles data retrieval.

Tech: .NET 8 gRPC Service, MongoDB.

Flow: Client -> Gateway -> gRPC -> Read Service -> MongoDB.

2\. Domain-Driven Design (DDD)

Aggregates: The business logic is encapsulated in rich domain models (Reservation, Showtime) that enforce invariants.

Example Algorithm: The Reservation aggregate enforces the 10-minute expiration rule internally (ExpiresAt = createdAt.AddMinutes(10)) and prevents confirming expired or already confirmed reservations.

Value Objects: Uses strongly-typed value objects (SeatNumber, ShowtimeId) to prevent primitive obsession and ensure data integrity.

3\. Event-Driven Consistency (The "Outbox Pattern")

To ensure data consistency between the Write DB (SQL Server) and Read DB (MongoDB) without distributed transactions (2PC):



Transaction: When a reservation is created, the state is saved to SQL Server and a domain event (ReservationCreatedEvent) is saved to an OutboxMessages table in the same transaction.

Publishing: A background job (ProcessOutboxMessagesJob) polls the outbox and publishes events to Kafka.

Consumption: The Read Service consumes these Kafka events and updates the MongoDB read models, achieving Eventual Consistency.

4\. Infrastructure Components

API Gateway (Ocelot): Acts as the single entry point, routing requests to the appropriate internal services.

Load Balancer (YARP): Distributes incoming traffic across multiple instances of the API service (cinema-api-1, cinema-api-2) for high availability.

gRPC: Used for high-performance synchronous communication between the Gateway and the Read Service.

Redis: Utilized for distributed caching to further improve read performance.

🛠️ Tech Stack

Framework: .NET 8 (C#)

Databases: SQL Server 2022 (Write), MongoDB 7.0 (Read), Redis (Cache)

Messaging: Apache Kafka \& Zookeeper

Containerization: Docker \& Docker

Compose



Communication: gRPC, REST, YARP (Reverse Proxy)

Testing: xUnit, FluentAssertions, Integration Tests



&nbsp;                  FOR TESTING

============================================================

curl -X POST "http://localhost:5001/api/Showtimes" \\

-H "Content-Type: application/json" \\

-d '{

&nbsp; "movieImdbId": "tt1375666",

&nbsp; "screeningTime": "2025-12-12T20:00:00Z",

&nbsp; "auditoriumId": "0C7F275C-A5EA-456C-BBF9-4DAC0B028E73"

}'





============================================================

curl -X POST "http://localhost:5001/api/Reservations" \\

-H "Content-Type: application/json" \\

-d '{

&nbsp; "showtimeId": "34306464-2135-4992-89b1-3e25839fbc4f",

&nbsp; "seats": \[

&nbsp;   {

&nbsp;     "row": 5,

&nbsp;     "number": 10

&nbsp;   }

&nbsp; ]

}'

