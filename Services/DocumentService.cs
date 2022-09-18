using System;
using UserApi.Models;
using UserApi.Repository;

namespace UserApi.Services
{
    public class DocumentService : BaseServiceRepository<DocumentsModel>
    {
        public DocumentService(DBContext context, string collectionName = "Document") : base(context, collectionName)
        {
        }
    }
}