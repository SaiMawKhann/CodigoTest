using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodigoTest.Models;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using CodigoTest.Cache;
using System.Drawing;
using System.Drawing.Imaging;

namespace CodigoTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly CodigoTestDatabaseContext _context;
        private readonly JWTSettings _jwtsettings;
        private readonly ICacheService _cacheService;



        public MembersController(CodigoTestDatabaseContext context, IOptions<JWTSettings> jwtsettings, ICacheService cacheService)
        {
            _context = context;
            _jwtsettings = jwtsettings.Value;
            _cacheService = cacheService;

        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Member>>("Member");

            if (_context.Members == null)
             {
                return NotFound();
             }


            var expirationTime = DateTimeOffset.Now.AddHours(1.0);
            cacheData = _context.Members.ToList();
            _cacheService.SetData<IEnumerable<Member>>("Member", cacheData, expirationTime);

            return await _context.Members.ToListAsync();
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            var cacheData = _cacheService.GetData<IEnumerable<Member>>("Member");

            if (_context.Members == null)
          {
              return NotFound();
          }
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            var expirationTime = DateTimeOffset.Now.AddHours(1.0);
            cacheData = _context.Members.ToList();
            _cacheService.SetData<IEnumerable<Member>>("Member", cacheData, expirationTime);

            return member;
        }

        [HttpGet("GetMemberDetails/{id}")]
        public async Task<ActionResult<Member>> GetMemberDetails(int id)
        {
            var user = _context.Members
                                            .Include(pub => pub.Orders)
                                            .Where(pub => pub.MemberId == id)
                                            .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        [HttpPost("Login")]
        public async Task<ActionResult<MemberWithToken>> Login([FromBody] Member member)
        {
            member = await _context.Members
                                        .Where(u => u.Email == member.Email
                                                && u.Password == member.Password).FirstOrDefaultAsync();

            MemberWithToken memberWithToken = null;

            if (member != null)
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                member.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                memberWithToken = new MemberWithToken(member);
                memberWithToken.RefreshToken = refreshToken.Token;
            }

            if (memberWithToken == null)
            {
                return NotFound();
            }

            //sign your token here here..
            memberWithToken.AccessToken = GenerateAccessToken(member.MemberId);
            return memberWithToken;
        }
        
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<MemberWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            Member member = GetUserFromAccessToken(refreshRequest.AccessToken);

            if (member != null && ValidateRefreshToken(member, refreshRequest.RefreshToken))
            {
                MemberWithToken memberWithToken = new MemberWithToken(member);
                memberWithToken.AccessToken = GenerateAccessToken(member.MemberId);

                return memberWithToken;
            }

            return null;
        }

        private bool ValidateRefreshToken(Member member, string refreshToken)
        {
            RefreshToken refreshTokenMember = _context.RefreshTokens.Where(rt => rt.Token == refreshToken)
                                                        .OrderByDescending(rt => rt.ExpiryDate)
                                                        .FirstOrDefault();

            if (refreshTokenMember != null && refreshTokenMember.MemberId == member.MemberId
                    && refreshTokenMember.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        private Member GetUserFromAccessToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var memberId = principle.FindFirst(ClaimTypes.Name)?.Value;

                return _context.Members.Where(usr => usr.MemberId == Convert.ToInt32(User)).FirstOrDefault();
            }

            return null;
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            refreshToken.ExpiryDate = DateTime.UtcNow.AddMonths(6);

            return refreshToken;
        }
        private string GenerateAccessToken(int memberId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Convert.ToString(memberId))
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.MemberId)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
          if (_context.Members == null)
          {
              return Problem("Entity set 'CodigoTestDatabaseContext.Members'  is null.");
          }
            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMember", new { id = member.MemberId }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemberExists(int id)
        {
            return (_context.Members?.Any(e => e.MemberId == id)).GetValueOrDefault();
        }

       
    }
}
