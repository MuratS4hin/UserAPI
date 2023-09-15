using System;
using UserApi.Models;
using UserApi.Repository;
using UserApi.Settings;

namespace UserApi.Services
{
    public class DocumentService : BaseServiceRepository<DocumentsModel>
    {
        public DocumentService(DBContext context, IDocumentDatabaseSettings settings) : base(context, settings.CollectionName)
        {
        }
    }
}