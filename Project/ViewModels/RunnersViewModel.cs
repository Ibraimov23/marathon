using Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.ViewModels
{
    public class RunnersViewModel
    {
        public IEnumerable<Runner> Runners { get; set; }
        public SelectList Users { get; set; }
    }
}
