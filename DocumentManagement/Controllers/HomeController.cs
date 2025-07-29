using DocumentManagement.Data;
using DocumentManagement.Models;
using DocumentManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentManagement.Data;
using DocumentManagement.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

public class HomeController : Controller
{
    private readonly DocumentDbContext _context;

    public HomeController(DocumentDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var totalIncoming = await _context.DocumentRecipients
                                          .CountAsync(dr => dr.RecipientId == userId && dr.IsDeleted != true);

        var unreadIncoming = await _context.DocumentRecipients
                                           .CountAsync(dr => dr.RecipientId == userId && dr.IsDeleted != true && dr.IsMarked != true);

        var totalSent = await _context.Documents
                                      .CountAsync(d => d.SenderId == userId);

        var receivedDocs = _context.DocumentRecipients
                                .Where(dr => dr.RecipientId == userId)
                                .Select(dr => dr.Document);

        var sentDocs = _context.Documents
                             .Where(d => d.SenderId == userId);

        var recentDocuments = await receivedDocs.Union(sentDocs)
                                        .Include(d => d.Sender)
                                        .OrderByDescending(d => d.CreatedAt)
                                        .Take(5)
                                        .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            TotalIncoming = totalIncoming,
            UnreadIncoming = unreadIncoming,
            TotalSent = totalSent,
            RecentDocuments = recentDocuments
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}