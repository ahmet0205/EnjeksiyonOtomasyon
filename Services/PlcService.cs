using System;
using System.Threading.Tasks;
using S7.Net;
using EnjeksiyonOtomasyon.Models;

namespace EnjeksiyonOtomasyon.Services
{
    public class PlcService
    {
        private Plc? _plc;
        public MakineData Data { get; private set; } = new MakineData();

        public event Action<MakineData>? OnDataChanged;
        public event Action<MakineData, bool>? OnUretimTamamlandi;

        private bool _bagli = false;
        private uint _sonOkAdet = 0;
        private uint _sonNokAdet = 0;

        // Siemens S7-1200 / S7-1500 Bağlantısı
        public bool Baglan(string ipAdresi = "192.168.0.1", short rack = 0, short slot = 1)
        {
            try
            {
                _plc = new Plc(CpuType.S71200, ipAdresi, rack, slot);
                _plc.Open();
                _bagli = _plc.IsConnected;

                if (_bagli)
                {
                    Task.Run(OkumaDongusu);
                }
                return _bagli;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PLC Bağlantı Hatası: " + ex.Message);
                return false;
            }
        }

        public void BaglantiKes()
        {
            _bagli = false;
            _plc?.Close();
        }

        private async Task OkumaDongusu()
        {
            while (_bagli && _plc != null && _plc.IsConnected)
            {
                try
                {
                    // TIA Portal DB1200 Okumaları
                    // DB1200.DBD14 -> OK Adet (DWord / UDInt)
                    var okObj = await _plc.ReadAsync(DataType.DataBlock, 1200, 14, VarType.DWord, 1);
                    Data.OkUrunAdet = Convert.ToUInt32(okObj);

                    // DB1200.DBD18 -> NOK Adet (DWord / UDInt)
                    var nokObj = await _plc.ReadAsync(DataType.DataBlock, 1200, 18, VarType.DWord, 1);
                    Data.NokUrunAdet = Convert.ToUInt32(nokObj);

                    // DB1200.DBB8 -> Model No (Byte)
                    var modelObj = await _plc.ReadAsync(DataType.DataBlock, 1200, 8, VarType.Byte, 1);
                    Data.ModelNo = Convert.ToByte(modelObj);

                    // DB1200.DBX0.0 -> Otomatik Mod
                    Data.OtomatikMod = (bool)await _plc.ReadAsync("DB1200.DBX0.0");

                    // DB1200.DBD22, 26, 30 -> Süreler
                    Data.SogutmaSuresi = Convert.ToInt32(await _plc.ReadAsync(DataType.DataBlock, 1200, 22, VarType.DWord, 1)) / 1000;
                    Data.Enjeksiyon1Suresi = Convert.ToInt32(await _plc.ReadAsync(DataType.DataBlock, 1200, 26, VarType.DWord, 1)) / 1000;
                    Data.Enjeksiyon2Suresi = Convert.ToInt32(await _plc.ReadAsync(DataType.DataBlock, 1200, 30, VarType.DWord, 1)) / 1000;

                    // Giriş Bitleri (Sensörler)
                    Data.ParcaVar1 = (bool)await _plc.ReadAsync("I0.0");
                    Data.ParcaVar2 = (bool)await _plc.ReadAsync("I0.1");
                    Data.PimVar = (bool)await _plc.ReadAsync("I0.2");

                    Data.DurumMesaji = Data.OtomatikMod ? "OTOMATİK ÇALIŞIYOR" : "MANUEL MOD";

                    // SQL Tetikleme Mantığı: Sayaç arttığında otomatik log atar
                    if (Data.OkUrunAdet > _sonOkAdet)
                    {
                        _sonOkAdet = Data.OkUrunAdet;
                        OnUretimTamamlandi?.Invoke(Data, true);
                    }
                    else if (Data.NokUrunAdet > _sonNokAdet)
                    {
                        _sonNokAdet = Data.NokUrunAdet;
                        OnUretimTamamlandi?.Invoke(Data, false);
                    }

                    OnDataChanged?.Invoke(Data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Okuma Döngüsü Hatası: " + ex.Message);
                }

                await Task.Delay(200); // 200 ms tarama periyodu
            }
        }

        // PLC'ye Yazma (Sayacı Sıfırla Komutu)
        public async Task SayacSifirlaAsync()
        {
            if (_plc != null && _plc.IsConnected)
            {
                await _plc.WriteAsync(DataType.DataBlock, 1200, 14, (uint)0);
                await _plc.WriteAsync(DataType.DataBlock, 1200, 18, (uint)0);
            }
        }
    }
}