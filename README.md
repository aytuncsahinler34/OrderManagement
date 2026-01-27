# Sipariş Yönetim Sistemi (Order Management System)

E-ticaret platformu için geliştirilmiş modern bir sipariş yönetim backend servisi. .NET 9, RabbitMQ ve Clean Architecture prensiplerine dayalı olarak tasarlanmıştır.

## 📋 Özellikler

- ✅ RESTful API ile sipariş yönetimi
- ✅ RabbitMQ ile asenkron mesaj kuyruğu
- ✅ Background Worker Service ile sipariş işleme
- ✅ In-Memory Database (Entity Framework Core)
- ✅ Clean Architecture yapısı
- ✅ Comprehensive Unit Tests
- ✅ Swagger/OpenAPI dokümantasyonu

## 🏗️ Mimari

Proje Clean Architecture prensiplerine uygun olarak katmanlara ayrılmıştır:

```
OrderManagementSystem/
├── src/
│   ├── OrderManagement.Core/          # Domain katmanı (Entities, Interfaces)
│   ├── OrderManagement.Infrastructure/ # Veritabanı ve Messaging implementasyonları
│   ├── OrderManagement.API/           # REST API endpoints
│   └── OrderManagement.WorkerService/ # Background Service (RabbitMQ Consumer)
└── tests/
    └── OrderManagement.Tests/         # Unit testler
```

## 🛠️ Teknolojiler

- **.NET 9.0** - Framework
- **Entity Framework Core InMemory** - Veritabanı
- **RabbitMQ** - Message Queue
- **Swagger** - API Dokümantasyonu
- **xUnit** - Unit Testing
- **Moq** - Mocking Framework

## 📦 Kurulum

### Gereksinimler

- .NET 9.0 SDK veya üzeri
- Docker ve Docker Compose (RabbitMQ için)

### Adım 1: RabbitMQ'yu Başlatın

```bash
# Docker Compose ile RabbitMQ'yu başlatın
docker-compose up -d

# RabbitMQ Management UI: http://localhost:15672
# Kullanıcı adı: guest
# Şifre: guest
```

### Adım 2: Projeyi Derleyin

```bash
# Solution'ı restore edin ve derleyin
dotnet restore
dotnet build
```

### Adım 3: Uygulamaları Başlatın

**Terminal 1 - API'yi başlatın:**
```bash
cd src/OrderManagement.API
dotnet run
```

API şu adreste çalışacaktır: `http://localhost:5000` veya `https://localhost:5001`

Swagger UI: `http://localhost:5000/swagger`

**Terminal 2 - Worker Service'i başlatın:**
```bash
cd src/OrderManagement.WorkerService
dotnet run
```

Worker Service arka planda RabbitMQ kuyruğunu dinleyecektir.

## 🚀 API Kullanımı

### Endpoints

#### 1. Yeni Sipariş Oluştur
```http
POST /api/orders
Content-Type: application/json

{
  "productName": "Laptop",
  "price": 15999.99
}
```

**Yanıt:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productName": "Laptop",
  "price": 15999.99,
  "status": "Pending",
  "createdDate": "2024-01-20T10:30:00Z",
  "updatedDate": null
}
```

#### 2. Sipariş Detayını Getir
```http
GET /api/orders/{id}
```

**Yanıt:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productName": "Laptop",
  "price": 15999.99,
  "status": "Completed",
  "createdDate": "2024-01-20T10:30:00Z",
  "updatedDate": "2024-01-20T10:30:05Z"
}
```

#### 3. Tüm Siparişleri Listele
```http
GET /api/orders
```

