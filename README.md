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
- **MongoDB** - מסד נתונים NoSQL
- **AWS ECS** - Container orchestration
- **AWS DocumentDB** - MongoDB compatible database
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
https://localhost:7000
```

## API Endpoints

### יצירת תור חדש
```http
POST /api/appointments
Content-Type: application/json

{
  "citizenId": "123456789",
  "citizenName": "ישראל ישראלי", 
  "citizenPhone": "050-1234567",
  "officeId": "office123",
  "serviceType": "הנפקת תעודת זהות",
  "appointmentDate": "2024-01-15T10:00:00Z",
  "notes": "הערות"
}
```

### קבלת תורים לפי משרד
```http
GET /api/appointments/office/{officeId}?date=2024-01-15
```

### עדכון סטטוס תור
```http
PUT /api/appointments/{id}/status
Content-Type: application/json

{
  "status": "Confirmed",
  "notes": "התור אושר"
}
```

### ביטול תור
```http
DELETE /api/appointments/{id}
```

## פריסה בענן

הפרויקט מוכן לפריסה ב-AWS באמצעות:
- **ECS (Elastic Container Service)** - להרצת containers
- **DocumentDB** - מסד נתונים מנוהל
- **Application Load Balancer** - חלוקת עומס

לפרטים מלאים ראה: [ARCHITECTURE.md](ARCHITECTURE.md)

## מבנה הפרויקט

```
WebApplication1/
├── Controllers/          # API Controllers
├── Models/              # Data Models
├── DTOs/                # Data Transfer Objects  
├── Services/            # Business Logic
├── Program.cs           # Application Entry Point
├── Dockerfile           # Container Configuration
└── appsettings.json     # Configuration
```

## הגדרות תצורה

### appsettings.json
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "GoVisitAppointments"
  }
}
```

### משתני סביבה (Production)
- `MongoDB__ConnectionString` - חיבור למסד הנתונים
- `MongoDB__DatabaseName` - שם מסד הנתונים

## בדיקות

### הרצת בדיקות יחידה
```bash
dotnet test
```

### בדיקת Health Check
```http
GET /health
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