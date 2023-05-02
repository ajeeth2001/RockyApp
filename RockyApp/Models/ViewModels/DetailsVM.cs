using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockyApp.Models.ViewModels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Product = new Product();
            //ExistsInCart = false;
        }
        public Product Product { get; set; }
        public bool ExistsInCart { get; set; }
    }
}
