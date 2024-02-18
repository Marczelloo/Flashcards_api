using Microsoft.AspNetCore.Mvc;

namespace Flashcards_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetsController : Controller
    {
        private readonly ILogger<SetsController> _logger;

        public SetsController(ILogger<SetsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetSets")]
        public async Task<List<Set>> Get()
        {
            DatabaseConnection db = new DatabaseConnection();
            string query = "SELECT * FROM sets";
            List<string> data = await db.Select(query);
            
            if(data.Count == 0)
            {
                Console.WriteLine("No data found");
                return new List<Set>();
            }
            else
            {
                List<Set> sets = new List<Set>();
                foreach(string row in data)
                {
                    string[] fields = row.Split(",");
                    Set set = new Set();
                    if (int.TryParse(fields[0], out int setId))
                    {
                        set.Id = setId;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to parse '{fields[0]}' as an integer.");
                    }
                    set.Name = fields[1];
                    sets.Add(set);
                }
                return sets;
            }
        }


        [HttpPost(Name = "CreateSet")]
        public async Task<IActionResult> Create(Set set)
        {
            DatabaseConnection db = new DatabaseConnection();
            string query = $"INSERT INTO sets (name) VALUES ('{set.Name}')";
            int rowsAffected = await db.Insert(query);

            if(rowsAffected == -1)
            {
                return Conflict("Duplicate entry");
            }
            else if(rowsAffected == -2)
            {
                return Conflict("Insert failed");
            }
            else if(rowsAffected == -3)
            {
                return Conflict("Error opening connection to database");
            }
            else if(rowsAffected > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
