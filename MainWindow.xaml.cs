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

        public MainWindow()
        {
            InitializeComponent();

            _plcService = new PlcSimulasyonService();
            _sqlService = new SqlService();

            // Simülasyondan gelen veri değişimlerini ekrana bağlama
            _plcService.OnDataChanged += PlcService_OnDataChanged;

            // Üretim tamamlandığında SQL'e asenkron loglama
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
                LedSensor1.Fill = data.ParcaVar1 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                LedSensor2.Fill = data.ParcaVar2 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
                LedPim.Fill = data.PimVar ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            });
        }

        private async void PlcService_OnUretimTamamlandi(MakineData data, bool isOk)
        {
            await _sqlService.KayitEkleAsync(data, isOk);
        }

        private void BtnBaslat_Click(object sender, RoutedEventArgs e)
        {
            _plcService.Baslat();
            BtnBaslat.IsEnabled = false;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            _plcService.SayacSifirla();
        }
    }
}