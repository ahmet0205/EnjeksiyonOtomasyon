using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EnjeksiyonOtomasyon.Models;

namespace EnjeksiyonOtomasyon.Database
{
    public class SqlService
    {
        // Doğrudan sizin ekranınızdaki LocalDB sunucusuna bağlanan connection string
        private readonly string _connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=EnjeksiyonDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public async Task KayitEkleAsync(MakineData data, bool isOk)
        {
            string query = @"
                INSERT INTO UretimKayitlari 
                (KayitTarihi, ModelNo, OkUrunAdet, NokUrunAdet, Sonuc, Enjeksiyon1Suresi, Enjeksiyon2Suresi, SogutmaSuresi)
                VALUES 
                (@tarih, @model, @okAdet, @nokAdet, @sonuc, @enj1, @enj2, @sogutma)";

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tarih", DateTime.Now);
                        cmd.Parameters.AddWithValue("@model", data.ModelNo);
                        cmd.Parameters.AddWithValue("@okAdet", (int)data.OkUrunAdet);
                        cmd.Parameters.AddWithValue("@nokAdet", (int)data.NokUrunAdet);
                        cmd.Parameters.AddWithValue("@sonuc", isOk ? "OK" : "NOK");
                        cmd.Parameters.AddWithValue("@enj1", data.Enjeksiyon1Suresi);
                        cmd.Parameters.AddWithValue("@enj2", data.Enjeksiyon2Suresi);
                        cmd.Parameters.AddWithValue("@sogutma", data.SogutmaSuresi);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQL Hatası: " + ex.Message);
            }
        }
    }
}