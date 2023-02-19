using CodigoTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using ZXing.QrCode;

namespace CodigoTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CodigoTestDatabaseContext _context;

        public QRController(IConfiguration configuration, CodigoTestDatabaseContext context)
        {
            _configuration = configuration;
            _context = context;


        }
    }
}
