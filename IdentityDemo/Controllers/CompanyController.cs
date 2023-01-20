using IdentityDemo.Dal;
using IdentityDemo.Dal.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public CompanyController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<CompanyEntity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() => Ok(await _ctx.Companies.ToListAsync());

        [HttpGet("{id}")]
        [Authorize(Policy = "Authenticated")]
        [ProducesResponseType(typeof(CompanyEntity), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(int id) =>
            Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value) == id 
            || User.FindFirst(ClaimTypes.Role)!.Value == "Admin" ?
                Ok(await _ctx.Companies.SingleAsync(c => c.Id == id)) : Forbid();
    }
}
