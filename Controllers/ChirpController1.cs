using Microsoft.AspNetCore.Mvc;
using MockingBird.Models;
using MockingBird.Services;

namespace MockingBird.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChirpController : ControllerBase
    {
        //Dependency injection
        private readonly IChirpService service;

        public ChirpController(IChirpService service)
        {
            this.service = service;
        }


        //Get all chirps using the service GetAll
        [HttpGet]
        public IEnumerable<Chirp> Get()
        {
            return service.GetAll();
        }


        //Post a new chirp using the service Create
        [HttpPost]
        public IActionResult Post([FromBody] Chirp chirp)
        {
            if (chirp != null)
            {
                Chirp newChirp = service.Create(chirp);
                return CreatedAtRoute("GetById", new { Id = newChirp.Id }, newChirp);
            }
            else
            {
                return BadRequest();
            }
        }


        // GET api/<UserController>/5
        [HttpGet("{GetUser}", Name = "Get")]
        public IActionResult Get(string UserName, string Password
            )
        {
            User? user = service.GetUser(UserName, Password);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

    }
}

