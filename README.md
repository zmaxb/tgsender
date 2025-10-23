# TGSender Daemon

**Minimal Telegram sender daemon — HTTP in, Telegram out.**

---

### 🧠 Overview

`tgsenderd` is a lightweight HTTP daemon that receives POST requests and forwards their contents to a Telegram chat.  
Designed for infrastructure alerts, backup jobs, and automation hooks.

---

### ⚙️ Features

- Simple `/tg/send` endpoint
- Accepts raw text in POST body
- Built-in health check (`/health`)
- Ready for Docker, Compose, and CI/CD

---

### 🚀 Quick Start

#### Run with Docker

```bash
docker run -d \
  --name tgsenderd \
  -p 7850:7850 \
  -e TG_TOKEN="your_bot_token" \
  -e TG_CHAT_ID="your_chat_id" \
  zmaxb/tgsender:latest
```

#### Test the endpoint

```bash
curl -X POST \
  -H "Content-Type: text/plain" \
  --data "Hello from TGSender" \
  http://localhost:7850/tg/send
```

If everything is configured correctly,
you’ll receive the message in your Telegram chat.

### 🐳 Docker Compose Example

```yaml
services:
  tgsenderd:
    image: zmaxb/tgsender:latest
    container_name: tgsenderd
    restart: unless-stopped
    ports:
      - "7850:7850"
    environment:
      - TG_TOKEN=${TG_TOKEN}
      - TG_CHAT_ID=${TG_CHAT_ID}
    networks:
      - eden-net

networks:
  eden-net:
    external: bridge
