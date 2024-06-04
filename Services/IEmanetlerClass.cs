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
    internal interface IEmanetlerClass
    {
    List<EMANETLER> getEmanetler();
    void createEmanet(List<EMANETLER> list);
    void deleteEmanet(EMANETLER data);
    void updateEmanet(EMANETLER data);
        
    }
}
