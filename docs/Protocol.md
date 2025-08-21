# 🛠️ Service Communication Protocol (gRPC + Socket Streaming)

This document explains how the main app communicates with its services using `servicecmd.proto`, and how to properly add new services — whether **AutoStart**, **On-Demand**, or **Streaming**.

---

## 📦 `servicecmd.proto` Structure
> See `proto\servicecmd.proto` file to see the proto strucure
---
## 🚀 Service Types

### ✅ 1. AutoStart Services

These services are launched **automatically** on app start based on config and are assigned a **unique `ServiceId`**.

> **Example**: `LISTEN_TO_DESKTOP_AUDIO`, `SEE_SCREEN`

**How to add:**
- Add a new `ServiceId` enum entry in `servicecmd.proto`
- Create a corresponding handler in `service_handler.py`
- Implement your service logic

---

### 🕹️ 2. On-Demand Services

These are triggered **only when a `Cmd` is received** with `serviceId = START_SERVICE`.

> **Example**: User-initiated dynamic services

**How to add:**
- ❌ DO NOT modify the `ServiceId` enum
- ✅ Add a handler for your service name in `service_handler.py` under the `START_SERVICE` case
- Use `Cmd.serviceName` to identify your service
- Use `Cmd.data` to handle any input needed

---

## 🔁 Response Format

Every service should return a `CmdResponse`:

```proto
CmdResponse {
  string serviceName;
  bytes data;
  Status status;
}
```
## 📡 Streaming Data Services

Services that **continuously push data** (e.g. audio/video feeds/ai gen text) should **not use gRPC**.

### How to implement:
- Use the **"To Be Implemented"** streaming class
- Push data directly to the **main app’s socket connection**
- Ensure the **connection is alive** before sending

---

## ✅ Summary

| Type        | ServiceId Needed? | Triggered How?           | Data Flow                |
|-------------|-------------------|---------------------------|--------------------------|
| AutoStart   | ✅ Yes            | App Config / Startup      | gRPC Request/Response or tcp    |
| On-Demand   | ❌ No             | based on the name in `Cmd.serviceName` | gRPC Request/Response or tcp    |
| Streaming   | ❌ No             | Can be `AutoStart` or `OnDemand`       | Strict Socket Stream |

