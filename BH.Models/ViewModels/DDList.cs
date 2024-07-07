using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ViewModels
{
    public class DDList
    {
        public List<SelectListItem> Items { get; set; }
        public DDList()
        {
            Items = new List<SelectListItem>();
        }

        public string Caption { get; set; }

        public string Name { get; set; }

        public int Selected { get; set; }

    }
}
