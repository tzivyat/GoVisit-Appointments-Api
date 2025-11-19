# GoVisit Appointments API

מערכת זימון תורים עבור משרדי הממשלה - פרויקט דמו

## תיאור הפרויקט

מערכת API מבוססת .NET 8 ו-MongoDB המאפשרת:
- יצירת תורים חדשים
- ניהול תורים קיימים
- עדכון סטטוס תורים
- ביטול תורים

## טכנולוגיות

- **.NET 8.0** - Web API Framework
- **MongoDB Atlas** - מסד נתונים NoSQL בענן
- **Google Cloud Run** - Container orchestration
- **Docker** - Containerization
- **Swagger/OpenAPI** - תיעוד API

## התקנה מקומית

### דרישות מוקדמות
- .NET 8.0 SDK
- MongoDB (או Docker)

### הרצה מקומית

1. **שכפול הפרויקט:**
```bash
git clone [repository-url]
cd GoVisit.Appointments.Api/WebApplication1
```

2. **התקנת dependencies:**
```bash
dotnet restore
```

3. **הרצת MongoDB (Docker):**
```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

4. **הרצת האפליקציה:**
```bash
dotnet run
```

5. **גישה ל-Swagger UI:**
```
http://localhost:8080/swagger
```

## API Endpoints

### זימון תור חכם
```http
POST /api/appointments/smart-booking
Content-Type: application/json

{
  "citizenId": "123456789",
  "citizenName": "ישראל ישראלי",
  "citizenPhone": "050-1234567",
  "officeId": "office123",
  "serviceType": "הנפקת תעודת זהות",
  "preferredDate": "2024-01-15",
  "preferredTime": "10:00:00",
  "durationMinutes": 30,
  "notes": "הערות"
}
```

### חיפוש תורים מתקדם
```http
POST /api/appointments/office/{officeId}/search
Content-Type: application/json

{
  "activeOnly": true,
  "status": "Confirmed",
  "fromDate": "2024-01-01",
  "toDate": "2024-01-31"
}
```

### עדכון תור
```http
PATCH /api/appointments/{id}
Content-Type: application/json

{
  "status": "Confirmed",
  "notes": "התור אושר"
}
```

### ביטול תור עם חלופות
```http
DELETE /api/appointments/{id}/with-alternative
```

### קבלת תור
```http
GET /api/appointments/{id}
```

## פריסה בענן

הפרויקט מוכן לפריסה ב-Google Cloud Platform:
- **Cloud Run** - Serverless container platform
- **Cloud Build** - CI/CD pipeline
- **MongoDB Atlas** - מסד נתונים מנוהל
- **GitHub Integration** - פריסה אוטומטית

לפרטים מלאים ראה: [SYSTEM_ARCHITECTURE.md](SYSTEM_ARCHITECTURE.md)

## מבנה הפרויקט

```
WebApplication1/
├── Controllers/          # API Controllers (5 endpoints)
├── Services/            # Business Logic Layer
├── Repositories/        # Data Access Layer
├── Models/              # Data Models
├── DTOs/                # Data Transfer Objects
├── Resources/           # Messages & Constants
├── Properties/          # Launch Settings
├── Program.cs           # Application Entry Point
├── Dockerfile           # Container Configuration
├── appsettings.json     # Base Configuration
├── appsettings.Development.json  # Dev Settings
└── appsettings.Production.json   # Prod Settings
```

## הגדרות תצורה

### appsettings.json
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb+srv://govisit-user:SecurePass123@govisitdb.301iada.mongodb.net/?appName=GoVisitDB",
    "DatabaseName": "GoVisitAppointments"
  }
}
```

### משתני סביבה (Production)
- `MongoDB__ConnectionString` - חיבור למסד הנתונים
- `MongoDB__DatabaseName` - שם מסד הנתונים

## בדיקות

### בדיקת Health Check
```http
GET /health
Response: {
  "status": "healthy",
  "timestamp": "2024-01-15T10:00:00Z"
}
```

### בדיקת Swagger Documentation
```
http://localhost:8080/swagger
```

## תרומה לפרויקט

1. Fork הפרויקט
2. צור branch חדש (`git checkout -b feature/amazing-feature`)
3. Commit השינויים (`git commit -m 'Add amazing feature'`)
4. Push ל-branch (`git push origin feature/amazing-feature`)
5. פתח Pull Request

## רישיון

פרויקט זה הוא לצרכי הדגמה בלבד.

## יצירת קשר

לשאלות ובעיות, אנא פתח Issue בפרויקט.