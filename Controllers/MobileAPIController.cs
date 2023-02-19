using CodigoTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodigoTest.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MobileAPIController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CodigoTestDatabaseContext _context;

        public MobileAPIController(IConfiguration configuration, CodigoTestDatabaseContext context)
        {
            _configuration = configuration;
            _context = context;

        }

        [HttpGet("GetHistoryByMemberId/{id}")]
        public async Task<ActionResult<Coupon>> GetHistoryByMemberId(int id)
        {

            string sqlDataSource = _configuration.GetConnectionString("dbconn");

            SqlConnection con = new SqlConnection(sqlDataSource);
            SqlCommand cmd = new SqlCommand("[GetHistoryByMemberId]", con);
            con.Open();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MemberId", id);
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new JsonResult(ds);

        }
    }
}
