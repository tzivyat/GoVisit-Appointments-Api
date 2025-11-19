# ארכיטקטורת מערכת GoVisit Appointments API

## סקירה כללית

מערכת זימון תורים מבוססת ענן עבור משרדי הממשלה, הבנויה על ארכיטקטורת microservices עם דגש על ביצועים, זמינות גבוהה ויכולת הרחבה.

## ארכיטקטורת המערכת

### רכיבי המערכת

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Web Client    │    │   Mobile App    │    │  Admin Portal   │
│   (Browser)     │    │   (iOS/Android) │    │   (Dashboard)   │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────▼─────────────┐
                    │   Load Balancer (GCP)    │
                    │   Application Gateway     │
                    └─────────────┬─────────────┘
                                 │
                    ┌─────────────▼─────────────┐
                    │   GoVisit API Service    │
                    │   (.NET 8 Web API)       │
                    │   - Controllers Layer    │
                    │   - Business Logic       │
                    │   - Repository Pattern   │
                    └─────────────┬─────────────┘
                                 │
                    ┌─────────────▼─────────────┐
                    │   MongoDB Atlas          │
                    │   (Document Database)    │
                    │   - Appointments         │
                    │   - Offices              │
                    │   - Audit Logs           │
                    └─────────────────────────┘
```

## פריסה בענן - Google Cloud Platform

### רכיבי התשתית

#### 1. **Cloud Run** - Application Hosting
- **Container Runtime**: Docker containers
- **Auto Scaling**: 0-10 instances
- **Resource Allocation**: 1 vCPU, 512 MiB RAM
- **Request Timeout**: 300 seconds
- **Concurrency**: 80 requests per instance

#### 2. **Cloud Build** - CI/CD Pipeline
- **Source**: GitHub repository integration
- **Build Trigger**: Automatic on master branch push
- **Build Process**: Multi-stage Docker build
- **Deployment**: Automatic to Cloud Run

#### 3. **MongoDB Atlas** - Database Service
- **Cluster**: M0 (Free Tier) / M10 (Production)
- **Region**: Europe-West1 (Belgium)
- **Replication**: 3-node replica set
- **Backup**: Continuous backup with point-in-time recovery

#### 4. **Cloud Load Balancing**
- **Type**: Application Load Balancer
- **SSL/TLS**: Automatic certificate management
- **Health Checks**: HTTP health endpoint monitoring
- **Geographic Distribution**: Multi-region support

### ארכיטקטורת הרשת

```
Internet
    │
    ▼
┌─────────────────────────────────────────────────────────────┐
│                    Google Cloud Platform                    │
│                                                             │
│  ┌─────────────────┐    ┌─────────────────┐                │
│  │  Cloud DNS      │    │  Cloud CDN      │                │
│  │  (Domain)       │    │  (Static Assets)│                │
│  └─────────┬───────┘    └─────────┬───────┘                │
│            │                      │                        │
│  ┌─────────▼──────────────────────▼───────┐                │
│  │        Application Load Balancer       │                │
│  │        (SSL Termination)               │                │
│  └─────────────────┬───────────────────────┘                │
│                    │                                        │
│  ┌─────────────────▼───────────────────────┐                │
│  │            Cloud Run Service            │                │
│  │  ┌─────────────────────────────────────┐ │                │
│  │  │        API Container             │ │                │
│  │  │  - .NET 8 Runtime               │ │                │
│  │  │  - Swagger Documentation        │ │                │
│  │  │  - Health Monitoring            │ │                │
│  │  └─────────────────────────────────────┘ │                │
│  └─────────────────┬───────────────────────┘                │
│                    │                                        │
└────────────────────┼────────────────────────────────────────┘
                     │
                     ▼
            ┌─────────────────┐
            │  MongoDB Atlas  │
            │  (External SaaS)│
            └─────────────────┘
