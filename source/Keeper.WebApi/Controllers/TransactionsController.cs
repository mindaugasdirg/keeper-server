using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Keeper.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Keeper.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private ITransactionsService transactionsService;

        public TransactionsController(ITransactionsService _transactionsService)
        {
            transactionsService = _transactionsService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] string newTransaction)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = GetUserId();
            if(string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Token is invalid");
            var result = await transactionsService.AddAsync(userId, newTransaction);
            if(result <= 0)
                return StatusCode(500, "Error has occured while adding transaction");
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetFrom(int id)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var userId = GetUserId();
            if(string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Token is invalid");
            var transactions = transactionsService.GetTransactionsFrom(userId, id);
            return Ok(transactions);
        }

        private string GetUserId() => User.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault();
    }
}