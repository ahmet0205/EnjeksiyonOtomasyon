using System;
using System.Threading.Tasks;
using EnjeksiyonOtomasyon.Models;

namespace EnjeksiyonOtomasyon.Services
{
    public class PlcSimulasyonService
    {
        public MakineData Data { get; private set; } = new MakineData();

        // Arayüzü güncellemek için tetiklenen olay
        public event Action<MakineData> OnDataChanged;

        // Parça bittiğinde SQL'e kayıt tetikleyecek olay
        public event Action<MakineData, bool> OnUretimTamamlandi; // bool isOk

        private bool _calisiyor = false;

        public void Baslat()
        {
            if (_calisiyor) return;
            _calisiyor = true;
            Task.Run(SimulasyonDongusu);
        }

        public void Durdur()
        {
            _calisiyor = false;
        }

        public void SayacSifirla()
        {
            Data.OkUrunAdet = 0;
            Data.NokUrunAdet = 0;
            OnDataChanged?.Invoke(Data);
        }

        private async Task SimulasyonDongusu()
        {
            Random rnd = new Random();

            while (_calisiyor)
            {
                // Adım 1: Gövde Bekleniyor
                Data.OtomatikStatus = 1;
                Data.DurumMesaji = "GÖVDELİ YERLEŞTİRİN";
                Data.ParcaVar1 = false; Data.ParcaVar2 = false; Data.PimVar = false;
                OnDataChanged?.Invoke(Data);
                await Task.Delay(2500);

                if (!_calisiyor) break;

                // Adım 2: Sensörler algıladı (Noktalar yeşil)
                Data.OtomatikStatus = 2;
                Data.DurumMesaji = "GÖVDE VE PİMLER ALGILANDI";
                Data.ParcaVar1 = true; Data.ParcaVar2 = true; Data.PimVar = true;
                OnDataChanged?.Invoke(Data);
                await Task.Delay(1500);

                if (!_calisiyor) break;

                // Adım 3: Enjeksiyon yapılıyor
                Data.OtomatikStatus = 3;
                Data.DurumMesaji = "ENJEKSİYON VE KLİPS ÇAKMA DEVREDE...";
                OnDataChanged?.Invoke(Data);
                await Task.Delay(3000);

                if (!_calisiyor) break;

                // Adım 4: Tamamlandı - Kalite Kontrol (%90 OK, %10 NOK)
                bool isOk = rnd.Next(1, 11) <= 9;
                if (isOk)
                {
                    Data.OkUrunAdet++;
                    Data.DurumMesaji = "PARÇA ONAYLANDI (OK) - ALINIZ";
                }
                else
                {
                    Data.NokUrunAdet++;
                    Data.DurumMesaji = "HATALI PARÇA (NOK) - AYIRINIZ";
                }

                OnDataChanged?.Invoke(Data);
                OnUretimTamamlandi?.Invoke(Data, isOk);

                await Task.Delay(2500);
            }
        }
    }
}