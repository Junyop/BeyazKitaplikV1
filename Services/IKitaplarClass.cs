using BeyazKitaplikV1.DTO;
using BeyazKitaplikV1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyazKitaplikV1.Services
{
    internal interface IKitaplarClass
    {
        void CreateBook(KitaplarDTO book);
        void UpdateBook(KitaplarDTO data);
        void DeleteBook(int id);
        List<KitaplarDTO> GetBooks( List<int> ids=null);
        double CalculatePercentage();
        KitaplarDTO GetTypes(int ID);
        List<YAYINLAR> GetAllPublications();
        List<YAZARLAR> GetAllWriters();
        List<TURLER> GetAllGenres();
        List<ALTTURLER> GetAllSubGenres(int id);
        YAYINLAR CreatePublication(string yayinAdi);
        List<KitaplarDTO> SearchBooks(string searchTerm);

    }
}
