using BeyazKitaplikV1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeyazKitaplikV1.Services
{
    internal interface IOkunanlarClass
    {
        List<OKUNAN_KITAPLAR> getOkunanlar();
        void createOkunan(List<OKUNAN_KITAPLAR> list);

        void deleteOkunan(OKUNAN_KITAPLAR data);

        void updateOkunan(OKUNAN_KITAPLAR data);
        List<OKUNAN_KITAPLAR> GetUnfinishedFive();
        List<OKUNAN_KITAPLAR> GetLastFive();


    }
}