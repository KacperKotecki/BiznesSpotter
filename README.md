<div align="center">
  <h1>📍 BiznesSpotter</h1>
  <p><b>Data-Driven Location Intelligence | BiteHack 2026 Project</b></p>
  
  ![.NET 8](https://img.shields.io/badge/.NET%208.0-purple?style=flat&logo=dotnet)
  ![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue?style=flat)
  ![Google Maps API](https://img.shields.io/badge/API-Google%20Places-4285F4?style=flat&logo=googlemaps)
  ![GUS API](https://img.shields.io/badge/API-Statistics%20Poland%20(GUS)-dc3545?style=flat)
  ![SQLite](https://img.shields.io/badge/Database-SQLite-003B57?style=flat&logo=sqlite)
</div>

> **Stop guessing where to open your business. Let data decide.**
> BiznesSpotter is an analytical platform that evaluates the business potential of a specific location. By concurrently aggregating geospatial data from **Google Places API** and demographic statistics from the **Polish Central Statistical Office (GUS)**, it calculates market saturation and helps entrepreneurs find the perfect spot for their next venture.

---

## 🚀 Hackathon MVP & Architecture

This project was developed during **BiteHack 2026**. The codebase was built with solid engineering principles to ensure maintainability and performance:

### 🧠 1. Market Saturation Algorithm
The core `BusinessAnalysisService` calculates a custom **Competition Index** by analyzing the ratio of existing competitors (from Google Maps) to the local population (from GUS public data). It categorizes the market status from "Very Low Competition" to "Very Saturated".

### ⚡ 2. Concurrent API Orchestration
To ensure a snappy user experience, the system fetches data from independent external APIs concurrently using asynchronous programming (`Task.WhenAll`), significantly reducing the overall response time.

### 🛡️ 3. Architecture & Design Patterns
* **Repository & Unit of Work:** Abstracts data access and ensures atomic database transactions (e.g., saving user search history).
* **Dependency Injection:** Strict use of DI for services (`IGooglePlacesService`, `IGusService`, `IBusinessAnalysisService`) to ensure loose coupling.
* **Separation of Concerns:** Clear boundaries between Domain models, ViewModels, and external API DTOs, utilizing custom Mappers to keep Controllers thin.

---

## 📺 System Preview

### 1. Location & Industry Search
Users can define their target city, industry (e.g., Gastronomy, Beauty), and search radius.

<img width="1920" height="1440" alt="377shots_so" src="https://github.com/user-attachments/assets/9f10702b-784b-4b6c-b892-5b4c66b65918" />


### 2. Interactive Map & Demographic Dashboard
A visual representation of the search radius, competitors' locations, and a detailed breakdown of local demographics and the calculated Competition Index.

<img width="1920" height="1440" alt="205shots_so" src="https://github.com/user-attachments/assets/69e358c7-865a-477d-9f96-6b1e75d66bf1" />

---

## 🛠️ Tech Stack

**Backend & Architecture:**
* **.NET 8.0** (ASP.NET Core MVC)
* **Entity Framework Core 8** (SQLite for rapid prototyping)
* **Identity** (Authentication & User Management)
* **N-Tier Architecture**

**Frontend:**
* Razor Views (`.cshtml`)
* Bootstrap 5 & Custom CSS
* Vanilla JavaScript & Google Maps JS API

**Integrations:**
* **Google Places API** (Geocoding & Competitor Search - requires API Key)
* **GUS API** (Demographic Data via `bdl.stat.gov.pl` - public access)

---

## ⚙️ Getting Started (Local Development)

### Prerequisites
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* Google Cloud Console Account (for Places API Key)

### Installation Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/YourUsername/BiznesSpotter.git
   cd BiznesSpotter/BiznesSpoter.Web
   ```

2. **Configure Environment Variables (Secrets):**
   You need to provide a Google Maps API key. The GUS API uses public access.
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "GoogleMaps:ApiKey" "YOUR_GOOGLE_MAPS_API_KEY"
   ```

3. **Apply Database Migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

---

## 👥 Team (BiteHack 2026)
* **Kacper Kotecki** 
* **Emil Piwowarczyk**
* **Kacper Papuga**
* **Emil Górski**
