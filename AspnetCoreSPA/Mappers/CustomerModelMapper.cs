using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspnetCoreSPATemplate.ViewModels;

namespace AspnetCoreSPATemplate.Mappers
{

    public sealed class CustomerModelMapper : ClassMap<CustomerViewModel>
    {
        public CustomerModelMapper() 
        {
            Map(m => m.Id).Name("Id");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Email).Name("Email");
            Map(m => m.Phone).Name("Phone");

        }
    }
}
