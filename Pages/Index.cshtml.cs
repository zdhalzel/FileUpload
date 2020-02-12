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

namespace FileUpload.Pages
{
    public class IndexModel : PageModel
    {
        private static readonly string BlobContainerName = "halzeltemp0"; // TODO
        private readonly ILogger<IndexModel> _logger;
        private readonly BlobContainerClient _blobContainer;

        [BindProperty]
        public MyFile fileUpload { get; set; }

        public IList<MyFile> myFiles { get; set; }


        public IndexModel(ILogger<IndexModel> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;

            var blobService = blobServiceClient;
            _blobContainer = blobService.GetBlobContainerClient(BlobContainerName);
            _blobContainer.CreateIfNotExists();
            myFiles = new List<MyFile>();
        }

        public void OnGetAsync()
        {
            List<MyFile> files = new List<MyFile>();
            foreach (BlobItem blob in _blobContainer.GetBlobs())
            {
                files.Add(new MyFile() { FileName = blob.Name });
            }

            myFiles = files;
        }

        public async Task<ActionResult> OnGetDownloadFileAsync(string id)
        {
            BlobClient client = _blobContainer.GetBlobClient(id);
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

            await _blobContainer.UploadBlobAsync(fileUpload.FileName, fileUpload.File.OpenReadStream());
            return RedirectToPage("./Index");
        }
    }
}
