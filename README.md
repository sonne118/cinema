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