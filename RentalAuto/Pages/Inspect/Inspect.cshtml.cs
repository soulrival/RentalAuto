using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.Inspect
{
    [Authorize(Policy = "RentalInspector")]
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<Car> Cars { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Cars = await _context.Cars.ToListAsync();
        }
    }
}