using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RentalAuto.Models;

namespace RentalAuto.Pages.BookingDetails
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RentalAuto.Models.RentalDbContext _context;

        public IndexModel(RentalAuto.Models.RentalDbContext context)
        {
            _context = context;
        }

        public IList<BookingDetail> BookingDetail { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.BookingDetails != null)
            {
                BookingDetail = await _context.BookingDetails.ToListAsync();
            }
        }
    }
}