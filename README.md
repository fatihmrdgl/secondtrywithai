# mimcrm

Sigorta acenteciliği sektöründe satılan poliçeleri, müşterileri ve kullanıcıları yönetmek için hazırlanmış web tabanlı bir mikroservis örnek projesidir.

## Mimari

Proje çok katmanlı ve ilerde geliştirilmeye uygun olacak şekilde tasarlanmıştır:

- **Frontend:** React.js (Vite)
- **Backend:** .NET 8 Minimal API + Entity Framework Core + MySQL
- **Mimari:** Mikroservis (Identity, Customer, Product)
- **Veritabanı:** MySQL (servis başına ayrılmış DB)

### Servisler

- `IdentityService`: Kullanıcı kaydı / giriş işlemleri, kullanıcı listesi
- `CustomerService`: Müşteri CRUD işlemleri
- `ProductService`: Ürün/Poliçe CRUD işlemleri

Her servis için katmanlar:

- `*.Domain` (Entity modeller)
- `*.Application` (DTO ve iş akışı sözleşmeleri)
- `*.Infrastructure` (EF Core DbContext)
- `*.API` (HTTP endpointleri)

## Ekranlar ve Modüller

Frontend içinde aşağıdaki fonksiyonlar hazırdır:

- **Karşılama ekranı** (`mimcrm` başlığı ve hoş geldiniz metni)
- **Yeni kullanıcı / Login menüsü**
- **Kullanıcı yönetimi** (aktif kullanıcı bilgisi + çıkış)
- **Müşteri yönetimi** (listeleme + yeni müşteri ekleme)
- **Ürün yönetimi** (listeleme + yeni ürün/poliçe ekleme)

## Kurulum

### 1) Docker ile çalıştırma (önerilen)

```bash
docker compose up --build
```

Servis URL'leri:

- Frontend: `http://localhost:5173`
- Identity API: `http://localhost:5001`
- Customer API: `http://localhost:5002`
- Product API: `http://localhost:5003`

### 2) Lokal geliştirme (docker olmadan)

> Not: Bu ortamda `dotnet` kurulu değilse backend derlenemez. Uygun .NET SDK yüklü makinede çalıştırınız.

- Her API projesini ayrı terminalde çalıştırın.
- Frontend için:

```bash
cd frontend
npm install
npm run dev
```

## Gelecek geliştirmeler

- JWT ve refresh token altyapısı
- API Gateway (YARP/Ocelot) ve merkezi auth doğrulama
- Event bus (RabbitMQ/Kafka) ile servisler arası olay tabanlı iletişim
- Logging/Tracing (OpenTelemetry + Serilog + Grafana)
- Role-based yetkilendirme
- Poliçe yenileme, tahsilat takibi ve teklif yönetimi modülleri

## Klasör yapısı

```text
backend/
  src/
    IdentityService/
    CustomerService/
    ProductService/
frontend/
docker-compose.yml
```
