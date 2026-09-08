# Claude.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

Server Monitor: A web application that monitors the status of servers and services, providing real-time updates and alerts. The application is build using Blazor .NET 10 and C#.
A user can log in to the application. Each user has his own list of servers and services to monitor. The application will check the status of each server and service at a 
configurable interval and update the user interface accordingly. If a server or service goes down, the user will receive an alert. The applicaton show statistics and charts 
about the servers and services, such as uptime, response time, and error rates. The application also provides a dashboard with charts and graphs to visualize the data.

## Architecture

The application is built using clean architecture principles, separating concerns into different layers: Domain(Data Models), Use Cases(Business Logic), 
Application(Controllers, Services), and Infrastructure(Data Access, External Services). 
The front-end is built using Blazor components, providing a responsive and interactive user interface.
The application uses Entity Framework Core for data access, and SignalR for real-time communication between the server and clients. 
The application is designed to be scalable and maintainable, with a focus on testability and code quality.
The application uses Identity from Entity Framework Core for user authentication and authorization, allowing users to securely log in and manage their servers and services. 
Also using Roles from Microsoft Identity to manage user permissions and access control.

## Coding standards

- Use latest/idiomatic library versions.
- Keep it simple — no over-engineering, no unnecessary defensive programming, no speculative features.
- Be concise; no emojis, ever.
- When debugging, find the root cause with evidence before fixing — don't guess.
