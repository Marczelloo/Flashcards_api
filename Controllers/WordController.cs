using Microsoft.AspNetCore.Mvc;

namespace Flashcards_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WordController : Controller
    {
        private readonly ILogger<SetsController> _logger;

        public WordController(ILogger<SetsController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "CreateWord")]
        public async Task<IActionResult> Create(Word word)
        {
            if(word == null)
            {
                return BadRequest("No word provided");
            }
            else if(word.TableName == null || word.TableName == "")
            {
                return BadRequest("No table name provided");
            }
            else if(word.WordName == null || word.WordName == "")
            {
                return BadRequest("Not word name provided");
            }
            else if(word.Definition == null || word.Definition == "")
            {
                return BadRequest("No definition provided");
            }
            else
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"INSERT INTO `wordset_{word.TableName}`(word, definition) VALUES(`{word.WordName}`, `{word.Definition}`)";
                int rowsAffected = await db.Insert(query);

                if(rowsAffected == -2)
                {
                    return Conflict("Duplicate entry");
                }
                else if(rowsAffected == -1)
                {
                    return Conflict("Insert failed");
                }
                else if(rowsAffected == -3)
                {
                    return Conflict("Failed to connect to database");
                }
                else if(rowsAffected > 0)
                {
                    return Ok("Word added");
                }
                else
                {
                    BadRequest("Unknown error");
                }
            }
        }
    }
}
