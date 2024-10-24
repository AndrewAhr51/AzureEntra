using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntraIdBL.Interfaces;

namespace EntraIdBL.Services
{
    public class ConnectionServices: IConnectionServices
    {
        private string _sharePointBaseUrl;
        private string _sharePointSiteName;
        private string _sharePointListName;

        public ConnectionServices(string sharePointBaseUrl, string sharePointSiteName, string sharePointListName )
        {
            _sharePointBaseUrl = sharePointBaseUrl;
            _sharePointSiteName = sharePointSiteName;
            _sharePointListName = sharePointListName;
        }
        
        public string GetSharePointBaseUrl()
        {
           return _sharePointBaseUrl;
        }

        public string GetSharePointSiteName()
        {
            return _sharePointSiteName;
        }

        public string GetSharePointListName()
        {
            return _sharePointListName;
        }
    }
}
