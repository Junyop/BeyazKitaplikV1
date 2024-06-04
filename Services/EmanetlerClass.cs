using BeyazKitaplikV1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace BeyazKitaplikV1.Services
{
    public class EmanetlerClass : IEmanetlerClass
    {
        BeyazKitaplikEntities db = new BeyazKitaplikEntities();
        public List<EMANETLER> getEmanetler()
        {
            var list = new List<EMANETLER>();

            try
            {
                string query = @"
                SELECT * 
                FROM EMANETLER";

                IEnumerable<EMANETLER> data = db.Database.SqlQuery<EMANETLER>(query);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        EMANETLER emanet = new EMANETLER();
                        emanet = item;
                        list.Add(emanet);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "getEmanetler metodunu çağırırken hata alındı.\nVerilen kitapların listelemesi yapılamadı");
                throw;
            }

            return list;
        }

        public void createEmanet(List<EMANETLER> list)
        {
            try
            {
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        db.EMANETLER.Add(item);
                        db.SaveChanges();

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "createEmanet metodunu çağırırken hata alındı.\nYeni verilen kitap ekleme işlemi yapılamadı");
                throw;
            }
        }

        public void deleteEmanet(EMANETLER data)
        {
            try
            {
                if (data != null)
                {
                    EMANETLER kitap = db.EMANETLER.Find(data.EmanetID);
                    db.EMANETLER.Remove(kitap);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, "deleteEmanet metodunu çağırırken hata alındı.\nVerilen kitabı kaldırma işlemi yapılamadı");
                throw;
            }
        }

        public void updateEmanet(EMANETLER data)
        {
            try
            {
                if (data != null)
                {
                    var query = @"
                    UPDATE EMANETLER SET ";

                    if (data.KitapID != null)
                    {
                        query += "KitapID=@kitapID,";
                        query = query.Replace("@kitapID", data.KitapID.ToString());
                    }
                    if (data.Kime_Verildi !="" && data.Kime_Verildi != null)
                    {
                        query += "Kime_Verildi = '@kimeVerildi',";
                        query = query.Replace("@kimeVerildi", data.Kime_Verildi.ToString());

                    }
                   
                    if (data.Verilme_Tarihi != null)
                    {
                        query += "Verilme_Tarihi = '@verilmeTarihi',";
                        query = query.Replace("@verilmeTarihi", ((DateTime)data.Verilme_Tarihi).ToString("MM/dd/yyyy HH:mm:ss"));

                    }
                    if (data.Geri_Alinma_Tarihi != null)
                    {
                        query += "Geri_Alinma_Tarihi = '@geriAlinmaTarihi',";
                        query = query.Replace("@geriAlinmaTarihi", ((DateTime)data.Geri_Alinma_Tarihi).ToString("MM/dd/yyyy HH:mm:ss"));

                    }
                    query = query.TrimEnd(',');

                    query += " WHERE EmanetID = @emanetID";
                    query = query.Replace("@emanetID", data.EmanetID.ToString());

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
                Console.WriteLine(e.Message, "updateEmanetler metodunu çağırırken hata alındı.\nVerilen kitap ile ilgli güncelleme işlemi yapılamadı");
                throw;
            }
        }
    }
}