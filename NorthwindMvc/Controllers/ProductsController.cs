using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace NorthwindMvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Northwind _context;

        public ProductsController(Northwind context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["CategorySortParm"] = sortOrder == "category_desc" ? "category" : "category_desc";
            ViewData["SupplierSortParm"] = sortOrder == "supplier_desc" ? "supplier" : "supplier_desc";
            ViewData["ProductSortParm"] = sortOrder == "product_desc" ? "product" : "product_desc";
            ViewData["UnitPriceSortParm"] = sortOrder == "unit_price_desc" ? "unit_price" : "unit_price_desc";
            ViewData["UnitInStockSortParm"] = sortOrder == "unit_in_stock_desc" ? "unit_in_stock" : "unit_in_stock_desc";
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Product> products = _context.Products.Include(p => p.Category).Include(p => p.Supplier);
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.ProductName.Contains(searchString)
                                       || p.Category.CategoryName.Contains(searchString)
                                       || p.Supplier.CompanyName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "category":
                    products = products.OrderBy(p => p.Category.CategoryName);
                    break;
                case "supplier":
                    products = products.OrderBy(p => p.Supplier.CompanyName);
                    break;
                case "product":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "unit_price":
                    products = products.OrderBy(p => p.UnitPrice);
                    break;
                case "unit_in_stock":
                    products = products.OrderBy(p => p.UnitsInStock);
                    break;
                case "category_desc":
                    products = products.OrderByDescending(p => p.Category.CategoryName);
                    break;
                case "supplier_desc":
                    products = products.OrderByDescending(s => s.Supplier.CompanyName);
                    break;
                case "product_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "unit_price_desc":
                    products = products.OrderByDescending(p => p.UnitPrice);
                    break;
                case "unit_in_stock_desc":
                    products = products.OrderByDescending(p => p.UnitsInStock);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            return View(await products.AsNoTracking().ToListAsync());

        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierID", "CompanyName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierID", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierID", "CompanyName", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductName,SupplierId,CategoryId,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierID", "CompanyName", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        //TO DO: add CategoriesController, CustomersController
    }
}
