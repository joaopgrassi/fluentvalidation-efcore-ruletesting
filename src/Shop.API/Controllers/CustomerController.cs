using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Data.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ShopDbContext _shopDbContext;

        public CustomerController(ShopDbContext shopDbContext)
        {
            _shopDbContext = shopDbContext ?? throw new ArgumentNullException(nameof(shopDbContext));
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        public async Task<ActionResult<Customer>> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _shopDbContext.Customers.FindAsync(id, cancellationToken);

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer newCustomer, CancellationToken cancellationToken)
        {
            _shopDbContext.Add(newCustomer);
            await _shopDbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtRoute(
                nameof(GetById),
                new { id = newCustomer.Id }, newCustomer);
        }
    }
}
