using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Packt.Shared;

namespace NorthwindMvc.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IHttpClientFactory clientFactory;
        public CustomersController(IHttpClientFactory httpClientFactory)
        {
            clientFactory = httpClientFactory;
        }

        // GET: Customers
        public async Task<IActionResult> Index(string country, string sortOrder, string searchString)
        {
            ViewData["CompanySortParm"] = sortOrder == "company_desc" ? "company" : "company_desc";
            ViewData["ContactSortParm"] = sortOrder == "contact_desc" ? "contact" : "contact_desc";
            ViewData["AddressSortParm"] = sortOrder == "address_desc" ? "address" : "address_desc";
            ViewData["PhoneSortParm"] = sortOrder == "phone_desc" ? "phone" : "phone_desc";

            ViewData["CurrentFilter"] = searchString;

            string uri;
            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers/";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }

            var client = clientFactory.CreateClient(name: "NorthwindService");

            var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            var customers = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();


            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.CompanyName.Contains(searchString)
                                       || c.ContactName.Contains(searchString)
                                       || c.Address.Contains(searchString)
                                       || c.Phone.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "company":
                    customers = customers.OrderBy(c => c.CompanyName);
                    break;
                case "contact":
                    customers = customers.OrderBy(c => c.ContactName);
                    break;
                case "address":
                    customers = customers.OrderBy(c => c.Address);
                    break;
                case "phone":
                    customers = customers.OrderBy(c => c.Phone);
                    break;
                case "company_desc":
                    customers = customers.OrderByDescending(c => c.CompanyName);
                    break;
                case "contact_desc":
                    customers = customers.OrderByDescending(c => c.ContactName);
                    break;
                case "address_desc":
                    customers = customers.OrderByDescending(c => c.Address);
                    break;
                case "phone_desc":
                    customers = customers.OrderByDescending(c => c.Phone);
                    break;
                default:
                    customers = customers.OrderByDescending(c => c.CompanyName);
                    break;
            }
            return View(customers);
        }

        public async Task<IActionResult> Customers(string country)
        {
            string uri;
            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers/";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }

            var client = clientFactory.CreateClient(name: "NorthwindService");

            var request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            var model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

            return View(model);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = clientFactory.CreateClient(name: "NorthwindService");
            HttpResponseMessage response = await client.GetAsync($"api/customers/{id}");
            Customer customer = await response.Content.ReadFromJsonAsync<Customer>();
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                var client = clientFactory.CreateClient(name: "NorthwindService");
                HttpResponseMessage response = await client.PostAsJsonAsync("api/customers", customer);
                response.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            return BadRequest(ModelState);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return NotFound(); // 404 Resource not found
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad request
            }

            var client = clientFactory.CreateClient(name: "NorthwindService");
            HttpResponseMessage response = await client.PutAsJsonAsync($"api/customers/{customer.CustomerID}", customer);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = clientFactory.CreateClient(name: "NorthwindService");

            HttpResponseMessage response = await client.DeleteAsync($"api/customers/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }

        private async Task<Customer> GetCustomerAsync(string id)
        {
            Customer customer = null;
            var client = clientFactory.CreateClient(name: "NorthwindService");
            HttpResponseMessage response = await client.GetAsync($"api/customers/{id}");
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadFromJsonAsync<Customer>();
            }
            return customer;
        }

    }
}
