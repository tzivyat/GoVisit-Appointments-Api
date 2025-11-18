# מערכת זימון תורים - GoVisit Appointments API

## סקירה כללית
מערכת מרכזית לזימון תורים עבור כלל משרדי הממשלה, המאפשרת לאזרחים לקבוע תורים במשרדים שונים ולעובדי המשרדים לנהל את התורים.

## ארכיטקטורת המערכת

### רכיבי המערכת
1. **API Layer** - ASP.NET Core 8.0 Web API
2. **Business Logic** - Services עם Repository Pattern
3. **Data Layer** - MongoDB
4. **Cloud Infrastructure** - AWS (ECS + DocumentDB)

### מודלים עיקריים

#### Appointment (תור)
- מזהה ייחודי
- פרטי האזרח (ת.ז, שם, טלפון)
- מזהה משרד
- סוג שירות
- תאריך ושעת התור
- סטטוס התור
- הערות

#### Office (משרד)
- מזהה ייחודי
- שם המשרד
- משרד ממשלתי
- כתובת
- שירותים זמינים
- שעות פעילות

### API Endpoints

#### 1. יצירת תור חדש
```
POST /api/appointments
```
**Body:**
```json
{
  "citizenId": "123456789",
  "citizenName": "ישראל ישראלי",
  "citizenPhone": "050-1234567",
  "officeId": "office_id",
  "serviceType": "הנפקת תעודת זהות",
  "appointmentDate": "2024-01-15T10:00:00Z",
  "notes": "הערות נוספות"
}
```

#### 2. קבלת תורים לפי משרד
```
GET /api/appointments/office/{officeId}?date=2024-01-15
```

#### 3. עדכון סטטוס תור
```
PUT /api/appointments/{id}/status
```
**Body:**
```json
{
  "status": "Confirmed",
  "notes": "התור אושר"
}
```

#### 4. ביטול תור
```
DELETE /api/appointments/{id}
```

### סטטוסי תור
- `Scheduled` - מתוזמן
- `Confirmed` - מאושר
- `Completed` - הושלם
- `Cancelled` - בוטל
- `NoShow` - לא הגיע

## פריסה בענן AWS

### רכיבי התשתית

#### 1. Amazon ECS (Elastic Container Service)
- **Cluster**: GoVisit-Appointments-Cluster
- **Service**: appointments-api-service
- **Task Definition**: מכיל את ה-Docker container של ה-API

#### 2. Amazon DocumentDB (MongoDB Compatible)
- **Cluster**: govisit-docdb-cluster
- **Database**: GoVisitAppointments
- **Collections**: appointments, offices

#### 3. Application Load Balancer (ALB)
- חלוקת עומס בין instances
- SSL/TLS termination
- Health checks

#### 4. Amazon VPC
- **Subnets**: Public (ALB) + Private (ECS, DocumentDB)
- **Security Groups**: הגבלת גישה לפורטים נדרשים בלבד
- **NAT Gateway**: גישה לאינטרנט מ-private subnets

#### 5. AWS Systems Manager Parameter Store
- אחסון מאובטח של connection strings
- הגדרות תצורה

### תרשים ארכיטקטורה

```
Internet
    ↓
Application Load Balancer (Public Subnet)
    ↓
ECS Service (Private Subnet)
    ↓
DocumentDB Cluster (Private Subnet)
```

### אבטחה
- **VPC Security Groups**: הגבלת תעבורת רשת
- **IAM Roles**: הרשאות מינימליות נדרשות
- **Parameter Store**: אחסון מאובטח של credentials
- **HTTPS Only**: כל התקשורת מוצפנת

### ניטור ולוגים
- **CloudWatch Logs**: לוגי האפליקציה
- **CloudWatch Metrics**: מטריקות ביצועים
- **Health Checks**: בדיקות תקינות אוטומטיות

### סקיילביליות
- **Auto Scaling**: הגדלה/הקטנה אוטומטית של instances
- **DocumentDB Scaling**: קריאה מ-read replicas
- **Load Balancer**: חלוקת עומס אוטומטית

## הוראות פריסה

### דרישות מוקדמות
1. AWS CLI מותקן ומוגדר
2. Docker מותקן
3. .NET 8.0 SDK

### שלבי הפריסה

#### 1. בניית Docker Image
```bash
docker build -t govisit-appointments-api .
docker tag govisit-appointments-api:latest {account-id}.dkr.ecr.{region}.amazonaws.com/govisit-appointments-api:latest
```

#### 2. העלאה ל-ECR
```bash
aws ecr get-login-password --region {region} | docker login --username AWS --password-stdin {account-id}.dkr.ecr.{region}.amazonaws.com
docker push {account-id}.dkr.ecr.{region}.amazonaws.com/govisit-appointments-api:latest
```

#### 3. יצירת DocumentDB Cluster
```bash
aws docdb create-db-cluster \
    --db-cluster-identifier govisit-docdb-cluster \
    --engine docdb \
    --master-username admin \
    --master-user-password {secure-password}
```

#### 4. יצירת ECS Service
- הגדרת Task Definition
- יצירת Service עם Load Balancer
- הגדרת Auto Scaling

### משתני סביבה נדרשים
- `MongoDB__ConnectionString`: חיבור ל-DocumentDB
- `MongoDB__DatabaseName`: שם הדאטאבייס
- `ASPNETCORE_ENVIRONMENT`: Production

## בדיקות ואימות

### Health Check Endpoint
```
GET /health
```

### Swagger Documentation
זמין ב: `https://{domain}/swagger`

### בדיקות אוטומטיות
- Unit Tests עבור Services
- Integration Tests עבור API Endpoints
- Load Tests עבור ביצועים

## תחזוקה ועדכונים

### גיבויים
- DocumentDB Automated Backups
- Point-in-time Recovery

### עדכוני גרסה
- Blue/Green Deployment באמצעות ECS
- Zero-downtime updates

### ניטור ואלרטים
- CloudWatch Alarms עבור שגיאות
- Performance monitoring
- Cost monitoring