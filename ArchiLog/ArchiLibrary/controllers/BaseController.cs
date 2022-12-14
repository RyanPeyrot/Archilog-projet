using ArchiLibrary.Data;
using ArchiLibrary.Models;
using ArchiLibrary.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.TraceSource;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace ArchiLibrary.controllers
{
    [ApiController]
    [Route("api/[controller]/v{version:apiVersion}/")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public abstract class BaseController<TContext, TModel> : ControllerBase where TContext : BaseDbContext where TModel : BaseModel
    {
        protected readonly TContext _context;

        public BaseController(TContext context)
        {
            _context = context;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        public async Task<IEnumerable<TModel>> GetAll([FromQuery] Params param)
        {
            var newArrayParam = new Dictionary<string, string>();

            foreach (var filter in Request.Query)
            {
                if (param.GetType().GetProperty(filter.Key) == null && typeof(TModel).GetProperty(filter.Key) != null)
                {
                    newArrayParam[filter.Key] = filter.Value;
                }
            }

            int totalItems = _context.Set<TModel>().Count();
            int acceptRange = totalItems < 50 ? totalItems : 50;
            string[] indexes = (param.Range ?? "1-25").Split('-');
            var startIndex = int.Parse(indexes[0]);
            var endIndex = int.Parse(indexes[1]);
            var difference = endIndex - startIndex;
            if (difference > acceptRange) endIndex = startIndex + acceptRange;


            var results = await _context.Set<TModel>().Filter(param, newArrayParam).Where(x => x.Active).Sort(param).Pagination(startIndex, endIndex).ToListAsync();
            
            Response.Headers.Add("Content-Range", param.Range);
            Response.Headers.Add("Accept-Range", results[0].GetType().Name + " " + acceptRange);
            Response.Headers.Add("Link",
                Request.Host + Request.Path + "?range=1-" + (difference + 1) + "; rel=\"first\", " +
                Request.Host + Request.Path + "?range=" + (startIndex - difference - 1 < 1 ? 1 : startIndex - difference - 1) + "-" +
                    (endIndex - difference - 1 < 1 ? endIndex : endIndex - difference - 1) + "; rel=\"prev\", " +
                Request.Host + Request.Path + "?range=" + (startIndex + difference + 1) + "-" +
                    (endIndex + difference + 1 > totalItems ? totalItems : endIndex + difference + 1) + "; rel=\"next\", " +
                Request.Host + Request.Path + "?range=" + (totalItems - difference) + "-" + totalItems + "; rel=\"last\"");

            return results;
        }

        [HttpGet("{id}")]// /api/{item}/3
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<TModel>> GetById([FromRoute] int id)
        {
            var item = await _context.Set<TModel>().SingleOrDefaultAsync(x => x.ID == id);
            if (item == null || !item.Active)
                return NotFound();
            return item;
        }

        [HttpPost]
        public async Task<IActionResult> PostItem([FromBody] TModel item)
        {
            item.Active = true;
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetById", new { id = item.ID }, item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TModel>> PutItem([FromRoute] int id, [FromBody] TModel item)
        {
            if (id != item.ID)
                return BadRequest();
            if (!ItemExists(id))
                return NotFound();

            //_context.Entry(item).State = EntityState.Modified;
            _context.Update(item);
            await _context.SaveChangesAsync();

            return item;
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<TModel>> DeleteItem([FromRoute] int id)
        {
            var item = await _context.Set<TModel>().FindAsync(id);
            if (item == null)
                return BadRequest();
            //_context.Entry(item).State = EntityState.Deleted;
            _context.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }

       [HttpGet]
       [Route("fields=id")]
       [MapToApiVersion("2.0")]
        public async Task<IEnumerable<int>> GetAllId()
        {
            var results = await _context.Set<TModel>().Where(x => x.Active).Select(x=>x.ID).ToListAsync();
            return results;
        }

        private bool ItemExists(int id)
        {
            return _context.Set<TModel>().Any(x => x.ID == id);
        }
    }
}
