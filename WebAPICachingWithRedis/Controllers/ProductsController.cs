using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPICachingWithRedis.Application.Interfaces;
using WebAPICachingWithRedis.Entities;
using WebAPICachingWithRedis.Persistance.Context;

namespace WebAPICachingWithRedis.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICacheService _cacheService;

        public ProductsController(AppDbContext appDbContext, ICacheService cacheService)
        {
            _appDbContext = appDbContext;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> GetAsync()
        {

            var cacheData = _cacheService.GetCache<IEnumerable<Product>>("products");
            if (cacheData != null && cacheData.Count() > 0)
                return Ok(cacheData);
            else
            {
                cacheData = await _appDbContext.Products.ToListAsync();
                var expityTime = DateTime.Now.AddSeconds(30);
                _cacheService.SetCache<IEnumerable<Product>>("products", cacheData, expityTime);
                return Ok(cacheData);
            }

        }
    }
}
