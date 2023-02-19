using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodigoTest.Models;
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using CodigoTest.Cache;
using Hangfire;

namespace CodigoTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly CodigoTestDatabaseContext _context;
        private readonly ICacheService _cacheService;


        public CouponsController(CodigoTestDatabaseContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;

        }

        // GET: api/Coupons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Coupon>>("Coupon");

            if (_context.Coupons == null)
            {
                return NotFound();
            }

            BackgroundJob.Schedule(() => Console.WriteLine("Schedule Testing"),TimeSpan.FromMinutes(1));

            RecurringJob.AddOrUpdate(() => Console.WriteLine("Rucurring Testing"), "* * * * *", TimeZoneInfo.Local);

            var expirationTime = DateTimeOffset.Now.AddHours(1.0);
            cacheData = _context.Coupons.ToList();
            _cacheService.SetData<IEnumerable<Coupon>>("Coupon", cacheData, expirationTime);
            return await _context.Coupons.ToListAsync();
        }

        [HttpGet("GetCouponDetails/{id}")]
        public async Task<ActionResult<Coupon>> GetCouponDetails(int id)
        {

            var coupon = _context.Coupons
                                            .Include(pub => pub.MemberCoupons)
                                            .Where(pub => pub.CouponId == id)
                                            .FirstOrDefault();

            if (coupon == null)
            {
                return NotFound();
            }

            return coupon;
        }

        // GET: api/Coupons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(int id)
        {
            var cacheData = _cacheService.GetData<IEnumerable<Coupon>>("Coupon");

            if (_context.Coupons == null)
          {
              return NotFound();
          }
            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon == null)
            {
                return NotFound();
            }

            var expirationTime = DateTimeOffset.Now.AddHours(1.0);
            cacheData = _context.Coupons.ToList();
            _cacheService.SetData<IEnumerable<Coupon>>("Coupon", cacheData, expirationTime);
            return coupon;
        }

        // PUT: api/Coupons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoupon(int id, Coupon coupon)
        {
            if (id != coupon.CouponId)
            {
                return BadRequest();
            }

            _context.Entry(coupon).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Coupons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Coupon>> PostCoupon(Coupon coupon)
        {
          if (_context.Coupons == null)
          {
              return Problem("Entity set 'CodigoTestDatabaseContext.Coupons'  is null.");
          }
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCoupon", new { id = coupon.CouponId }, coupon);
        }

        // DELETE: api/Coupons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            if (_context.Coupons == null)
            {
                return NotFound();
            }
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CouponExists(int id)
        {
            return (_context.Coupons?.Any(e => e.CouponId == id)).GetValueOrDefault();
        }


    }
}
