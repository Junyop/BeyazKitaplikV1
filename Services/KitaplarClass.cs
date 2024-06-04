using BeyazKitaplikV1.DTO;
using BeyazKitaplikV1.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.EnterpriseServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Web.Mvc;



namespace BeyazKitaplikV1.Services
{
    public class KitaplarClass : IKitaplarClass
    {
        BeyazKitaplikEntities db = new BeyazKitaplikEntities();
        public List<KitaplarDTO> GetBooks( List<int> ids = null)
        {
            var list = new List<KitaplarDTO>();

            try
            {
                using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                {
                    string query = @"
                        SELECT 
                            Kit.KitapID AS KITAPID,
                            Kit.Kitap_Adi AS KITAPADI,
                            Kit.YazarID AS YAZARID,
                            Yaz.Yazar_Adi AS YAZARADI,
                            Kit.YayinID AS YAYINID,
                            Yay.Yayin_Adi AS YAYINADI,
                            Kit.Baski_no AS BASKINO,
                            Kit.Baski_Tarihi AS BASKITARIHI,
                            Kit.Sayfa AS SAYFA,
                            Kit.Alinma_Tarihi AS ALINMATARIHI,
                            Kit.Alindigi_Yer AS ALINDIGIYER
                        FROM 
                            KITAPLAR AS Kit
                        JOIN 
                            YAZARLAR AS Yaz ON Yaz.YazarID = Kit.YazarID
                        JOIN 
                            YAYINLAR AS Yay ON Yay.YayinID = Kit.YayinID";

                    if (ids != null)
                    {
                        if (ids.Count == 1)
                        {
                            query += " WHERE Kit.KitapID=@kitapID";
                        }
                        else if (ids.Count > 1)
                        {
                            query += " WHERE Kit.KitapID IN (";
                            string values = string.Join(",", Enumerable.Range(0, ids.Count).Select(i => $"@kitapID{i}"));
                            query += values;
                            query += ")";
                        }
                    }

                    using (SqlCommand target = new SqlCommand(query, myConn))
                    {
                        if (ids != null)
                        {
                            if (ids.Count == 1)
                            {
                                target.Parameters.AddWithValue("@kitapID", ids[0]);
                            }
                            else if (ids.Count > 1)
                            {
                                for (int i = 0; i < ids.Count; i++)
                                {
                                    target.Parameters.AddWithValue($"@kitapID{i}", ids[i]);
                                }
                            }
                        }
                        myConn.Open();
                        using (SqlDataReader dr = target.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                KitaplarDTO kitap = new KitaplarDTO();

                                kitap.KitapID = dr.GetInt32(dr.GetOrdinal("KITAPID"));
                                kitap.KitapAdi = dr.GetString(dr.GetOrdinal("KITAPADI"));
                                kitap.YazarID = dr.GetInt32(dr.GetOrdinal("YAZARID"));
                                kitap.YazarAdi = dr.GetString(dr.GetOrdinal("YAZARADI"));
                                kitap.YayinID = dr.GetInt32(dr.GetOrdinal("YAYINID"));
                                kitap.YayinAdi = dr.GetString(dr.GetOrdinal("YAYINADI"));
                                kitap.BaskiNo = dr.IsDBNull(dr.GetOrdinal("BASKINO")) ? 0 : dr.GetInt32(dr.GetOrdinal("BASKINO"));
                                kitap.Sayfa = dr.GetInt32(dr.GetOrdinal("SAYFA"));
                                if (!dr.IsDBNull(dr.GetOrdinal("ALINDIGIYER")))
                                {
                                    kitap.AlindigiYer = dr.GetString(dr.GetOrdinal("ALINDIGIYER"));
                                }
                                else
                                {
                                    kitap.AlindigiYer = string.Empty;
                                }
                                if (!dr.IsDBNull(dr.GetOrdinal("BASKITARIHI")))
                                {
                                    kitap.BaskiTarihi = dr.GetDateTime(dr.GetOrdinal("BASKITARIHI"));
                                    kitap.BaskiTarihiText = kitap.BaskiTarihi.Value.ToString("dd MMMM yyyy");
                                }

                                if (!dr.IsDBNull(dr.GetOrdinal("ALINMATARIHI")))
                                {
                                    kitap.AlinmaTarihi = dr.GetDateTime(dr.GetOrdinal("ALINMATARIHI"));
                                    kitap.AlinmaTarihiText = kitap.AlinmaTarihi.Value.ToString("dd MMMM yyyy");
                                }

                                list.Add(kitap);
                            }
                        }
                    }
                }

                foreach (var kitap in list)
                {
                    var altTur = GetTypes(kitap.KitapID);
                    if (altTur != null && altTur.KitapID == kitap.KitapID)
                    {
                        kitap.AltTurAdis = altTur.AltTurAdis;
                        kitap.AltTurIDs = altTur.AltTurIDs;
                        kitap.TurAdi = altTur.TurAdi;
                        kitap.TurID = altTur.TurID;
                        kitap.SubTypes = altTur.SubTypes;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"getKitaplar metodunu çağırırken hata alındı.\nKitapların listelemesi yapılamadı: {e.Message}");
                throw;
            }
            return list;
        }

