using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext supplierDb;

        public SupplierController(ApplicationDbContext SupplierDb)
        {
            supplierDb = SupplierDb;
        }
        public IActionResult Index()
        {
            if (supplierDb == null)
            {
                return StatusCode(500, "Database context is not initialized. Check database connection and SQL Server availability.");
            }
            
            var SupplierData = supplierDb.Suppliers.ToList();
            return View(SupplierData);
        }
    }
}
