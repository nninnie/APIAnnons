using APIAnnons.Api.Data;
using APIAnnons.Api.DTOs;
using APIAnnons.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIAnnons.Api.Controllers;

// Handles CRUD operations for ads via /Ads
[ApiController]
[Route("[controller]")]
public class AdsController : ControllerBase
{
    private readonly AppDbContext _context;

    // The DbContext is injected by dependency injection
    public AdsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /Ads - returns all ads, newest first
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AdDto>>> GetAll()
    {
        var ads = await _context.Ads
            .AsNoTracking()
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => ToDto(a))
            .ToListAsync();

        return Ok(ads);
    }

    // GET /Ads/{id} - returns one ad, or 404 if it does not exist
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdDto>> GetById(int id)
    {
        var ad = await _context.Ads.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        if (ad is null)
            return NotFound();

        return Ok(ToDto(ad));
    }

    // POST /Ads - creates a new ad and returns 201 with a Location header
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdDto>> Create(CreateAdDto dto)
    {
        var ad = new Ad
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Category = dto.Category,
            Location = dto.Location,
            CreatedAt = DateTime.UtcNow
        };

        _context.Ads.Add(ad);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = ad.Id }, ToDto(ad));
    }

    // PUT /Ads/{id} - full update, all fields are replaced
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, UpdateAdDto dto)
    {
        var ad = await _context.Ads.FindAsync(id);

        if (ad is null)
            return NotFound();

        ad.Title = dto.Title;
        ad.Description = dto.Description;
        ad.Price = dto.Price;
        ad.Category = dto.Category;
        ad.Location = dto.Location;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /Ads/{id} - deletes an ad, or returns 404 if it does not exist
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var ad = await _context.Ads.FindAsync(id);

        if (ad is null)
            return NotFound();

        _context.Ads.Remove(ad);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Maps an Ad entity to an AdDto in one place
    private static AdDto ToDto(Ad ad) => new()
    {
        Id = ad.Id,
        Title = ad.Title,
        Description = ad.Description,
        Price = ad.Price,
        Category = ad.Category,
        Location = ad.Location,
        CreatedAt = ad.CreatedAt
    };
}