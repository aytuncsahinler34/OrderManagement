Sipariş Yönetim Sistemi 

Modern e-ticaret platformları için geliştirilmiş, ölçeklenebilir bir sipariş yönetim backend servisi. Clean Architecture prensipleri ve asenkron mesajlaşma ile tasarlanmıştır. 

Teknoloji Stack 

.NET 9.0 

Entity Framework Core (In-Memory) 

RabbitMQ 

xUnit & Moq 

Mimari Yapı 

Proje, katmanlı mimari prensiplerine uygun şekilde organize edilmiştir: 

├── OrderManagement.Core # Domain katmanı ├── OrderManagement.Infrastructure # Data access ve messaging ├── OrderManagement.API # REST API ├── OrderManagement.WorkerService # Background processing └── OrderManagement.Tests # Unit tests  

Tasarım Kararları 

Neden Clean Architecture? Katmanlar arası bağımlılıkları minimize ederek, değişime açık ve test edilebilir bir yapı oluşturmak istedim. Core katmanı hiçbir external dependency içermiyor. 

Neden RabbitMQ? Sipariş işleme süreçlerini asenkron hale getirerek sistem performansını artırmak ve servisler arası bağımlılığı azaltmak için message queue kullandım. 

Neden Guid ID? Distributed sistemlerde ID çakışmalarını önlemek ve database bağımlılığını azaltmak için Guid tercih ettim. Bu sayede client-side ID generation mümkün oluyor. 

Kurulum 

Gereksinimler 

.NET 9.0 SDK 

Docker Desktop 

RabbitMQ Başlatma 

docker-compose up -d  

RabbitMQ Management: http://localhost:15672 (guest/Qweasdzxc123) 

Uygulamayı Çalıştırma 

API: 

cd src/OrderManagement.API dotnet run  

Worker Service: 

cd src/OrderManagement.WorkerService dotnet run  

(http)  API: http://localhost:5131 | Swagger: http://localhost:5131/swagger   
(https)  API: https://localhost:7186 | Swagger: https://localhost:7186/swagger 
API Endpoints 

Method 

Endpoint 

Açıklama 

POST 

/api/orders 

Yeni sipariş oluşturur 

GET 

/api/orders/{id} 

Sipariş detayını getirir 

GET 

/api/orders 

Tüm siparişleri listeler 

Örnek Request 

curl -X POST http://localhost:5131/api/orders \ -H "Content-Type: application/json" \ -d '{"productName":"Laptop","price":15999.99}'  

Sipariş İşlem Akışı 

Client, API'ye POST isteği gönderir 

Order entity oluşturulur (Status: Pending) 

Database'e kaydedilir 

RabbitMQ kuyruğuna publish edilir 

Worker Service mesajı consume eder 

Sipariş işlenir (Status: Processing → Completed) 

Database güncellenir 

Test 

dotnet test  

Test kapsamı: 

Repository CRUD operations 

Controller endpoint behaviors 

Mapping logic 

Business rules 

Notlar 

In-Memory Database: Development kolaylığı için kullanıldı. Production'da SQL Server veya PostgreSQL önerilir. 

FluentValidation: Input validation için tercih ettim. Data Annotations'a göre daha esnek ve test edilebilir. 

Index Stratejisi: GetAll endpoint'inde CreatedDate'e göre sıralama yaptığım için bu alana index ekledim. Bu sayede büyük veri setlerinde performans kaybı yaşanmaz. 

Geliştirme Süreci Hakkında 

Bu projeyi geliştirirken şu noktalara özen gösterdim: 

Separation of Concerns: Her katmanın sorumluluğu net şekilde ayrılmış 

SOLID Principles: Özellikle Dependency Inversion ve Single Responsibility 

Testability: Unit test yazımını kolaylaştıracak bir yapı 

Maintainability: Kodun okunabilirliği ve genişletilebilirliği 


Geliştirme Süresi: 4 saat 

Odak: Clean code, scalability, maintainability 