        public KitaplarDTO GetTypes(int ID)
        {
            KitaplarDTO kitaplarDTO = new KitaplarDTO();

            try
            {
                string query = @"
            SELECT Kit.KitapID AS KITAPID,
            Kit.Kitap_Adi AS KITAPADI,
            Kti.TurID AS TURID,
            Tur.Tur_Adi AS TURADI,
            STRING_AGG(Alt.Alt_Tur_Adi, '-') AS ALTTURADI,
            STRING_AGG(Kti.AltTurID, '-') AS ALTTURID
            FROM KITAPLAR AS Kit
            JOIN KITAP_TUR_ILISKISI AS Kti ON Kti.KitapID = Kit.KitapID
            JOIN TURLER AS Tur ON Tur.TurID = Kti.TurID
            JOIN ALTTURLER AS Alt ON Alt.Alt_TurID = Kti.AltTurID
            WHERE Kit.KitapID=@kitapID
            GROUP BY Kit.KitapID, Kit.Kitap_Adi, Kti.TurID, Tur.Tur_Adi";

                query = query.Replace("@kitapID", ID.ToString());

                using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                {
                    using (SqlCommand target = new SqlCommand(query, myConn))
                    {
                        target.CommandType = CommandType.Text;

                        myConn.Open();
                        using (SqlDataReader dr = target.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    kitaplarDTO.KitapID = (int)dr["KITAPID"];
                                    kitaplarDTO.KitapAdi = (string)dr["KITAPADI"];
                                    kitaplarDTO.TurID = dr["TURID"] != DBNull.Value ? (int)dr["TURID"] : 0;
                                    kitaplarDTO.TurAdi = (string)dr["TURADI"] ?? "";
                                    kitaplarDTO.SubTypes = dr["ALTTURADI"].ToString() ?? "";
                                    kitaplarDTO.AltTurIDs = dr["ALTTURID"].ToString().Split('-').Select(int.Parse).ToList();
                                    kitaplarDTO.AltTurAdis = kitaplarDTO.SubTypes.Split('-').ToList();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata: {e.Message}. Kitap türleri getirilirken bir hata oluştu.");
                throw;
            }

            return kitaplarDTO;
        }

        public void CreateBook(KitaplarDTO book)
        {
            try
            {
                if (book != null)
                {
                    using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                    {
                        string query = @"DECLARE @kitapId INT;
                            INSERT INTO KITAPLAR (Kitap_Adi,YazarID,YayinID,Baski_no,Baski_Tarihi,Sayfa,Alinma_Tarihi,Alindigi_Yer)
                            VALUES(@kitapAdi,@yazarId,@yayinId,@baskiNo,@baskiTarihi,@sayfa,@alinmaTarihi,@alindigiYer);
                            SET @kitapId = SCOPE_IDENTITY();
                            INSERT INTO KITAP_TUR_ILISKISI(KitapID,TurID,AltTurID)
                            VALUES";

                        if (book.AltTurIDs.Count > 0)
                        {
                            string values = string.Join(",", Enumerable.Range(0, book.AltTurIDs.Count).Select(i => $"(@kitapId,@turId,@altTurID{i})"));
                            query += values;
                        }

                        using (SqlCommand myCommand = new SqlCommand(query, myConn))
                        {
                            myCommand.Parameters.AddWithValue("@kitapAdi", book.KitapAdi);
                            myCommand.Parameters.AddWithValue("@yazarId", book.YazarID);
                            myCommand.Parameters.AddWithValue("@yayinId", book.YayinID);
                            myCommand.Parameters.AddWithValue("@baskiNo", book.BaskiNo);
                            myCommand.Parameters.AddWithValue("@baskiTarihi", book.BaskiTarihi);
                            myCommand.Parameters.AddWithValue("@sayfa", book.Sayfa);
                            myCommand.Parameters.AddWithValue("@alinmaTarihi", book.AlinmaTarihi);
                            myCommand.Parameters.AddWithValue("@alindigiYer", book.AlindigiYer);
                            myCommand.Parameters.AddWithValue("@turId", book.TurID);

                            if (book.AltTurIDs.Count > 0)
                            {
                                for (int i = 0; i < book.AltTurIDs.Count; i++)
                                {
                                    myCommand.Parameters.AddWithValue($"@altTurID{i}", book.AltTurIDs[i]);
                                    
                                }
                                string values = string.Join(",", Enumerable.Range(0, book.AltTurIDs.Count).Select(i => $"(@kitapId,@turId,@altTurID{i})"));
                                query += values;
                            }

                            myConn.Open();
                            myCommand.ExecuteNonQuery();
                            MessageBox.Show("Kitap başarıyla oluşturuldu.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Hata: {e.Message}. Kitap oluşturma işlemi başarısız oldu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        public void DeleteBook(int Id)
        {
            try
            {
                if (Id != 0)
                {
                    string query = @"DELETE FROM KITAPLAR WHERE KitapID = @kitapId;
                                     DELETE FROM KITAP_TUR_ILISKISI WHERE KitapID=@kitapId";
                    SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True");
                    SqlCommand myCommand = new SqlCommand(query, myConn);

                    // KitapID parametresini ekle
                    myCommand.Parameters.AddWithValue("@kitapId", Id);

                    myConn.Open();
                    myCommand.ExecuteNonQuery();

                    //MessageBox.Show("Veritabanı başarıyla güncellendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    myConn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata: {e.Message}. Kitap silinirken bir hata oluştu.");
                throw;
            }
        }
        public void UpdateBook (KitaplarDTO data)
        {
            try
            {
                if(data != null)
                {
                    var query1 = new StringBuilder("UPDATE KITAPLAR SET"); 
                    var parameters = new List<SqlParameter>();

                    var query = @"
                           UPDATE KITAPLAR SET ";
                    if (data != null)
                    {
                        if (data.KitapAdi != "" && data.KitapAdi != null)
                        {
                            query += "Kitap_Adi='@kitapAdi',";
                            query = query.Replace("@kitapAdi", data.KitapAdi.ToString());
                        }
                        if (data.YazarID != 0 && data.YazarID != null)
                        {
                            query += "YazarID = @yazarId,";
                            query = query.Replace("@yazarId", data.YazarID.ToString());
                        }
                        
                        if (data.YayinID != 0 && data.YayinID != null)
                        {
                            query += " YayinID = @yayinId,";
                            query = query.Replace("@yayinId", data.YayinID.ToString());
                        }
                        if (data.BaskiNo != 0 && data.BaskiNo != null)
                        {
                            query += "Baski_no = @baskiNo,";
                            query = query.Replace("@baskiNo", data.BaskiNo.ToString());
                        }
                        if (data.BaskiTarihi != null)
                        {
                            query += "Baski_Tarihi = '@baskiTarihi',";
                            query = query.Replace("@baskiTarihi", ((DateTime)data.BaskiTarihi).ToString("MM/dd/yyyy HH:mm:ss"));
                        }
                        if (data.Sayfa != 0 && data.Sayfa != null)
                        {
                            query += "Sayfa = @sayfa,";
                            query = query.Replace("@sayfa", data.Sayfa.ToString());
                        }
                        if (data.AlinmaTarihi != null)
                        {
                            query += "Alinma_Tarihi = '@alinmaTarihi',";
                            query = query.Replace("@alinmaTarihi", ((DateTime)data.AlinmaTarihi).ToString("MM/dd/yyyy HH:mm:ss"));
                        }
                        if (data.AlindigiYer != "" && data.AlindigiYer != null)
                        {
                            query += "Alindigi_Yer = '@alindigiYer',";
                            query = query.Replace("@alindigiYer", data.AlindigiYer.ToString());
                        }

                        query = query.TrimEnd(',');
                        query += " WHERE KitapID = @kitapId;";
                        if (query.Contains("SET  WHERE"))
                        {
                            query = "";
                        }

                        if (data.AltTurIDs.Count > 0)
                        {

                            query += "DELETE FROM KITAP_TUR_ILISKISI WHERE KitapID=@kitapId;";

                            query += "INSERT INTO KITAP_TUR_ILISKISI(KitapID, TurID, AltTurID)";
                            for (int i = 0; i < data.AltTurIDs.Count; i++)
                            {
                                if (i == data.AltTurIDs.Count - 1)
                                {
                                    query += "SELECT " + data.KitapID+ ",@turId,@altTurID;";
                                }
                                else
                                {
                                    query += "SELECT " + data.KitapID+ ",@turId,@altTurID UNION ALL ";
                                }
                                query = query.Replace("@altTurID", data.AltTurIDs[i].ToString());
                            }
                            query = query.Replace("@turId", data.TurID.ToString());
                        }
                        query = query.Replace("@kitapId", data.KitapID.ToString());

                        query = query.TrimEnd(';');

                        SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True");
                        SqlCommand myCommand = new SqlCommand(query, myConn);
                        try
                        {
                            myConn.Open();
                            myCommand.ExecuteNonQuery();
                            MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            if (myConn.State == ConnectionState.Open)
                            {
                                myConn.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "updateKitaplar metodunu çağırırken hata alındı.\nKitap güncelleme işlemi yapılamadı");
                throw;
            }
        }
        public double CalculatePercentage()
        {
            double result = 0;
            try
            {
                List<KITAPLAR> kitaplar = new List<KITAPLAR>();
                List<OKUNAN_KITAPLAR> okunanlar = new List<OKUNAN_KITAPLAR>();

                kitaplar = db.KITAPLAR.ToList();
                okunanlar = db.OKUNAN_KITAPLAR.ToList();
                if (kitaplar.Count > 0 && okunanlar.Count > 0)
                {
                    double oran = (okunanlar.Count / kitaplar.Count)*100;
                    result = Math.Round(oran,0);
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message, "CalculatePercentage metodunu çağırırken hata alındı.");
                throw;
            }
            return result;
        }
        public YAZARLAR CreateWriter(string yazarAdi)
        {
            if (string.IsNullOrWhiteSpace(yazarAdi))
            {
                throw new ArgumentException("Yazar adı boş olamaz.", nameof(yazarAdi));
            }

            // SQL sorgusunu parametre olarak kullanmak, SQL Injection saldırılarına karşı korur
            var query = "INSERT INTO YAZARLAR (Yazar_Adi) VALUES (@YazarAdi); SELECT SCOPE_IDENTITY();";

            using (var myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
            {
                using (var myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.AddWithValue("@YazarAdi", yazarAdi);

                    try
                    {
                        myConn.Open();

                        int yeniYazarID = Convert.ToInt32(myCommand.ExecuteScalar());

                        // MessageBox.Show can be problematic in web applications, consider logging instead
                        // MessageBox.Show("Yazar başarıyla eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return new YAZARLAR { YazarID = yeniYazarID, Yazar_Adi = yazarAdi };
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // For web applications, consider logging the error instead of showing a message box
                        throw new ApplicationException("Yazar ekleme işleminde hata oluştu.", ex);
                    }
                }
            }
        }
        public YAYINLAR CreatePublication(string yayinAdi)
        {
            if (string.IsNullOrWhiteSpace(yayinAdi))
            {
                throw new ArgumentException("Yayin adı boş olamaz.", nameof(yayinAdi));
            }
            var query = "INSERT INTO YAYINLAR (Yayin_Adi) VALUES (@yayinAdi); SELECT SCOPE_IDENTITY();";

            using (var myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
            {
                using (var myCommand = new SqlCommand(query, myConn))
                {
                    myCommand.Parameters.AddWithValue("@yayinAdi", yayinAdi);

                    try
                    {
                        myConn.Open();

                        int newPublicationID = Convert.ToInt32(myCommand.ExecuteScalar());

                        // MessageBox.Show can be problematic in web applications, consider logging instead
                        // MessageBox.Show("Yazar başarıyla eklendi", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return new YAYINLAR { YayinID = newPublicationID, Yayin_Adi = yayinAdi };
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // For web applications, consider logging the error instead of showing a message box
                        throw new ApplicationException("Yayın ekleme işleminde hata oluştu.", ex);
                    }
                }
            }
        }
        public List<YAZARLAR> GetAllWriters()
        {
            var list = new List<YAZARLAR>();

            try
            {
                using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                {
                    string query = @"
                        SELECT
                            YazarID AS YAZARID,
                            Yazar_Adi AS YAZARADI
                        FROM 
                            YAZARLAR";

                    using (SqlCommand target = new SqlCommand(query, myConn))
                    {
                        myConn.Open();
                        using (SqlDataReader dr = target.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                YAZARLAR author = new YAZARLAR();

                                author.YazarID = dr.GetInt32(dr.GetOrdinal("YAZARID"));
                                author.Yazar_Adi = dr.GetString(dr.GetOrdinal("YAZARADI"));
                                
                                list.Add(author);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetAllWriters metodunu çağırırken hata alındı.\nYazarların listelemesi yapılamadı: {e.Message}");
                throw;
            }
            return list;
        }
        public List<YAYINLAR> GetAllPublications()
        {
            var list = new List<YAYINLAR>();

            try
            {
                using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                {
                    string query = @"
                        SELECT
                            YayinID AS YAYINID,
                            Yayin_Adi AS YAYINADI
                        FROM 
                            YAYINLAR";

                    using (SqlCommand target = new SqlCommand(query, myConn))
                    {
                        myConn.Open();
                        using (SqlDataReader dr = target.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                YAYINLAR publication = new YAYINLAR();

                                publication.YayinID = dr.GetInt32(dr.GetOrdinal("YAYINID"));
                                publication.Yayin_Adi = dr.GetString(dr.GetOrdinal("YAYINADI"));

                                list.Add(publication);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetAllPublications metodunu çağırırken hata alındı.\nYayınların listelemesi yapılamadı: {e.Message}");
                throw;
            }
            return list;
        }
        public List<TURLER> GetAllGenres()
        {
            List<TURLER> list = new List<TURLER>();
            try
            {
                string query = @"
            SELECT 
            TurID AS TURID,
            Tur_Adi AS TURADI
            
            FROM TURLER";

                using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                {
                    using (SqlCommand target = new SqlCommand(query, myConn))
                    {
                        target.CommandType = CommandType.Text;

                        myConn.Open();
                        using (SqlDataReader dr = target.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    TURLER categori = new TURLER();

                                    categori.TurID = dr["TURID"] != DBNull.Value ? (int)dr["TURID"] : 0;
                                    categori.Tur_Adi = (string)dr["TURADI"] ?? "";
                                    list.Add(categori);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata: {e.Message}. Türler getirilirken bir hata oluştu.");
                throw;
            }

            return list;
        }

        public List<ALTTURLER> GetAllSubGenres(int id)
        {
            List<ALTTURLER> list = new List<ALTTURLER>();

            try
            {
                string query = @"
            SELECT 
            TurID AS TURID,
            Alt_Tur_Adi AS ALTTURADI,
            Alt_TurID AS ALTTURID
            FROM ALTTURLER 
           
            WHERE TurID=@turID";

                using (SqlConnection myConn = new SqlConnection("Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True"))
                {
                    using (SqlCommand target = new SqlCommand(query, myConn))
                    {
                        target.Parameters.AddWithValue("@turID", id);

                        target.CommandType = CommandType.Text;

                        myConn.Open();
                        using (SqlDataReader dr = target.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    ALTTURLER subGenre = new ALTTURLER();

                                    subGenre.TurID = dr["TURID"] != DBNull.Value ? (int)dr["TURID"] : 0;
                                    subGenre.Alt_Tur_Adi = dr["ALTTURADI"].ToString() ?? "";
                                    subGenre.Alt_TurID = dr["ALTTURID"] != DBNull.Value ? (int)dr["ALTTURID"] : 0;
                                    list.Add(subGenre);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Hata: {e.Message}. Kitap türleri getirilirken bir hata oluştu.");
                throw;
            }

            return list;
        }

        public IEnumerable<KitaplarDTO> FilterBooks(string bookName, int? yazarID, int? yayinID, int? categoryID, int[] subCategoryIDs)
        {
           
            var books = GetBooks().AsQueryable();

            if (!string.IsNullOrEmpty(bookName))
            {
                books = books.Where(b => b.KitapAdi.Contains(bookName));
            }
            if (yazarID.HasValue)
            {
                books = books.Where(b => b.YazarID == yazarID.Value);
            }
            if (yayinID.HasValue)
            {
                books = books.Where(b => b.YayinID == yayinID.Value);
            }
            
            if (categoryID.HasValue)
            {
                books = books.Where(b => b.TurID == categoryID.Value);
            }

            if (subCategoryIDs != null && subCategoryIDs.Length > 0)
            {
                books = books.Where(b => b.AltTurIDs.Any(subCatId => subCategoryIDs.Contains(subCatId)));
            }
            return books.ToList();
        }

        public List<KitaplarDTO> SearchBooks(string searchTerm)
        {
            List<KitaplarDTO> books = new List<KitaplarDTO>();

            // Normalize search term to lower case
            searchTerm = searchTerm?.ToLower();
            string connectionString="Data Source=YUSUF;Initial Catalog=BeyazKitaplik;Integrated Security=True";


            using (SqlConnection myConn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT
                    k.KitapID,
                    k.Kitap_Adi,
                    k.YazarID,
                    y.Yazar_Adi,
                    k.YayinID,
                    ya.Yayin_Adi,
                    k.Baski_no,
                    k.Baski_Tarihi,
                    k.Sayfa,
                    k.Alinma_Tarihi,
                    k.Alindigi_Yer,
                    t.TurID,
                    t.Tur_Adi,
                    alt.Alt_TurID,
                    alt.Alt_Tur_Adi
                FROM
                    KITAPLAR k
                    LEFT JOIN YAZARLAR y ON k.YazarID = y.YazarID
                    LEFT JOIN YAYINLAR ya ON k.YayinID = ya.YayinID
                    LEFT JOIN KITAP_TUR_ILISKISI kt ON k.KitapID = kt.KitapID
                    LEFT JOIN TURLER t ON kt.TurID = t.TurID
                    LEFT JOIN ALTTURLER alt ON kt.AltTurID = alt.Alt_TurID
                WHERE
                    k.KitapID IN (
                        SELECT k.KitapID
                        FROM KITAPLAR k
                        LEFT JOIN YAZARLAR y ON k.YazarID = y.YazarID
                        LEFT JOIN YAYINLAR ya ON k.YayinID = ya.YayinID
                        LEFT JOIN KITAP_TUR_ILISKISI kt ON k.KitapID = kt.KitapID
                        LEFT JOIN TURLER t ON kt.TurID = t.TurID
                        LEFT JOIN ALTTURLER alt ON kt.AltTurID = alt.Alt_TurID
                        WHERE
                            LOWER(k.Kitap_Adi) LIKE '%' + @searchTerm + '%' OR
                            LOWER(y.Yazar_Adi) LIKE '%' + @searchTerm + '%' OR
                            LOWER(ya.Yayin_Adi) LIKE '%' + @searchTerm + '%' OR
                            LOWER(t.Tur_Adi) LIKE '%' + @searchTerm + '%' OR
                            LOWER(alt.Alt_Tur_Adi) LIKE '%' + @searchTerm + '%' OR
                            LOWER(k.Alindigi_Yer) LIKE '%' + @searchTerm + '%'
                    )";

                SqlCommand myCommand = new SqlCommand(query, myConn);
                myCommand.Parameters.AddWithValue("@searchTerm", searchTerm);

                myConn.Open();
                SqlDataReader reader = myCommand.ExecuteReader();

                while (reader.Read())
                {
                    var kitapID = reader.GetInt32(reader.GetOrdinal("KitapID"));
                    var kitapDTO = books.FirstOrDefault(b => b.KitapID == kitapID);
                    if (kitapDTO == null)
                    {
                        kitapDTO = new KitaplarDTO
                        {
                            KitapID = kitapID,
                            KitapAdi = reader.IsDBNull(reader.GetOrdinal("Kitap_Adi")) ? "" : reader.GetString(reader.GetOrdinal("Kitap_Adi")),
                            YazarID = reader.IsDBNull(reader.GetOrdinal("YazarID")) ? 0 : reader.GetInt32(reader.GetOrdinal("YazarID")),
                            YazarAdi = reader.IsDBNull(reader.GetOrdinal("Yazar_Adi")) ? "" : reader.GetString(reader.GetOrdinal("Yazar_Adi")),
                            YayinID = reader.IsDBNull(reader.GetOrdinal("YayinID")) ? 0 : reader.GetInt32(reader.GetOrdinal("YayinID")),
                            YayinAdi = reader.IsDBNull(reader.GetOrdinal("Yayin_Adi")) ? "" : reader.GetString(reader.GetOrdinal("Yayin_Adi")),
                            BaskiNo = reader.IsDBNull(reader.GetOrdinal("Baski_no")) ? 0 : reader.GetInt32(reader.GetOrdinal("Baski_no")),
                            BaskiTarihi = reader.IsDBNull(reader.GetOrdinal("Baski_Tarihi")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Baski_Tarihi")),
                            BaskiTarihiText = reader.IsDBNull(reader.GetOrdinal("Baski_Tarihi")) ? "" : reader.GetDateTime(reader.GetOrdinal("Baski_Tarihi")).ToString("yyyy-MM-dd"),
                            Sayfa = reader.IsDBNull(reader.GetOrdinal("Sayfa")) ? 0 : reader.GetInt32(reader.GetOrdinal("Sayfa")),
                            AlinmaTarihi = reader.IsDBNull(reader.GetOrdinal("Alinma_Tarihi")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Alinma_Tarihi")),
                            AlinmaTarihiText = reader.IsDBNull(reader.GetOrdinal("Alinma_Tarihi")) ? "" : reader.GetDateTime(reader.GetOrdinal("Alinma_Tarihi")).ToString("yyyy-MM-dd"),
                            AlindigiYer = reader.IsDBNull(reader.GetOrdinal("Alindigi_Yer")) ? "" : reader.GetString(reader.GetOrdinal("Alindigi_Yer")),
                            TurID = reader.IsDBNull(reader.GetOrdinal("TurID")) ? 0 : reader.GetInt32(reader.GetOrdinal("TurID")),
                            TurAdi = reader.IsDBNull(reader.GetOrdinal("Tur_Adi")) ? "" : reader.GetString(reader.GetOrdinal("Tur_Adi")),
                            AltTurIDs = new List<int>(),
                            AltTurAdis = new List<string>(),
                            SubTypes = ""
                        };
                        books.Add(kitapDTO);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Alt_TurID")))
                    {
                        kitapDTO.AltTurIDs.Add(reader.GetInt32(reader.GetOrdinal("Alt_TurID")));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("Alt_Tur_Adi")))
                    {
                        kitapDTO.AltTurAdis.Add(reader.GetString(reader.GetOrdinal("Alt_Tur_Adi")));
                    }
                }
                myConn.Close();
            }

            // Update SubTypes property for each book
            foreach (var book in books)
            {
                book.SubTypes = string.Join(", ", book.AltTurAdis);
            }

            return books;
        }
    }
}