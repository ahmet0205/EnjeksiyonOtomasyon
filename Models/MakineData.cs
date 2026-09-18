using System;

namespace EnjeksiyonOtomasyon.Models
{
    public class MakineData
    {
        // Sayaçlar (SQL'e yazılacak veriler)
        public uint OkUrunAdet { get; set; } = 0;       // DB1200.DBD14 (UDInt)
        public uint NokUrunAdet { get; set; } = 0;      // DB1200.DBD18 (UDInt)
        public byte ModelNo { get; set; } = 1;          // DB1200.DBB8 (USInt)

        // Makine Durumu
        public bool OtomatikMod { get; set; } = true;   // DB1200.DBX0.0
        public short OtomatikStatus { get; set; } = 1;  // DB1200.DBW12 (Adım no)
        public string DurumMesaji { get; set; } = "SİSTEM HAZIR";

        // Sensör Varlıkları (Görseldeki 3 Kırmızı/Yeşil Nokta)
        public bool ParcaVar1 { get; set; } = false;    // vizibiliyt.Baru 1
        public bool ParcaVar2 { get; set; } = false;    // vizibiliyt.Boru 2
        public bool PimVar { get; set; } = false;       // vizibiliyt.Pim

        // Süreler
        public int Enjeksiyon1Suresi { get; set; } = 15;
        public int Enjeksiyon2Suresi { get; set; } = 15;
        public int SogutmaSuresi { get; set; } = 60;
    }
}