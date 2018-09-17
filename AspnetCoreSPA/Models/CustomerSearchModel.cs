using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreSPATemplate.Models
{
    public class CustomerSearchModel
    {
        public string TextSearch { get; set; }
        public int CurrentPage { get; set; }
        public int NumberItemOnPage { get; set; }
        public string SortKey { get; set; }
        public string OrderBy { get; set; }
    }
}
