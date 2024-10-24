using EntraIdBL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntraIdBL.Interfaces
{
    public interface IRegionalMembers
    {
        public Task<IActionResult> ProcessRegionalMembers(string baseUrl, string siteName, string listName, string region, List<EntraIdRecord> entraIdRecords );
    }
}
