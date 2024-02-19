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
                string queryCreate = $"CREATE TABLE `wordset_{set.Name}` (id INT AUTO_INCREMENT PRIMARY KEY, word VARCHAR(255), definition VARCHAR(255))";
                int rowsAffectedCreate = await db.CreateTable(queryCreate);

                if(rowsAffectedCreate == -1)
                {
                    return Conflict("Table creation failed");
                }
                else if(rowsAffectedCreate == -2)
                {
                    return Conflict("Table already exists");
                }
                else if(rowsAffectedCreate == -3)
                {
                    return Conflict("Error opening connection to database");
                }
                else if(rowsAffectedCreate > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete(Name = "DeleteSet")]
        public async Task<IActionResult> Delete(string setName)
        {
            if(setName == null || setName == "")
            {
                return BadRequest("No set provided");
            }
            else
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"DELETE FROM sets where name = '{setName}'";
                int rowsAffected = await db.Delete(query);

                if(rowsAffected == -1)
                {
                    return Conflict("Delete failed");
                }
                else if(rowsAffected == -2)
                {
                    return Conflict("Set does not exist");
                }
                else if(rowsAffected == -3)
                {
                    return Conflict("Error opening connection to database");
                }
                else if(rowsAffected > 0)
                {
                    string queryDrop = $"DROP TABLE `wordset_{setName}`";
                    int rowsAffectedDrop = await db.DropTable(queryDrop);
                    
                    if(rowsAffectedDrop == -1)
                    {
                        return Conflict("Table deletion failed");
                    }
                    else if(rowsAffectedDrop == -2)
                    {
                        return Conflict("Table set not exist, set name deletion successfull");
                    }
                    else if(rowsAffectedDrop == -3)
                    {
                        return Conflict("Error opening connection to database");
                    }
                    else if(rowsAffectedDrop == 0)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                    
                }
                else
                {
                    return BadRequest();
                }
            }
        }
        

    }
}
