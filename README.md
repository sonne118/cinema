# 🎬 Cinema Reservation System

A high-performance, distributed reservation system for cinemas, built with **.NET 8**, **Clean Architecture**, and **Domain-Driven Design (DDD)**. Designed for scalability, resilience, and real-world production readiness.

---

## 📖 Overview

This system demonstrates a production-grade implementation of:

- ✅ Clean Architecture  
- ✅ Domain-Driven Design (DDD)  
- ✅ CQRS (Command Query Responsibility Segregation)  
- ✅ Event-Driven Consistency via Kafka

It separates **write operations** (business logic) from **read operations** (queries), ensuring high throughput and eventual consistency.

---

## 🚀 Features

### 🎥 Showtime Management
- Create and schedule movie showtimes per auditorium  
- Conflict detection to prevent overlapping screenings

### 🎟️ Reservation System
- Reserve specific seats for a showtime  
- 10-minute hold mechanism with automatic expiration  
- Confirm reservations before expiration

### ⚡ High-Performance Querying
- Dedicated Read Service backed by MongoDB  
- Low-latency queries independent of transactional load

---

## 🏗️ Architecture

### 🧠 CQRS Pattern
- **Write Side**:  
  `.NET 8 API → SQL Server → Entity Framework Core`  
- **Read Side**:  
  `.NET 8 gRPC Service → MongoDB`

### 🧱 Domain-Driven Design
- Rich Aggregates: `Reservation`, `Showtime`  
- Value Objects: `SeatNumber`, `ShowtimeId`  
- Internal expiration logic:  
  `ExpiresAt = CreatedAt.AddMinutes(10)`

### 🔁 Event-Driven Consistency (Outbox Pattern)
- Domain events saved to `OutboxMessages` table  
- Background job publishes events to Kafka  
- Read Service consumes Kafka events → updates MongoDB

---

## 🧩 Infrastructure

- **API Gateway**: Ocelot  
- **Load Balancer**: YARP  
- **Messaging**: Kafka + Zookeeper  
- **Cache**: Redis  
- **Containerization**: Docker + Docker Compose  
- **Communication**: REST + gRPC

---

## 🧪 Testing

- Unit Tests: xUnit  
- Assertions: FluentAssertions  
- Integration Tests: Dockerized test environment

---

## 🧬 Tech Stack

