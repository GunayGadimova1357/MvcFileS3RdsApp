using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcS3Files.Data;
using MvcS3Files.Models;
using MvcS3Files.Services;

namespace MvcS3Files.Controllers;

public class FilesController : Controller
{
    private readonly AppDbContext _db;
    private readonly S3FileService _s3;

    public FilesController(AppDbContext db, S3FileService s3)
    {
        _db = db;
        _s3 = s3;
    }

    // GET: /Files
    public async Task<IActionResult> Index()
    {
        var items = await _db.FileItems.ToListAsync();
        return View(items);
    }

    // GET: /Files/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.FileItems.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // GET: /Files/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Files/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FileItem model, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            // Console.WriteLine("File is missing!");
            ModelState.AddModelError("file", "File is required");
            return View(model);
        }

        Console.WriteLine("File received: " + file.FileName);

        var fileKey = await _s3.UploadAsync(file);
        // Console.WriteLine("File uploaded with key: " + fileKey);

        model.FileKey = fileKey;
        model.CreatedAt = DateTime.Now;

        _db.FileItems.Add(model);
        await _db.SaveChangesAsync();

        // Console.WriteLine("Saved to DB with ID: " + model.Id);

        return RedirectToAction(nameof(Index));
    }

    // GET: /Files/Download/5
    public async Task<IActionResult> Download(int id)
    {
        var item = await _db.FileItems.FindAsync(id);
        if (item == null) return NotFound();

        var stream = await _s3.DownloadAsync(item.FileKey);

        if (stream == null)
            return NotFound("File not found in S3");

        return File(stream, "application/octet-stream", item.Name);

    }

    // GET: /Files/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.FileItems.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    // POST: /Files/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.FileItems.FindAsync(id);
        if (item == null) return NotFound();

        await _s3.DeleteAsync(item.FileKey); 
        _db.FileItems.Remove(item);         
        await _db.SaveChangesAsync();       

        return RedirectToAction(nameof(Index));
    }
}
