<p align="right">
  <a href="README.md">English</a> | <strong>Türkçe</strong>
</p>


---

# Ürün Satış & E-Ticaret Portalı

> **Kapsamlı bir E-Ticaret Çözümü:** Güvenli yönetim paneli, gerçek zamanlı bildirimler ve ölçeklenebilir repository mimarisi ile **.NET Core MVC** kullanılarak geliştirilmiştir.

---

##  Genel Bakış
Bu proje, kesintisiz ürün yönetimi ve satış işlemleri için tasarlanmış tam kapsamlı (full-stack) bir web uygulamasıdır. İki farklı arayüz sunar:
* **Kullanıcı Portalı:** Müşterilerin ürünleri incelemesi ve etkileşime girmesi için.
* **Gelişmiş Yönetim Paneli:** Mağaza yöneticilerinin stok ve içerik kontrolü yapabileceği güvenli dashboard.

Sistem, endüstri standardı temiz kod prensiplerine odaklanarak **.NET 8.0** ve **Model-View-Controller (MVC)** tasarım deseni ile inşa edilmiştir.

##  Teknoloji Yığını ve Desenler
* **Framework:** .NET Core MVC
* **Veritabanı:** MSSQL Server (Code-First yaklaşımı)
* **Frontend:** Bootstrap 4+, JQuery, AJAX
* **Gerçek Zamanlı İletişim:**  **SignalR** (Canlı güncellemeler ve bildirimler için entegre edildi)
* **Güvenlik:** 
    * **Cookie Tabanlı Kimlik Doğrulama** ve **ASP.NET Core Identity**
    * **Rol Tabanlı Erişim Kontrolü (RBAC)** (Yönetici/Kullanıcı yetkileri)
* **Tasarım Deseni:** Veri katmanını soyutlamak ve test edilebilirliği artırmak için **Repository Pattern**.

##  Temel Özellikler
* **Güvenli Üyelik:** Kayıt ve giriş işlemleri için tam kapsamlı ASP.NET Core Identity uygulaması.
* **Gelişmiş Yönetim Paneli:** Ürünlerin, kategorilerin ve kullanıcı rollerinin yönetimi için özelleşmiş arayüz.
* **AJAX Entegrasyonu:** Sayfa yenilenmeden asenkron veri işlemleri ile optimize edilmiş kullanıcı deneyimi.
* **Gerçek Zamanlı Bildirimler:** Yönetim panelinde canlı izleme için entegre edilmiş **SignalR** yapısı.
* **Duyarlı Tasarım:** **Bootstrap** sayesinde mobil ve masaüstü cihazlarla tam uyumlu arayüz.

##  Teknik Detaylar
* **Sorumlulukların Ayrılması:** UI mantığını veritabanı modellerinden ayırmak ve güvenliği artırmak için **ViewModels** yapısı kullanıldı.
* **Ölçeklenebilir Mimari:** Proje büyüdükçe kod tabanının bakımı kolay kalsın diye **Repository Pattern** uygulandı.
* **Veritabanı Versiyonlama:** Tüm şema değişiklikleri, sorunsuz bir geliştirme iş akışı için **EF Core Migrations** üzerinden yönetildi.

---


