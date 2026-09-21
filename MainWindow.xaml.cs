using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using EnjeksiyonOtomasyon.Database;
using EnjeksiyonOtomasyon.Models;
using EnjeksiyonOtomasyon.Services;

namespace EnjeksiyonOtomasyon
{
    public partial class MainWindow : Window
    {
        private readonly PlcSimulasyonService _plcService;
        private readonly SqlService _sqlService;
        private readonly ObservableCollection<UretimKaydiGorunum> _canliKayitlar;
        private int _kayitIdSayac = 1;

        public MainWindow()
        {
            InitializeComponent();

            _plcService = new PlcSimulasyonService();
            _sqlService = new SqlService();

            // Tablo veri kaynağını bağlama
            _canliKayitlar = new ObservableCollection<UretimKaydiGorunum>();
            DgUretimKayitlari.ItemsSource = _canliKayitlar;

            // Simülasyondan gelen veri değişimlerini ekrana bağlama
            _plcService.OnDataChanged += PlcService_OnDataChanged;

            // Üretim tamamlandığında SQL'e asenkron loglama ve tabloya satır ekleme
            _plcService.OnUretimTamamlandi += PlcService_OnUretimTamamlandi;
        }

        private void PlcService_OnDataChanged(MakineData data)
        {
            Dispatcher.Invoke(() =>
            {
                TxtOkAdet.Text = data.OkUrunAdet.ToString();
                TxtNokAdet.Text = data.NokUrunAdet.ToString();
                TxtDurum.Text = data.DurumMesaji;

                TxtEnj1.Text = data.Enjeksiyon1Suresi.ToString();
                TxtEnj2.Text = data.Enjeksiyon2Suresi.ToString();
                TxtSogutma.Text = data.SogutmaSuresi.ToString();

                // Sensör renkleri (Yeşil / Kırmızı)
                LedSensor1.Fill = data.ParcaVar1 ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Color.FromRgb(231, 76, 60));
                LedSensor2.Fill = data.ParcaVar2 ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Color.FromRgb(231, 76, 60));
                LedPim.Fill = data.PimVar ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Color.FromRgb(231, 76, 60));
            });
        }

        private async void PlcService_OnUretimTamamlandi(MakineData data, bool isOk)
        {
            // 1. SQL Veritabanına asenkron kaydet
            await _sqlService.KayitEkleAsync(data, isOk);

            // 2. Canlı arayüz tablosuna XAML sütunlarıyla birebir eşleşen satır düşür
            Dispatcher.Invoke(() =>
            {
                _canliKayitlar.Insert(0, new UretimKaydiGorunum
                {
                    Id = _kayitIdSayac++,
                    KayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                    ModelNo = $"Model {data.ModelNo}",
                    Sonuc = isOk ? "OK" : "NOK",
                    Enjeksiyon1Suresi = data.Enjeksiyon1Suresi,
                    Enjeksiyon2Suresi = data.Enjeksiyon2Suresi,
                    SogutmaSuresi = data.SogutmaSuresi
                });
            });
        }

        private void BtnBaslat_Click(object sender, RoutedEventArgs e)
        {
            _plcService.Baslat();
            BtnBaslat.IsEnabled = false;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            _plcService.SayacSifirla();
            _canliKayitlar.Clear();
            _kayitIdSayac = 1;
        }
    }

    // XAML Binding="{Binding ...}" isimleriyle birebir eşleşen model sınıfı
    public class UretimKaydiGorunum
    {
        public int Id { get; set; }
        public string KayitTarihi { get; set; } = string.Empty;
        public string ModelNo { get; set; } = string.Empty;
        public string Sonuc { get; set; } = string.Empty;
        public int Enjeksiyon1Suresi { get; set; }
        public int Enjeksiyon2Suresi { get; set; }
        public int SogutmaSuresi { get; set; }
    }
}