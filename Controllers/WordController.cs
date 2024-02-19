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
                    return BadRequest("Unknown error");
                }
            }
        }

        [HttpDelete(Name = "DeleteWord")]
        public async Task<IActionResult> Delete(Word word)
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
                return BadRequest("No word name provided");
            }
            else if(word.Definition == null || word.Definition == "")
            {
                return BadRequest("No definition provided");
            }
            else
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"DELETE FROM `wordset_{word.TableName}` WHERE word = `{word.WordName}` AND definition = `{word.Definition}`";
                int rowsAffected = await db.Insert(query);

                if(rowsAffected == -1)
                {
                    return Conflict("Delete failed");
                }
                else if(rowsAffected == -3)
                {
                    return Conflict("Failed to connect to database");
                }
                else if(rowsAffected > 0)
                {
                    return Ok("Word deleted");
                }
                else
                {
                    return BadRequest("Unknown error");
                }
            }
        }

        [HttpPut(Name = "UpdateWord")]
        public async Task<IActionResult> Update(Word word)
        {
            if (word == null)
            {
                return BadRequest("No word provided");
            }
            else if (word.TableName == null || word.TableName == "")
            {
                return BadRequest("No table name provided");
            }
            else if (word.WordName == null || word.WordName == "")
            {
                return BadRequest("No word name provided");
            }
            else if (word.Definition == null || word.Definition == "")
            {
                return BadRequest("No definition provided");
            }
            else
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"UPDATE `wordset_{word.TableName}` SET definition = `{word.Definition}` WHERE word = `{word.WordName}`";
                int rowsAffected = await db.Insert(query);

                if (rowsAffected == -1)
                {
                    return Conflict("Update failed");
                }
                else if(rowsAffected == -2)
                {
                    return Conflict("Duplicate entry");
                }
                else if (rowsAffected == -3)
                {
                    return Conflict("Failed to connect to database");
                }
                else if (rowsAffected > 0)
                {
                    return Ok("Word updated");
                }
                else
                {
                    return BadRequest("Unknown error");
                }
            }
        }
    }
}