| Layer         | Technology               |
|---------------|---------------------------|
| Framework     | .NET 8 (C#)               |
| Write DB      | SQL Server 2022           |
| Read DB       | MongoDB 7.0               |
| Cache         | Redis                     |
| Messaging     | Apache Kafka + Zookeeper  |
| Gateway       | Ocelot                    |
| Load Balancer | YARP                      |
| Container     | Docker + Compose          |

---

%%{init: {'theme': 'base', 'themeVariables': { 'primaryColor': '#326ce5', 'edgeLabelBackground':'#ffffff', 'tertiaryColor': '#fff0f0'}}}%%
graph TD
    User([👤 User / Client])
    
    subgraph "Entry Point"
        Gateway[🌐 API Gateway\n(Ocelot)]
    end
    
    subgraph "Write Side (Commands)"
        direction TB
        LB[⚖️ Load Balancer\n(YARP)]
        API[⚙️ Cinema API\n(.NET 8)]
        SQL[(🗄️ SQL Server\nWrite DB)]
        Outbox[🔄 Outbox Processor]
    end
    
    subgraph "Event Bus"
        Kafka{{📨 Apache Kafka}}
    end
    
    subgraph "Read Side (Queries)"
        direction TB
        ReadService[🚀 Read Service\n(gRPC)]
        Mongo[(🍃 MongoDB\nRead DB)]
        Redis[(⚡ Redis\nCache)]
    end

    User ==>|1. Request| Gateway
    
    %% Write Path
    Gateway -->|POST/PUT| LB
    LB --> API
    API -->|2. Persist State| SQL
    Outbox -.->|3. Poll Events| SQL
    Outbox -->|4. Publish| Kafka
    
    %% Read Path
    Gateway -->|GET (gRPC)| ReadService
    ReadService -->|5. Query| Mongo
    ReadService -.->|Cache| Redis
    
    %% Sync
    Kafka ==>|6. Sync Data| ReadService
    ReadService -->|7. Update Model| Mongo

    classDef db fill:#e1f5fe,stroke:#01579b,stroke-width:2px;
    classDef service fill:#e8f5e9,stroke:#2e7d32,stroke-width:2px;
    classDef infra fill:#fff3e0,stroke:#ef6c00,stroke-width:2px;
    
    class SQL,Mongo,Redis db;
    class API,ReadService,Outbox service;
    class Gateway,LB,Kafka infra;
	
	###Outbox Pattern and Eventual Consistency mechanism.
    sequenceDiagram
    autonumber
    participant Client
    participant Gateway as API Gateway
    participant API as Write API
    participant SQL as SQL Server
    participant Worker as Outbox Job
    participant Kafka
    participant Read as Read Service
    participant Mongo as MongoDB

    Note over Client, SQL: 🟢 Synchronous Write Path
    Client->>Gateway: POST /reservations
    Gateway->>API: Proxy Request
    API->>API: Validate Domain Logic
    API->>SQL: BEGIN TRANSACTION
    API->>SQL: Insert Reservation
    API->>SQL: Insert Outbox Message
    API->>SQL: COMMIT TRANSACTION
    API-->>Client: 202 Accepted

    Note over Worker, Mongo: 🟡 Asynchronous Consistency Path
    loop Every 10s
        Worker->>SQL: Poll Unprocessed Messages
        Worker->>Kafka: Publish "ReservationCreated"
        Worker->>SQL: Mark Processed
    end

    Kafka->>Read: Consume Event
    Read->>Mongo: Update Read Model
    
    Note over Client, Mongo: 🔵 High-Performance Read Path
    Client->>Gateway: GET /reservations
    Gateway->>Read: gRPC Call
    Read->>Mongo: Query Optimized Data
    Read-->>Client: Return JSON
sequenceDiagram
    autonumber
    participant Client
    participant Gateway as API Gateway
    participant API as Write API
    participant SQL as SQL Server
    participant Worker as Outbox Job
    participant Kafka
    participant Read as Read Service
    participant Mongo as MongoDB

    Note over Client, SQL: 🟢 Synchronous Write Path
    Client->>Gateway: POST /reservations
    Gateway->>API: Proxy Request
    API->>API: Validate Domain Logic
    API->>SQL: BEGIN TRANSACTION
    API->>SQL: Insert Reservation
    API->>SQL: Insert Outbox Message
    API->>SQL: COMMIT TRANSACTION
    API-->>Client: 202 Accepted

    Note over Worker, Mongo: 🟡 Asynchronous Consistency Path
    loop Every 10s
        Worker->>SQL: Poll Unprocessed Messages
        Worker->>Kafka: Publish "ReservationCreated"
        Worker->>SQL: Mark Processed
    end

    Kafka->>Read: Consume Event
    Read->>Mongo: Update Read Model
    
    Note over Client, Mongo: 🔵 High-Performance Read Path
    Client->>Gateway: GET /reservations
    Gateway->>Read: gRPC Call
    Read->>Mongo: Query Optimized Data
    Read-->>Client: Return JSON       


## 🧪 Sample API Calls

### Create a Showtime
curl -X POST http://localhost:5001/api/Showtimes \
-H "Content-Type: application/json" \
-d '{
  "movieImdbId": "tt1375666",
  "screeningTime": "2025-12-12T20:00:00Z",
  "auditoriumId": "0C7F275C-A5EA-456C-BBF9-4DAC0B028E73"
}'

### Reserve Seats
curl -X POST http://localhost:5001/api/Reservations \
-H "Content-Type: application/json" \
-d '{
  "showtimeId": "34306464-2135-4992-89b1-3e25839fbc4f",
  "seats": [
    { "row": 5, "number": 10 }
  ]
}'