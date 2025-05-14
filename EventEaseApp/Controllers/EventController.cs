﻿using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace EventEaseApp.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Event.Include(v => v.Venue).ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations");
            return View(new Event());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created succesfully.";
                return RedirectToAction(nameof(Index));

            }
            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations",events.VenueId);
            return View(events);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _context.Event.FindAsync(id);
            if (evt == null) return NotFound();

            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations", evt.VenueId);

            return View(evt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event evt)
        {
            if (id != evt.EventId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(evt);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated succesfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.VenueId = new SelectList(_context.Venue, "VenueId", "Locations", evt.VenueId);

            return View(evt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var evt = await _context.Event
                 .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);
            if (evt == null) return NotFound();
            return View(evt);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _context.Event.FindAsync(id);

            if (evt == null) return NotFound();
            var isBooked = await _context.Booking.AnyAsync(b => b.EventId == id);

            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has exisitng bookings.";
                return RedirectToAction(nameof(Index));
            }
            _context.Event.Remove(evt);
            TempData["SuccessMessage"] = "Event deleted succesfully.";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {

            var events = await _context.Event.Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.EventId == id);

            if (events == null)
            {

                return NotFound();

            }
            return View(events);
        }
    }
}
