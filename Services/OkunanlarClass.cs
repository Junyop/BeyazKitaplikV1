using BeyazKitaplikV1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace BeyazKitaplikV1.Services
{
    public class OkunanlarClass : IOkunanlarClass
    {
        BeyazKitaplikEntities db = new BeyazKitaplikEntities();

        public List<OKUNAN_KITAPLAR> getOkunanlar()
        {

            var list = new List<OKUNAN_KITAPLAR>();

            try
            {
                string query = @"
                SELECT * 
                FROM OKUNAN_KITAPLAR";

                IEnumerable<OKUNAN_KITAPLAR> data = db.Database.SqlQuery<OKUNAN_KITAPLAR>(query);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        OKUNAN_KITAPLAR okunan = new OKUNAN_KITAPLAR();
                        okunan = item;
                        list.Add(okunan);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "getOkunanlar metodunu çağırırken hata alındı.\nOkunan kitapların listelemesi yapılamadı");
                throw;
            }

            return list;
        }

        public void createOkunan(List<OKUNAN_KITAPLAR> list)
        {
            try
            {
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        db.OKUNAN_KITAPLAR.Add(item);
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "createOkunan metodunu çağırırken hata alındı.\nYeni okunan kitap ekleme işlemi yapılamadı");
                throw;
            }
        }

        public void deleteOkunan(OKUNAN_KITAPLAR data)
        {
            try
            {
                if (data != null)
                {
                    OKUNAN_KITAPLAR kitap = db.OKUNAN_KITAPLAR.Find(data.OkumaID);
                    db.OKUNAN_KITAPLAR.Remove(kitap);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "deleteOkunan metodunu çağırırken hata alındı.\nOkunan kitabı kaldırma işlemi yapılamadı");
                throw;
            }
        }

        public void updateOkunan(OKUNAN_KITAPLAR data)
        {
            try
            {
                if (data != null)
                {
                    var query = @"
                    UPDATE OKUNAN_KITAPLAR SET ";
                
                    if (data.KitapID != null)
                    {
                        query += "KitapID=@kitapID,";
                        query = query.Replace("@kitapID", data.KitapID.ToString());
                    }
                    if (data.Degerlendirme_Puani != 0 && data.Degerlendirme_Puani != null)
                    {
                        query += "Degerlendirme_Puani = @degerlendirmePuani,";
                        query = query.Replace("@degerlendirmePuani", data.Degerlendirme_Puani.ToString());

                    }
                    if (data.Yorum != "" && data.Yorum != null)
                    {
                        query += "Yorum = '@yorum',";
                        query = query.Replace("@yorum", data.Yorum.ToString());

                    }
                    if (data.Baslama_Tarihi != null)
                    {
                        query += "Baslama_Tarihi = '@baslamaTarihi',";
                        query = query.Replace("@baslamaTarihi", ((DateTime)data.Baslama_Tarihi).ToString("MM/dd/yyyy HH:mm:ss"));

                    }
                    if (data.Bitirme_Tarihi != null)
                    {
                        query += "Bitirme_Tarihi = '@bitirmeTarihi',";
                        query = query.Replace("@bitirmeTarihi", ((DateTime)data.Bitirme_Tarihi).ToString("MM/dd/yyyy HH:mm:ss"));

                    }
                    query = query.TrimEnd(',');

                    query += " WHERE OkumaID = @okumaId";
                    query = query.Replace("@okumaId", data.OkumaID.ToString());

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
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "updateKitaplar metodunu çağırırken hata alındı.\nKitap güncelleme işlemi yapılamadı");
                throw;
            }
        }

        public List<OKUNAN_KITAPLAR> GetLastFive()
        {
            var list = new List<OKUNAN_KITAPLAR>();

            try
            {
                string query = @"SELECT*
                    FROM OKUNAN_KITAPLAR
                    ORDER BY Bitirme_Tarihi DESC
                    OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY";

                IEnumerable<OKUNAN_KITAPLAR> data = db.Database.SqlQuery<OKUNAN_KITAPLAR>(query);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        OKUNAN_KITAPLAR okunan = new OKUNAN_KITAPLAR();
                        okunan = item;
                        list.Add(okunan);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "getLastFive metodunu çağırırken hata alındı.\nOkunan kitapların listelemesi yapılamadı");
                throw;
            }

            return list;
        }
        public List<OKUNAN_KITAPLAR> GetUnfinishedFive()
        {
            var list = new List<OKUNAN_KITAPLAR>();

            try
            {
                string query = @"SELECT*
                    FROM OKUNAN_KITAPLAR
                    WHERE Bitirme_Tarihi IS NULL
                    ORDER BY Baslama_Tarihi ASC
                    OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY";

                IEnumerable<OKUNAN_KITAPLAR> data = db.Database.SqlQuery<OKUNAN_KITAPLAR>(query);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        OKUNAN_KITAPLAR okunan = new OKUNAN_KITAPLAR();
                        okunan = item;
                        list.Add(okunan);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "GetUnfinishedFive metodunu çağırırken hata alındı.\nOkunan kitapların listelemesi yapılamadı");
                throw;
            }

            return list;
        }
    }
}