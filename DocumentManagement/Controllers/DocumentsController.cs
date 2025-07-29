using DocumentManagement.Data;
using DocumentManagement.Models.Entities;
using DocumentManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DocumentManagement.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly DocumentDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentsController(DocumentDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


        [HttpGet]
        public async Task<IActionResult> Inbox(string keyword, int? statusId, int? urgencyLevelId, int pageIndex = 1)
        {
            var currentUserId = GetCurrentUserId();
            const int pageSize = 10;

            // --- BƯỚC 1: Lấy ID của tất cả văn bản người dùng đã nhận ---
            var receivedDocumentIdsQuery = _context.DocumentRecipients
                                                   .Where(dr => dr.RecipientId == currentUserId && dr.IsDeleted != true)
                                                   .Select(dr => dr.DocumentId); // <<< SỬA TẠI ĐÂY: dr.Id => dr.DocumentId

            // --- BƯỚC 2: Query cơ sở để thống kê (chưa lọc) ---
            var allReceivedDocsQuery = _context.Documents
                                               .Include(d => d.Status) // QUAN TRỌNG: Include Status để thống kê
                                               .Where(d => receivedDocumentIdsQuery.Contains(d.Id));

            // --- BƯỚC 3: Tính toán các số liệu thống kê ---
            var totalCount = await allReceivedDocsQuery.CountAsync();
            var unprocessedCount = await allReceivedDocsQuery.CountAsync(d => d.Status != null && d.Status.StatusName == "Chưa xử lý");
            var inProgressCount = await allReceivedDocsQuery.CountAsync(d => d.Status != null && d.Status.StatusName == "Đang xử lý");
            var processedCount = await allReceivedDocsQuery.CountAsync(d => d.Status != null && d.Status.StatusName == "Đã xử lý");

            // --- BƯỚC 4: Query chính để lấy danh sách hiển thị và áp dụng bộ lọc ---
            var documentsQuery = allReceivedDocsQuery.AsQueryable(); // Bắt đầu từ query đã có

            if (!string.IsNullOrEmpty(keyword))
            {
                documentsQuery = documentsQuery.Where(d => d.Title.Contains(keyword));
            }
            if (statusId.HasValue)
            {
                documentsQuery = documentsQuery.Where(d => d.StatusId == statusId.Value);
            }
            if (urgencyLevelId.HasValue)
            {
                documentsQuery = documentsQuery.Where(d => d.UrgencyLevelId == urgencyLevelId.Value);
            }

            // --- BƯỚ-C 5: Phân trang ---
            var totalFilteredItems = await documentsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalFilteredItems / (double)pageSize);

            var documentsForPage = await documentsQuery
                                        .Include(d => d.Sender) // QUAN TRỌNG: Include Sender và UrgencyLevel
                                        .Include(d => d.UrgencyLevel)
                                        .OrderByDescending(d => d.CreatedAt)
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            // --- BƯỚC 6: Tạo ViewModel và trả về ---
            var viewModel = new InboxViewModel
            {
                Documents = documentsForPage,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                Keyword = keyword,
                StatusId = statusId,
                UrgencyLevelId = urgencyLevelId,
                StatusList = new SelectList(await _context.DocumentStatus.ToListAsync(), "Id", "StatusName", statusId),
                UrgencyLevelList = new SelectList(await _context.UrgencyLevels.ToListAsync(), "Id", "LevelName", urgencyLevelId),
                TotalCount = totalCount,
                UnprocessedCount = unprocessedCount,
                InProgressCount = inProgressCount,
                ProcessedCount = processedCount
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Sent(string keyword, int? statusId)
        {
            var currentUserId = GetCurrentUserId();

            // Query cơ sở: Lấy tất cả văn bản do người dùng hiện tại gửi
            var baseQuery = _context.Documents
                                    .Where(d => d.SenderId == currentUserId)
                                    .Include(d => d.Status); // Include Status để thống kê

            // --- Thống kê (tính trên toàn bộ văn bản đã gửi) ---
            var totalCount = await baseQuery.CountAsync();
            var inProgressCount = await baseQuery.CountAsync(d => d.Status != null && d.Status.StatusName == "Đang xử lý");
            var processedCount = await baseQuery.CountAsync(d => d.Status != null && d.Status.StatusName == "Đã xử lý");

            // --- Áp dụng bộ lọc để hiển thị ---
            var documentsQuery = baseQuery.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                documentsQuery = documentsQuery.Where(d => d.Title.Contains(keyword));
            }
            if (statusId.HasValue)
            {
                documentsQuery = documentsQuery.Where(d => d.StatusId == statusId);
            }

            // Lấy kết quả cuối cùng, include các thông tin cần thiết cho View
            var sentDocuments = await documentsQuery
                                        .Include(d => d.UrgencyLevel)
                                        .Include(d => d.DocumentRecipients) // QUAN TRỌNG: Lấy thông tin người nhận
                                            .ThenInclude(dr => dr.Recipient) // Lấy thông tin chi tiết của người nhận
                                        .OrderByDescending(d => d.CreatedAt)
                                        .ToListAsync();

            // Tạo ViewModel
            var viewModel = new SentViewModel
            {
                Documents = sentDocuments,
                Keyword = keyword,
                StatusId = statusId,
                StatusList = new SelectList(await _context.DocumentStatus.ToListAsync(), "Id", "StatusName"),
                // Thống kê
                TotalCount = totalCount,
                SentCount = totalCount,
                InProgressCount = inProgressCount,
                RespondedCount = processedCount
            };

            return View(viewModel);
        }

        // Chi tiết công văn
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var currentUserId = GetCurrentUserId(); // Giả sử bạn đã có hàm này

            // Lấy thông tin văn bản và tất cả các dữ liệu liên quan
            var document = await _context.Documents
                .Include(d => d.Sender)
                .Include(d => d.UrgencyLevel)
                .Include(d => d.Status)
                .Include(d => d.DocumentAttachments)
                .Include(d => d.DocumentLogs).ThenInclude(log => log.Performer)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            var recipientEntry = await _context.DocumentRecipients
                .FirstOrDefaultAsync(dr => dr.DocumentId == id && dr.RecipientId == currentUserId);

            if (recipientEntry != null && recipientEntry.IsMarked != true)
            {
                recipientEntry.IsMarked = true;

                _context.DocumentLogs.Add(new DocumentLog
                {
                    DocumentId = id,
                    Action = "Đã Đọc",
                    PerformedBy = currentUserId,
                    PerformedAt = DateTime.Now
                });

                await _context.SaveChangesAsync();
                document = await _context.Documents
                    .Include(d => d.Sender)
                    .Include(d => d.UrgencyLevel)
                    .Include(d => d.Status)
                    .Include(d => d.DocumentAttachments)
                    .Include(d => d.DocumentLogs).ThenInclude(log => log.Performer)
                    .FirstOrDefaultAsync(d => d.Id == id);
            }

            return View(document);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadAttachment(int id)
        {
            var attachment = await _context.DocumentAttachments.FindAsync(id);
            if (attachment == null || string.IsNullOrEmpty(attachment.FilePath))
            {
                return NotFound();
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "attachments", attachment.FilePath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Không tìm thấy tệp trên server.");
            }

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", attachment.FileName);
        }


        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchViewModel searchModel)
        {
            // Bắt đầu một query cơ bản, lấy tất cả văn bản và thông tin liên quan
            var query = _context.Documents
                                .Include(d => d.Sender)
                                .Include(d => d.Status)
                                .Include(d => d.UrgencyLevel)
                                .AsQueryable();

            // Áp dụng các bộ lọc nếu có
            if (!string.IsNullOrEmpty(searchModel.Keyword))
            {
                query = query.Where(d => d.Title.Contains(searchModel.Keyword) ||
                                         (d.Content != null && d.Content.Contains(searchModel.Keyword)) ||
                                         (d.Sender != null && d.Sender.FullName.Contains(searchModel.Keyword)));
            }
            if (searchModel.StatusId.HasValue)
            {
                query = query.Where(d => d.StatusId == searchModel.StatusId);
            }
            if (searchModel.UrgencyLevelId.HasValue)
            {
                query = query.Where(d => d.UrgencyLevelId == searchModel.UrgencyLevelId);
            }
            if (searchModel.SenderId.HasValue)
            {
                query = query.Where(d => d.SenderId == searchModel.SenderId);
            }
            if (searchModel.StartDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt >= searchModel.StartDate.Value);
            }
            if (searchModel.EndDate.HasValue)
            {
                query = query.Where(d => d.CreatedAt < searchModel.EndDate.Value.AddDays(1));
            }

            // Tạo ViewModel để truyền dữ liệu tới View
            var viewModel = new SearchViewModel
            {
                // Gán lại các tiêu chí tìm kiếm để form giữ lại giá trị
                Keyword = searchModel.Keyword,
                StatusId = searchModel.StatusId,
                UrgencyLevelId = searchModel.UrgencyLevelId,
                SenderId = searchModel.SenderId,
                StartDate = searchModel.StartDate,
                EndDate = searchModel.EndDate,

                // Lấy dữ liệu cho các dropdown
                StatusList = new SelectList(await _context.DocumentStatus.ToListAsync(), "Id", "StatusName"),
                UrgencyLevelList = new SelectList(await _context.UrgencyLevels.ToListAsync(), "Id", "LevelName"),
                SenderList = new SelectList(await _context.Users.Select(u => new { u.Id, Name = u.FullName + " (" + u.Workplace + ")" }).ToListAsync(), "Id", "Name"),

                // Thực thi query để lấy kết quả
                SearchResults = await query.OrderByDescending(d => d.CreatedAt).ToListAsync()
            };

            viewModel.ResultCount = viewModel.SearchResults.Count();

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var currentUserId = GetCurrentUserId();

            // Lấy 3 văn bản đã gửi gần đây nhất
            var recentDocs = await _context.Documents
                                           .Where(d => d.SenderId == currentUserId)
                                           .OrderByDescending(d => d.CreatedAt)
                                           .Take(3)
                                           .ToListAsync();

            var viewModel = new CreateDocumentViewModel
            {
                // Lấy danh sách người dùng để chọn người nhận
                RecipientList = new SelectList(
                    await _context.Users.Where(u => u.Id != currentUserId).ToListAsync(),
                    "Id",
                    "FullName"
                ),
                // Lấy danh sách cho các dropdown khác nếu cần
                UrgencyLevelList = new SelectList(await _context.UrgencyLevels.ToListAsync(), "Id", "LevelName"),
                StatusList = new SelectList(await _context.DocumentStatus.ToListAsync(), "Id", "StatusName"),
                // Gán danh sách văn bản gần đây
                RecentDocuments = recentDocs
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocumentViewModel viewModel, List<IFormFile> attachments)
        {
            var currentUserId = GetCurrentUserId();
            if (ModelState.IsValid && viewModel.RecipientIds != null && viewModel.RecipientIds.Any())
            {
                var doc = viewModel.Document;
                doc.SenderId = currentUserId;
                doc.CreatedAt = DateTime.Now;

                _context.Documents.Add(doc);
                await _context.SaveChangesAsync();

                // Xử lý tệp đính kèm
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "attachments");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        await using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        var attachment = new DocumentAttachment
                        {
                            DocumentId = doc.Id,
                            FileName = file.FileName,
                            FilePath = uniqueFileName
                        };
                        _context.DocumentAttachments.Add(attachment);
                    }
                }

                // Xử lý người nhận
                foreach (var recipientId in viewModel.RecipientIds)
                {
                    var recipient = new DocumentRecipient
                    {
                        DocumentId = doc.Id,
                        RecipientId = recipientId,
                        IsMarked = false,
                        IsDeleted = false
                    };
                    _context.DocumentRecipients.Add(recipient);
                }

                // Ghi log
                _context.DocumentLogs.Add(new DocumentLog
                {
                    DocumentId = doc.Id,
                    Action = "Tạo và gửi công văn",
                    PerformedBy = currentUserId,
                    PerformedAt = DateTime.Now
                });

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Sent));
            }
            // Nếu lỗi, nạp lại danh sách cho dropdown
            viewModel.UrgencyLevelList = new SelectList(await _context.UrgencyLevels.ToListAsync(), "Id", "LevelName", viewModel.Document.UrgencyLevelId);
            viewModel.StatusList = new SelectList(await _context.DocumentStatus.ToListAsync(), "Id", "StatusName", viewModel.Document.StatusId);
            viewModel.RecipientList = new SelectList(await _context.Users.Where(u => u.Id != currentUserId).ToListAsync(), "Id", "FullName");
            return View(viewModel);
        }
    }
}
