﻿using System;
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
using FileUpload.Mongo;

namespace FileUpload.Pages
{
    public class IndexModel : PageModel
    {
        private const string BlobContainerNameA = "halzeltemp0", BlobContainerNameB = "halzeltemp1";
        private readonly ILogger<IndexModel> _logger;
        private readonly BlobContainerClient _blobContainerA, _blobContainerB;

        private readonly MyFileContext _context;

        private readonly MongoHelper _mongoHelper;

        [BindProperty]
        public MyFile fileUpload { get; set; }
        [BindProperty]
        public DBEntry dBEntry { get; set; }

        public IList<MyFile> myFilesA { get; set; }
        public IList<MyFile> myFilesB { get; set; }
        public IList<DBEntry> entries { get; set; }
        public IList<MyChat> chatHistory { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IAzureClientFactory<BlobServiceClient> blobServiceClientFactory, MyFileContext context, MongoHelper mongoHelper)
        {
            _logger = logger;

            var blobServiceA = blobServiceClientFactory.CreateClient("Default"); // storageA
            var blobServiceB = blobServiceClientFactory.CreateClient("storageB");

            _blobContainerA = blobServiceA.GetBlobContainerClient(BlobContainerNameA);
            _blobContainerB = blobServiceB.GetBlobContainerClient(BlobContainerNameB);

            _blobContainerA.CreateIfNotExists();
            _blobContainerB.CreateIfNotExists();

            myFilesA = new List<MyFile>();
            myFilesB = new List<MyFile>();

            entries = new List<DBEntry>();
            chatHistory = new List<MyChat>();

            _context = context;
            _mongoHelper = mongoHelper;
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

            files = new List<MyFile>();
            foreach (BlobItem blob in _blobContainerB.GetBlobs())
            {
                files.Add(new MyFile() { FileName = blob.Name });
            }
            myFilesB = files;

            entries = await _context.DbEntry.ToListAsync();

            chatHistory = _mongoHelper.GetAllChats();
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
            await _blobContainerB.UploadBlobAsync(fileUpload.FileName, fileUpload.File.OpenReadStream());

            // TODO: add entry to the DB
            dBEntry.FileName = fileUpload.FileName;
            _context.DbEntry.Add(dBEntry);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