**Yanıt:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "productName": "Laptop",
    "price": 15999.99,
    "status": "Completed",
    "createdDate": "2024-01-20T10:30:00Z",
    "updatedDate": "2024-01-20T10:30:05Z"
  }
]
```

### Sipariş Durumları

- **Pending**: Sipariş oluşturuldu, işleme alınmayı bekliyor
- **Processing**: Sipariş işleniyor
- **Completed**: Sipariş başarıyla tamamlandı
- **Cancelled**: Sipariş iptal edildi

## 🔄 İş Akışı

1. **Sipariş Oluşturma**: Kullanıcı `POST /api/orders` endpoint'ine istek gönderir
2. **Veritabanına Kayıt**: Sipariş `Pending` statüsünde veritabanına kaydedilir
3. **RabbitMQ'ya Gönderim**: Sipariş bilgisi RabbitMQ kuyruğuna publish edilir
4. **Arka Plan İşleme**: Worker Service mesajı alır ve siparişi işler
5. **Durum Güncelleme**: Sipariş durumu `Processing` → `Completed` olarak güncellenir

## 🧪 Testleri Çalıştırma

```bash
# Tüm testleri çalıştır
dotnet test

# Detaylı çıktı ile çalıştır
dotnet test --logger "console;verbosity=detailed"

# Belirli bir test projesini çalıştır
dotnet test tests/OrderManagement.Tests/OrderManagement.Tests.csproj
```

### Test Kapsamı

- **OrderRepositoryTests**: Repository katmanı testleri
  - CRUD operasyonları
  - Veritabanı işlemleri
  
- **OrdersControllerTests**: Controller katmanı testleri
  - API endpoint testleri
  - Mock nesnelerle izolasyon

## 📊 RabbitMQ Management

RabbitMQ Management UI'a erişim:
- URL: http://localhost:15672
- Kullanıcı: `guest`
- Şifre: `guest`

Burada şunları görebilirsiniz:
- Queue istatistikleri
- Message flow
- Connection bilgileri

## 🔧 Yapılandırma

### API Configuration (appsettings.json)
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

### Worker Service Configuration
Worker Service aynı yapılandırma dosyasını kullanır.

## 📝 Notlar

- **In-Memory Database**: Uygulama her yeniden başlatıldığında veriler sıfırlanır. Production ortamında SQL Server, PostgreSQL gibi gerçek bir veritabanı kullanılmalıdır.

- **RabbitMQ**: Uygulamalar başlatılmadan önce RabbitMQ'nun çalışır durumda olması gerekmektedir.

- **Port Çakışması**: Eğer 5000/5001 portları kullanılıyorsa, `launchSettings.json` dosyasından port değiştirilebilir.

## 🚀 Production'a Hazırlık

Production ortamına geçiş için yapılması gerekenler:

1. **Gerçek Veritabanı**: SQL Server, PostgreSQL veya MongoDB kullanın
2. **Authentication/Authorization**: JWT veya Identity Server entegrasyonu
3. **Logging**: Serilog, Application Insights gibi profesyonel logging çözümleri
4. **Monitoring**: Health checks, metrics collection
5. **Error Handling**: Global exception handler
6. **Validation**: FluentValidation kütüphanesi
7. **Rate Limiting**: API rate limiting middleware
8. **Caching**: Redis cache katmanı
9. **Configuration**: Azure Key Vault veya AWS Secrets Manager

## 🐛 Sorun Giderme

### RabbitMQ Bağlantı Hatası
```
RabbitMQ.Client.Exceptions.BrokerUnreachableException
```
**Çözüm**: RabbitMQ'nun çalıştığından emin olun:
```bash
docker-compose ps
```

### Port Zaten Kullanılıyor
**Çözüm**: `launchSettings.json` dosyasından farklı bir port seçin veya çakışan uygulamayı kapatın.

## 👨‍💻 Geliştirici

Bu proje, e-ticaret platformları için modern bir sipariş yönetim sistemi case study'si olarak geliştirilmiştir.

## 📄 Lisans

Bu proje eğitim amaçlı geliştirilmiştir.

---

**Geliştirme Süresi**: 2 gün  
**Teknoloji Stack**: .NET 9, RabbitMQ, Entity Framework Core, xUnit  
**Mimari Pattern**: Clean Architecture
