using CodigoTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using ZXing.QrCode;

namespace CodigoTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;


        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
  

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
            select c.coupon_id,c.couponName,m.member_id,m.member_Name,mc.order_id as receipt_no,mc.usedDate from dbo.member m join dbo.member_coupon mc on mc.member_id=m.member_id inner join dbo.coupon c on c.coupon_id=mc.coupon_id  ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("dbconn");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet("GetReportByCouponId/{id}")]
        public async Task<ActionResult<Coupon>> GetCouponById(int id)
        {

            string sqlDataSource = _configuration.GetConnectionString("dbconn");

            SqlConnection con = new SqlConnection(sqlDataSource);
            SqlCommand cmd = new SqlCommand("[ReportByCouponId]", con);
            con.Open();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@couponId", id);
            cmd.ExecuteNonQuery();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new JsonResult(ds);

        }
    }
}
