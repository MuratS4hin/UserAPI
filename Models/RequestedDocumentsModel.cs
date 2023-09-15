using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace UserApi.Models
{
    public class RequestedDocumentsModel
    {
        public Guid Id { get; set; }

        public IFormFile Files { get; set; }
        
        public DocumentsModel SetDocumentModel(User user)
        {
            if (user == null) return null;
            var document = new DocumentsModel()
            {
                Id = Guid.NewGuid(),
                CreatedBy = user.Id,
                UpdatedBy= new List<Guid> {user.Id},
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                OriginalFilename = Files.FileName,
                Filename = Files.FileName
            };
            return document;
        }
    }
}