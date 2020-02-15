using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using FileUpload.Models;
using Azure.Storage.Blobs.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

namespace FileUpload.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly string BlobContainerNameA = "halzeltemp0";
        private readonly ILogger<IndexModel> _logger;
        private readonly BlobContainerClient _blobContainerA;

        private readonly MyFileContext _context;

        [BindProperty]
        public MyFile fileUpload { get; set; }
        [BindProperty]
        public DBEntry dBEntry { get; set; }

        public IList<MyFile> myFilesA { get; set; }
        public IList<DBEntry> entries { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IAzureClientFactory<BlobServiceClient> factory, MyFileContext context)
        {
            _logger = logger;

            var blobServiceA = factory.CreateClient("storageA");
            _blobContainerA = blobServiceA.GetBlobContainerClient(BlobContainerNameA);
            _blobContainerA.CreateIfNotExists();

            myFilesA = new List<MyFile>();
            entries = new List<DBEntry>();

            _context = context;
        }

        public async Task OnGetAsync()
        {
            List<MyFile> files = new List<MyFile>();
            // storage A
            foreach (BlobItem blob in _blobContainerA.GetBlobs())
            {
                files.Add(new MyFile() { FileName = blob.Name });
            }

            myFilesA = files;

            entries = await _context.DbEntry.ToListAsync();
        }

        public async Task<ActionResult> OnGetDownloadFileAsync(string id)
        {
            BlobClient client = _blobContainerA.GetBlobClient(id);
            string filePath = Path.GetTempFileName();
            await client.DownloadToAsync(filePath);
            return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", id);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Perform an initial check to catch FileUpload class
            // attribute violations.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(Path.GetExtension(fileUpload.FileName)))
            {
                fileUpload.FileName += Path.GetExtension(fileUpload.File.FileName);
            }

            await _blobContainerA.UploadBlobAsync(fileUpload.FileName, fileUpload.File.OpenReadStream());

            // TODO: add entry to the DB
            dBEntry.FileName = fileUpload.FileName;
            _context.DbEntry.Add(dBEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
