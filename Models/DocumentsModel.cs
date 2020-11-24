using System;
using System.Collections.Generic;

namespace UserApi.Models
{
    public class DocumentsModel
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { set; get; }
        public List<Guid> UpdatedBy { set; get; }
        public DateTime CreatedAt { set; get; }
        public DateTime UpdatedAt { set; get; }
        public string OriginalFilename { set; get; }
        public string Filename { set; get; }
        
    }
}