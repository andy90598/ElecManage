using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElecManage.Models;
using Microsoft.AspNetCore.SignalR;
using ElecManage.Hubs;

namespace ElecManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectricitiesController : ControllerBase
    {
        private readonly ElecDBContext _context;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;


        public ElectricitiesController(ElecDBContext context,IHubContext<BroadcastHub,IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Electricities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Electricity>>> GetElectricities()
        {
            return await _context.Electricities.ToListAsync();
        }

        // GET: api/Electricities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Electricity>> GetElectricity(long id)
        {
            var electricity = await _context.Electricities.FindAsync(id);

            if (electricity == null)
            {
                return NotFound();
            }

            return electricity;
        }

        // PUT: api/Electricities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutElectricity(long id, Electricity electricity)
        {
            if (id != electricity.ElecSn)
            {
                return BadRequest();
            }

            _context.Entry(electricity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ElectricityExists(id))
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

        // POST: api/Electricities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Electricity>> PostElectricity(Electricity electricity)
        {
            
            _context.Electricities.Add(electricity);
            try
            {
                await _hubContext.Clients.All.BroadcastMessage();
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                if (ElectricityExists(electricity.ElecSn))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            

            return CreatedAtAction("GetElectricity", new { id = electricity.ElecSn }, electricity);
        }

        // DELETE: api/Electricities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteElectricity(long id)
        {
            var electricity = await _context.Electricities.FindAsync(id);
            if (electricity == null)
            {
                return NotFound();
            }

            _context.Electricities.Remove(electricity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ElectricityExists(long id)
        {
            return _context.Electricities.Any(e => e.ElecSn == id);
        }
    }
}
