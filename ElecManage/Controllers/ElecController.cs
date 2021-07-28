using ElecManage.Models;
using ElecManage.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElecManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElecController : ControllerBase
    {
        private readonly IHttpClient _httpClient;
        private readonly ElecDBContext _elecDBContext;
        public ElecController(ElecDBContext elecDBContext, IHttpClient httpClient)
        {
            _elecDBContext= elecDBContext;
            _httpClient = httpClient;
        }
        // GET: api/<ElecController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_elecDBContext.Electricities);
        }

        // GET api/<ElecController>/5
        [HttpGet("{id}")]
        public Task<List<ElecData>> Get(int id)
        {
            var aa = _httpClient.OnGet();
            
            return aa;
        }

        // POST api/<ElecController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<ElecController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ElecController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }



        [HttpGet("kWh")]
        public IActionResult kWh()
        {
            var db = _elecDBContext.Electricities;
            IActionResult msg =NotFound("0");
            
            return Ok(msg);
        }
    }
}
