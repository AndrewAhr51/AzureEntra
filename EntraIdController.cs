using Microsoft.AspNetCore.Mvc;
using EntraIdBL.Models;
using EntraIdBL.Interfaces;
using EntraIdBL.Helper;
using EntraIdBL.Services;

namespace NeudesicConsole.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntraIdController : Controller
    {
        private static ILogger? _logger;
        private readonly IGraphClient _graphClient;
        private readonly IRegionalMembers _regionalMembers;
        private readonly string _region;
        private readonly IConnectionServices _connectionServices;
        
        public EntraIdController(ILogger<EntraIdController> logger, IGraphClient graphClient, IRegionalMembers regionalMembers, IConnectionServices connectionServices)
        {
            _logger = logger;
            _graphClient = graphClient;
            _regionalMembers = regionalMembers;
            _connectionServices = connectionServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetGroupMembers")]
        public async Task<IActionResult> GetGroupMembers([FromBody] EntraIdGroup _entraIdGroup)
        {
            string baseUrl = _connectionServices.GetSharePointBaseUrl();
            string siteName = _connectionServices.GetSharePointSiteName();
            string listName = _connectionServices.GetSharePointListName();
            
            List<EntraIdRecord> _entraIdGroupMembers = new List<EntraIdRecord>();

            try
            {
                _entraIdGroupMembers = await _graphClient.GetGroupMembers(_entraIdGroup.Region, _entraIdGroup.GroupId);

                //Clear the list for the region passed in and insert records into neudesiconsultants list
                var result = await _regionalMembers.ProcessRegionalMembers(baseUrl, siteName, listName, _entraIdGroup.Region, _entraIdGroupMembers);

                if (result is not OkResult)
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
                       
            return Ok(_entraIdGroupMembers);
        }
    }
}
