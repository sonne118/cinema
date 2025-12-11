# 🎬 Cinema Reservation System

A high-performance, distributed reservation system for cinemas, built with **.NET 8**, **Clean Architecture**, and **Domain-Driven Design (DDD)**. Designed for scalability, resilience.

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

## 🏗️ Architecture Overview
   
graph TD
    subgraph "Write Side - Transactional"
        Gateway["🌐 API Gateway<br/>Port 5005"]
        LB["⚖️ Load Balancer<br/>Port 5003"]
        API1["⚙️ API Node 1<br/>Port 5001"]
        API2["⚙️ API Node 2<br/>Port 5002"]
        SQL["💾 SQL Server<br/>Write DB (CinemaDb)"]
        
        note_trans["📝 Atomic Transaction:<br/>1. Business Data<br/>2. Outbox Message"]
    end
    
    subgraph "The Bridge - Master Node"
        MasterNode["👷 Master Node Worker<br/>(Outbox Processor)"]
        MasterSQL["💾 SQL Server<br/>Master DB (Reporting)"]
        
        note_tpl["⚡ TPL Batching<br/>Parallel.ForEachAsync"]
    end
    
    subgraph "Event Streaming"
        Kafka["📨 Kafka Broker<br/>Port 9092"]
        Topic1["Topic: cinema.domain.events"]
    end
    
    subgraph "Read Side - Queries"
        ReadService["🚀 Read Service<br/>(Kafka Consumer)"]
        
        subgraph "MongoDB Replica Set"
            Mongo1["🍃 Primary"]
            Mongo2["🍃 Secondary"]
            Mongo3["🍃 Secondary"]
        end

        Redis["⚡ Redis<br/>Cache"]
    end

    %% Command Flow
    Gateway -->|POST/PUT| LB
    LB --> API1
    LB --> API2
    
    API1 -->|Write| SQL
    API2 -->|Write| SQL
    note_trans -.-> SQL

    %% The Outbox Pattern (Master Node)
    MasterNode -->|1. Poll READPAST| SQL
    MasterNode -->|2a. Project| MasterSQL
    MasterNode -->|2b. Publish| Kafka
    note_tpl -.-> MasterNode
    
    %% Event Flow
    Kafka -->|Stream| Topic1
    Topic1 -.->|Consume| ReadService
    
    %% Read Side Updates
    ReadService -->|Update View| Mongo1
    Mongo1 -.->|Replicate| Mongo2
    Mongo1 -.->|Replicate| Mongo3
    
    %% Query Flow
    Gateway -->|GET gRPC| ReadService
    ReadService -->|Query| Mongo1
    ReadService -.->|Cache| Redis
    
    %% Styling
    style MasterNode fill:#ffccff,stroke:#660066,stroke-width:3px
    style SQL fill:#99ccff
    style MasterSQL fill:#99ccff
    style Kafka fill:#ffe6cc,stroke:#cc6600,stroke-width:2px
    style Mongo1 fill:#90ee90,stroke:#006400,stroke-width:2px
    style Redis fill:#ff6b6b,stroke:#c92a2a,stroke-width:2px

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