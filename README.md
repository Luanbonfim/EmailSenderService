# Product Email Service

A .NET Worker Service that consumes messages from a RabbitMQ queue and sends emails when new products are created.

---

## Overview

This project is a .NET Worker Service that listens to a RabbitMQ queue for messages about new products. When a message is received, the service sends an email notification to a specified recipient.

---

## Features

- Consumes messages from a RabbitMQ queue.
- Sends email notifications using SMTP.
- Configurable settings for RabbitMQ and email via `appsettings.json`.
- Built as a .NET Worker Service for background processing.

---

## Prerequisites

Before running the project, ensure you have the following installed:

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [RabbitMQ](https://www.rabbitmq.com/download.html)
- An email account with SMTP access (e.g., Gmail, Outlook).

---

## Setup

### Clone the Repository

```bash
git clone https://github.com/your-username/EmailSenderService.git
cd EmailSenderService
