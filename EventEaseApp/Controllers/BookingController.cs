using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEaseApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumeric = int.TryParse(searchString, out int bookingId);

                bookings = bookings.Where(b =>
                    b.Venue.VenueName.Contains(searchString) ||
                    b.Event.EventName.Contains(searchString) ||
                    (isNumeric && b.BookingId == bookingId));

            }

            return View(await bookings.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations");
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName");
            return View(new Booking());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventId == booking.EventId);

            if (selectedEvent == null)
            {

                ModelState.AddModelError("", "Selected Event not found");
                ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations", booking.VenueId);
                ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
                return View(booking);
            }

            var conflict = await _context.Booking
                          .Include(b => b.Event)
                          .AnyAsync(b => b.VenueId == booking.VenueId &&
                          b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)
            {

                ModelState.AddModelError("", "This venue is already booked for that date");
            }

                if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations", booking.VenueId);
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }    
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations", booking.VenueId);
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            return View(booking);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
          
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations", booking.VenueId);
            ViewBag.EventId = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            return View(booking);
        }
    


public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var booking = await _context.Booking.FindAsync(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
    }
}