```

## ארכיטקטורת האפליקציה

### שכבות המערכת

#### 1. **Presentation Layer** (Controllers)
```csharp
AppointmentsController
├── SmartBookAppointment()      // POST /api/appointments/smart-booking
├── GetPrioritizedAppointments() // POST /api/appointments/office/{id}/search
├── UpdateAppointmentFields()    // PATCH /api/appointments/{id}
├── CancelWithAlternative()      // DELETE /api/appointments/{id}/with-alternative
└── GetAppointment()            // GET /api/appointments/{id}
```

#### 2. **Business Logic Layer** (Services)
```csharp
IAppointmentService
├── SmartBookAppointmentAsync()     // זימון חכם עם בדיקת זמינות
├── GetPrioritizedAppointmentsAsync() // סינון וסידור לפי עדיפות
├── UpdateAppointmentFieldsAsync()   // עדכון חלקי של שדות
├── CancelWithAlternativeAsync()     // ביטול עם הצעת חלופות
└── GetAppointmentByIdAsync()       // שליפת תור בודד
```

#### 3. **Data Access Layer** (Repository)
```csharp
IAppointmentRepository
├── CreateAsync()                   // יצירת תור חדש
├── GetByIdAsync()                 // שליפה לפי מזהה
├── GetFilteredAppointmentsAsync() // חיפוש מסונן
├── PartialUpdateAsync()           // עדכון חלקי
├── DeleteAsync()                  // מחיקת תור
└── GetAvailableSlotsAsync()       // זמנים פנויים
```

#### 4. **Data Models**
```csharp
Appointment
├── Id: string
├── CitizenId: string
├── CitizenName: string
├── CitizenPhone: string
├── OfficeId: string
├── ServiceType: string
├── AppointmentDate: DateTime
├── DurationMinutes: int
├── Status: AppointmentStatus
├── Priority: AppointmentPriority
├── Notes: string
├── CreatedAt: DateTime
└── UpdatedAt: DateTime
```

## אבטחה ואימות

### אמצעי אבטחה

1. **HTTPS Enforcement**
   - SSL/TLS 1.2+ חובה
   - Certificate management אוטומטי
   - HSTS headers

2. **Input Validation**
   - Data annotations validation
   - Model binding validation
   - SQL injection prevention

3. **Error Handling**
   - Structured logging
   - Error masking בproduction
   - Health monitoring

4. **CORS Policy**
   - Configured origins
   - Secure headers
   - Preflight handling

## ביצועים ואופטימיזציה

### אסטרטגיות ביצועים

1. **Database Optimization**
   - Connection timeout: 5 seconds ✅
   - Server selection timeout: 5 seconds ✅
   - Basic MongoDB configuration
   - Simple connection management

2. **Caching Strategy**
   - In-memory caching (IMemoryCache) ✅
   - Basic memory cache implementation
   - No CDN configured

3. **Async Programming**
   - Non-blocking I/O operations
   - Task-based async pattern
   - Cancellation token support

## מוניטורינג ולוגים

### כלי מוניטורינג

1. **Application Monitoring**
   - Health checks endpoint (`/health`)
   - Performance metrics
   - Error rate tracking

2. **Infrastructure Monitoring**
   - Cloud Run metrics
   - Resource utilization
   - Auto-scaling events

3. **Logging Strategy**
   - Structured logging (JSON)
   - Log levels configuration
   - Centralized log aggregation

## אסטרטגיית פריסה

### CI/CD Pipeline

```
GitHub Repository
        │
        ▼
┌─────────────────┐
│   Code Push     │
│   (master)      │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│  Cloud Build    │
│  - Docker Build │
│  - Unit Tests   │
│  - Security Scan│
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│  Container      │
│  Registry       │
│  (Artifact)     │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│  Cloud Run      │
│  Deployment     │
│  (Production)   │
└─────────────────┘
```

### Deployment Strategy

1. **Blue-Green Deployment**
   - Zero-downtime deployments
   - Instant rollback capability
   - Traffic splitting for testing

2. **Auto-scaling Configuration**
   - Min instances: 0 (cost optimization)
   - Max instances: 10 (performance limit)
   - CPU utilization threshold: 70%
   - Memory utilization threshold: 80%

## אסטרטגיית גיבוי ושחזור

### Database Backup
- **MongoDB Atlas**: Automatic backup (if using Atlas)
- **Local Development**: Manual backup procedures
- **Configuration**: Connection string based

### Disaster Recovery
- **Simple Recovery**: Container restart capability
- **Data Recovery**: Depends on MongoDB configuration
- **Stateless Design**: Easy redeployment

## עלויות ותחזוקה

### Cost Optimization
- **Serverless Architecture**: Pay-per-request model
- **Auto-scaling**: Scale to zero when idle
- **Resource Right-sizing**: Optimized CPU/Memory allocation

### Maintenance Windows
- **Automated Updates**: Container base image updates
- **Security Patches**: Automatic OS-level patching
- **Database Maintenance**: Managed by MongoDB Atlas

## מדדי ביצועים (KPIs)

### Performance Metrics
- **Health Check**: `/health` endpoint ✅
- **Basic Monitoring**: Application logs
- **Error Handling**: Try-catch blocks ✅
- **Timeout Configuration**: 5 seconds ✅

### Business Metrics
- **Appointment Success Rate**: > 95%
- **User Satisfaction**: Measured via API response times
- **System Utilization**: Optimal resource usage

---

## מצב הפרויקט הנוכחי

### ✅ **מיושם בפרויקט:**
- .NET 8 Web API עם Swagger
- MongoDB integration עם connection management
- Repository Pattern + Service Layer
- 5 API endpoints מתועדים
- Docker containerization
- Health check endpoint
- CORS configuration
- Error handling ו-logging
- Memory caching (IMemoryCache)

### 🚧 **לא מיושם (אפשר להוסיף):**
- Unit tests
- Authentication/Authorization
- Advanced monitoring
- Database indexing
- Response caching
- Rate limiting
- Advanced error handling

## סיכום

מערכת GoVisit Appointments API מספקת בסיס חזק לפתרון זימון תורים, עם ארכיטקטורה נקייה ומוכנות לפריסה בענן. הפרויקט כולל את כל הרכיבים הבסיסיים הנדרשים ומוכן להרחבה עתידית.