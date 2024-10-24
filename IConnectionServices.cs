using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntraIdBL.Interfaces
{
    public interface IConnectionServices
    {
        public string GetSharePointBaseUrl();
        public string GetSharePointSiteName();
        public string GetSharePointListName();
    }
}
