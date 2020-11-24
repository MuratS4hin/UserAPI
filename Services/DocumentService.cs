using System;
using UserApi.Models;
using UserApi.Repository;

namespace UserApi.Services
{
    public class DocumentService : BaseRepository<DocumentsModel>
    {
        public DocumentService(Context context, string collectionName = "Document") : base(context, collectionName)
        {
        }
    }
}