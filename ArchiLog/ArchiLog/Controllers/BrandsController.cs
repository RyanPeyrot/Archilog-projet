using System.Diagnostics;
using ArchiLibrary.controllers;
using ArchiLog.Data;
using ArchiLog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiLog.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]/v{version:apiVersion}/")]
    public class BrandsController : BaseController<ArchiLogDbContext, Brand>
    {
        public BrandsController(ArchiLogDbContext context) : base(context)
        {
        }

        [HttpGet]
        [Route("Search/")]
        public async Task<IEnumerable<Brand>> SearchName([FromQuery] string name)
        {
            if (name.Trim().StartsWith("*") && name.Trim().EndsWith("*"))
            {
                name = name.Substring(1);
                name = name.Substring(0, name.Length - 1);
                return await _context.Set<Brand>().Where(x => x.Active && x.Name!.ToLower().Contains(name.ToLower())).ToListAsync();
            } else if (name.Trim().StartsWith("*"))
            {
                name = name.Substring(1);
                return await _context.Set<Brand>().Where(x => x.Active && x.Name!.ToLower().EndsWith(name.ToLower())).ToListAsync();
            } else if (name.Trim().EndsWith("*"))
            {
                name = name.Substring(0, name.Length - 1);
                return await _context.Set<Brand>().Where(x => x.Active && x.Name!.ToLower().StartsWith(name.ToLower())).ToListAsync();
            }else
            {
                return await _context.Set<Brand>().Where(x => x.Active && x.Name!.ToLower().Equals(name.ToLower())).ToListAsync();
            }
        }
    }
}
