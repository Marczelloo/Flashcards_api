using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class WordSetController : Controller
    {
        private readonly ILogger<SetsController> _logger;

        public WordSetController(ILogger<SetsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWordSets")]
        public async Task<List<WordSet>> Get(string wordSetName)
        {
            DatabaseConnection db = new DatabaseConnection();
            string query = $"SELECT * FROM `wordset_{wordSetName}`";
            List<string> data = await db.Select(query);

            if (data.Count == 0)
            {
                Console.WriteLine("No data found");
                return new List<WordSet>();
            }
            else
            {
                List<WordSet> wordSet = new List<WordSet>();
                foreach(string row in data)
                {
                    string[] fields = row.Split(",");
                    WordSet word = new WordSet();
                    if (int.TryParse(fields[0], out int wordId))
                    {
                        word.Id = wordId;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to parse '{fields[0]}' as an integer.");
                    }
                    word.Word = fields[1];
                    word.Definition = fields[2];
                    wordSet.Add(word);
                }
                return wordSet;
            }
        }

        [HttpPost(Name = "CreateWordSet")]
        public async Task<IActionResult> Create(WordList wordList)
        {
            if (wordList.Words.Count == 0)
            {
                return BadRequest("No words in list");
            }
            else if (wordList.Name == null)
            {
                return BadRequest("No name for list");
            }
            else if (wordList.Name.Length == 0)
            {
                return BadRequest("No name for list");
            }
            else if (wordList == null)
            {
                return BadRequest("No list");
            }

            DatabaseConnection db = new DatabaseConnection();
            string query = $"CREATE TABLE `wordset_{wordList.Name}` (id INT AUTO_INCREMENT PRIMARY KEY, word VARCHAR(255), definition VARCHAR(255))";
            int rowsAffected = await db.CreateTable(query);

            if (rowsAffected == -2)
            {
                return Conflict("Table already exists");
            }
            else if (rowsAffected == -1)
            {
                return Conflict("Table creation failed");
            }
            else if (rowsAffected == -3)
            {
                return Conflict("Failed to open connection");
            }
            else
            {
                string queryIns = $"INSERT INTO `wordset_{wordList.Name}` (word, definition) VALUES ";
                foreach (WordSet word in wordList.Words)
                {
                    queryIns += $"('{word.Word}', '{word.Definition}'),";
                }
                queryIns = queryIns.Remove(queryIns.Length - 1);
                int rowsAffectedIns = await db.Insert(queryIns);

                if (rowsAffectedIns == -2)
                {
                    return Conflict("Duplicate entry");
                }
                else if (rowsAffectedIns == -1)
                {
                    return Conflict("Insert failed");
                }
                else if (rowsAffected == -3)
                {
                    return Conflict("Error opening connection to database");
                }
                else if (rowsAffectedIns > 0)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }

        [HttpDelete(Name = "DeleteWordSet")]
        public async Task<IActionResult> Delete(string wordSetName)
        {
            if(wordSetName == null || wordSetName.Length == 0)
            {
                return BadRequest("No name for list");
            }
            else
            {
                DatabaseConnection db = new DatabaseConnection();
                string query = $"DROP TABLE `wordset_{wordSetName}`";
                int rowsAffected = await db.DropTable(query);

                if(rowsAffected == -2)
                {
                    return Conflict("Table does not exist");
                }
                else if(rowsAffected == -1)
                {
                    return Conflict("Table deletion failed");
                }
                else if(rowsAffected == -3)
                {
                    return Conflict("Error opening connection to database");
                }
                else if(rowsAffected == 0)
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
}
