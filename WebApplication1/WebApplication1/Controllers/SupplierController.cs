using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;



namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin,Procurement Officer,Pharmacy Manager,Compliance Officer")]
    public class SupplierController : Controller
    {
        private readonly ApplicationDbContext supplierDb;

        public SupplierController(ApplicationDbContext SupplierDb)
        {
            supplierDb = SupplierDb;
        }

        //All list of suppliers
        public async Task<IActionResult> Index()
        {
            if (supplierDb == null)
            {
                return StatusCode(500, "Database context is not initialized. Check database connection and SQL Server availability.");
            }

            var SupplierData = await supplierDb.Suppliers.ToListAsync();
            return View(SupplierData);
        }

        //Create new supplier

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier sp)
        {
            if (ModelState.IsValid)
            {
                await supplierDb.Suppliers.AddAsync(sp);
                await supplierDb.SaveChangesAsync();
                TempData["SuccessMessage"] = "Supplier created.";
                return RedirectToAction("Index", "Supplier");
            }
            return View(sp);
        }

        //Details of supplier
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || supplierDb.Suppliers == null)
            {
                return NotFound();
            }

            var SupplierData = await supplierDb.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);

            if (SupplierData == null)
            {
                return NotFound();
            }
            return View(SupplierData);

        }

        //Edit supplier
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || supplierDb.Suppliers == null)
            {
                return NotFound();
            }
            var SupplierData = await supplierDb.Suppliers.FindAsync(id);

            if (SupplierData == null)
            {
                return NotFound();
            }
            return View(SupplierData);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Supplier sp)
        {
            if (id != sp.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                supplierDb.Suppliers.Update(sp);
                await supplierDb.SaveChangesAsync();
                TempData["SuccessUpdate"] = "Supplier Updated.";
                return RedirectToAction("Index", "Supplier");
            }

            return View(sp);
        }

        //Delete supplier

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || supplierDb.Suppliers == null)
            {
                return NotFound();
            }
            var SupplierData = await supplierDb.Suppliers.FirstOrDefaultAsync(x => x.SupplierId == id);

            if (SupplierData == null)
            {
                return NotFound();
            }
            return View(SupplierData);
        }

        [HttpPost , ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int? id)
        {

            var SupplierData = await supplierDb.Suppliers.FindAsync(id);

            if (SupplierData != null)
            {
                supplierDb.Suppliers.Remove(SupplierData);
            }
            await supplierDb.SaveChangesAsync();
            TempData["SuccessDelete"] = "Supplier Deleted.";
            return RedirectToAction("Index", "Supplier");



        }
    }
}