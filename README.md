# Endüstriyel Plastik Enjeksiyon & Klips Çakma SCADA / HMI Sistemi

Bu proje, plastik enjeksiyon ve klips çakma makinelerinin operasyonel verilerini anlık olarak izlemek, operatör panelini (HMI) simüle etmek ve üretim çıktılarını ilişkisel veritabanına loglamak amacıyla geliştirilmiş bir **WPF (.NET 8)** masaüstü SCADA uygulamasıdır.

---

## 🛠 Mimari ve Özellikler

* **Canlı İzleme ve SCADA Ekranı:** OK / NOK parça sayaçları, makine adım durumları ve çevrim sürelerinin (Enjeksiyon 1-2, Soğutma) canlı takibi.
* **Sensör Görselleştirme:** Gövde varlık ve kilit sensörlerinin anlık durum kontrolü (LED indikatörler).
* **Asenkron SQL Loglama:** Üretilen her parçanın tarih, saat, kalite durumu (OK/NOK) ve proses süreleriyle birlikte Microsoft SQL Server'a kaydedilmesi.
* **Çift Çalışma Modu:**
  * `PlcSimulasyonService`: Fiziksel donanım olmadan uçtan uca senaryo testi sağlayan durum makinesi (State Machine).
  * `PlcService (S7NetPlus)`: Siemens S7-1200 / S7-1500 PLC'ler ile DB1200 veri bloğu üzerinden TCP/IP haberleşmesi.

---

## 📂 PLC DB1200 Veri Haritası

| Değişken Adı | Veri Tipi | PLC Adresi | Açıklama |
| :--- | :--- | :--- | :--- |
| `OtomatikMod` | Bool | DB1200.DBX0.0 | İstasyon çalışma modu |
| `ModelNo` | USInt / Byte | DB1200.DBB8 | Aktif çalışan parça modeli |
| `OtomatikStatus` | Int | DB1200.DBW12 | İstasyon anlık çevrim adımı |
| `OkUrunAdet` | UDInt | DB1200.DBD14 | Kalite onaylı ürün sayacı |
| `NokUrunAdet` | UDInt | DB1200.DBD18 | Hatalı ürün sayacı |
| `SogutmaSuresi` | UDInt / DWord | DB1200.DBD22 | Kalıp soğutma süresi |
| `Enjeksiyon1Suresi` | UDInt / DWord | DB1200.DBD26 | 1. Enjeksiyon aşaması süresi |
| `Enjeksiyon2Suresi` | UDInt / DWord | DB1200.DBD30 | 2. Enjeksiyon aşaması süresi |

---

## ⚙️ Kurulum ve Çalıştırma

1. **Depoyu Klonlayın:**
   ```bash
   git clone [https://github.com/ahmet0205/EnjeksiyonOtomasyon.git](https://github.com/ahmet0205/EnjeksiyonOtomasyon.git)
1. Veritabanı Hazırlığı:

Database/TabloOlustur.sql dosyasını SQL Server Management Studio (SSMS) veya Visual Studio SQL Server Nesne Gezgini üzerinde çalıştırarak EnjeksiyonDB veritabanını ve UretimKayitlari tablosunu oluşturun.

Projeyi Çalıştırın:

Çözümü Visual Studio 2022 (.NET 8) ile açın.

F5 tuşuna basarak başlatın ve SİMÜLASYONU BAŞLAT butonuna tıklayın.

💻 Kullanılan Teknolojiler
Platform: C# / .NET 8 (WPF)

PLC Haberleşme: S7netplus (Siemens S7 Protocol)

Veritabanı: Microsoft SQL Server (LocalDB) / ADO.NET (Microsoft.Data.SqlClient)

Versiyon Kontrol: Git & GitHub