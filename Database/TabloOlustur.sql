CREATE DATABASE EnjeksiyonDB;
GO

USE EnjeksiyonDB;
GO

CREATE TABLE UretimKayitlari (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    KayitTarihi DATETIME DEFAULT GETDATE(),
    ModelNo TINYINT NOT NULL,
    OkUrunAdet INT NOT NULL,
    NokUrunAdet INT NOT NULL,
    Sonuc VARCHAR(10) NOT NULL, -- 'OK' veya 'NOK'
    Enjeksiyon1Suresi INT,
    Enjeksiyon2Suresi INT,
    SogutmaSuresi INT
);
GO
SELECT * FROM UretimKayitlari ORDER BY Id DESC;