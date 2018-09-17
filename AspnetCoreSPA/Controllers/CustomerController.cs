using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreSPATemplate.Mappers;
using AspnetCoreSPATemplate.Models;
using AspnetCoreSPATemplate.ViewModels;

namespace AspnetCoreSPATemplate.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class CustomerController : Controller
    {
        private IHostingEnvironment _env;
        public CustomerController(IHostingEnvironment env)
        {
            _env = env;
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var listCustomer = GetAllCustomer();
            return Json(new { customer = listCustomer.Where(x => x.Id == id).FirstOrDefault() });
        }
        [HttpPost("getlistcustomer")]
        public ActionResult GetListCustomer([FromBody]CustomerSearchModel model=null)
        {
            var listCustomer = GetAllCustomer();
            if(model != null)
            {
                if(!String.IsNullOrEmpty(model.TextSearch))
                    listCustomer = listCustomer.Where(x => x.FirstName.Contains(model.TextSearch) ||
                                                        x.LastName.Contains(model.TextSearch) ||
                                                        x.Email.Contains(model.TextSearch) ||
                                                        x.Phone.Contains(model.TextSearch) 
                                            );
                switch (model.SortKey.ToLower())
                {
                    case "lastname":
                        if(model.OrderBy.ToLower()=="asc")
                            listCustomer = listCustomer.OrderBy(x => x.LastName);
                        else
                            listCustomer = listCustomer.OrderByDescending(x => x.LastName);
                        break;
                    case "email":
                        if (model.OrderBy.ToLower() == "asc")
                            listCustomer = listCustomer.OrderBy(x => x.Email);
                        else
                            listCustomer = listCustomer.OrderByDescending(x => x.Email);
                        break;
                    case "phone":
                        if (model.OrderBy.ToLower() == "asc")
                            listCustomer = listCustomer.OrderBy(x => x.Phone);
                        else
                            listCustomer = listCustomer.OrderByDescending(x => x.Phone);
                        break;
                    case "firstname":
                        if (model.OrderBy.ToLower() == "asc")
                            listCustomer = listCustomer.OrderBy(x => x.FirstName);
                        else
                            listCustomer = listCustomer.OrderByDescending(x => x.FirstName);
                        break;
                }
            }

            var data = (model == null) ? listCustomer.ToList() : listCustomer.Skip((model.CurrentPage-1) * model.NumberItemOnPage).Take(model.NumberItemOnPage).ToList();
            return Json(new { total = listCustomer.Count(), customers = data });
        }
        private IEnumerable<CustomerViewModel> GetAllCustomer()
        {
            var webRoot = _env.WebRootPath;
            string path = System.IO.Path.Combine(webRoot, "App_Data\\customer.csv");

            FileStream fileStream = new FileStream(path, FileMode.Open);
            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.RegisterClassMap<CustomerModelMapper>();
                var records = csv.GetRecords<CustomerViewModel>().ToList();
                return records;
            }

        }

    }
}