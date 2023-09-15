using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UserApi.Middleware;
using UserApi.Models;
using UserApi.Services;
using UserApi.Settings;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentService _documentService;
        private readonly UserService _userService;
        private readonly string _uploadPath;
        private readonly string _downloadPath;
        private readonly ILogger<DocumentsController> _logger;
        private readonly MimeTypes _mimeTypes;
        private readonly IAuthorizeSettings _authorizeSettings;

        public DocumentsController( DocumentService documentService, UserService userService, ILogger<DocumentsController> logger, 
            MimeTypes mimeTypes, IAuthorizeSettings authorizeSettings)
        {
            _authorizeSettings = authorizeSettings;
            _mimeTypes = mimeTypes;
            _logger = logger;
            _userService = userService;
            _documentService = documentService;
            _uploadPath = "C:/Users/sahmu/OneDrive/Masaüstü/OKUL/STAJ/Tesojer/C#/1-STAJ" + "\\Upload\\";
            _downloadPath = "C:/Users/sahmu/OneDrive/Masaüstü/OKUL/STAJ/Tesojer/C#/1-STAJ" + "\\Download\\";
        }

        


        
        //[AllowAnonymous]
        //[HttpPost("Login")]
        //public string Login([FromForm]string username, string password)
        //{
        //    var user = IfLoginSuccessfulGetUser(username, password);
        //    return CreateJwtToken(user);
        //}

        [HttpGet]
        public async Task<List<DocumentsModel>> Get()
        {
            return _documentService.Find();
        }
        
        [HttpGet("Download")]
        public async Task<string> DownloadDocument([FromForm]Guid userId, Guid documentId)
        {
            var user = GetUserById(userId);
            DoesUserExist(user,userId);
            DoesAuthorizedTo(user,userId,"Download");
            var document = GetDocumentById(documentId);
            DoesDocumentExist(document);
            CheckThePathsExistenceAndCreateIfNotExist(_downloadPath);
            var myWebClient = new WebClient();
             myWebClient.DownloadFile(_uploadPath+document.OriginalFilename,_downloadPath+document.OriginalFilename);
            _logger.LogInformation(MyLogEvents.GetItem,"Download the item {Id} at {RequestTime}", documentId, DateTime.Now);
            return "Done";
        }

        [HttpPost("Upload")]
        public async Task<string> UploadFile([FromForm] RequestedDocumentsModel requestedDocument)
        {
            var user = GetUserById(requestedDocument.Id);
            DoesUserExist(user, requestedDocument.Id);
            DoesAuthorizedTo(user, requestedDocument.Id, "Upload");
            DoesUploadingDocumentEmpty(requestedDocument);
            new FileExtensionContentTypeProvider().TryGetContentType(requestedDocument.Files.FileName ,out string contentType);
            DoesMimeTypeSupported(contentType);
            CheckThePathsExistenceAndCreateIfNotExist(_uploadPath);
            System.IO.File.Create(_uploadPath + requestedDocument.Files.FileName);
            var documentModel = requestedDocument.SetDocumentModel(user);
            _documentService.Create(documentModel);
            return "\\Uploaded\\" + requestedDocument.Files.FileName;
            
        }

        [HttpPost("Update")]
        public async Task<string> UpdateUploadedFile(RequestedDocumentsModel objFile, [FromForm]Guid userId, Guid documentId)
        {
            var user = GetUserById(userId);
            var document = GetDocumentById(documentId);
            DoesUserExist(user,userId);
            DoesAuthorizedTo(user,userId,"Update");
            DoesDocumentExist(document);
            DoesUploadingDocumentEmpty(objFile);
            CheckThePathsExistenceAndCreateIfNotExist(_uploadPath);
            System.IO.File.Delete(_uploadPath + document.Filename);//deletes the file
            System.IO.File.Create(_uploadPath + objFile.Files.FileName);//uploads the file
            _documentService.Update(x => x.Id == documentId, doc => doc.Filename, objFile.Files.FileName);
            _documentService.Update(x => x.Id == documentId, doc => doc.UpdatedAt, DateTime.Now);
            document.UpdatedBy.Add(userId);
            return "\\Updated\\" + objFile.Files.FileName;

        }





        private User GetUserById(Guid id)
        {
            _logger.LogInformation(MyLogEvents.GetItem,"User ({id}) has successfully found", id);
            return _userService.FindById(x => x.Id == id);
        }

        private DocumentsModel GetDocumentById(Guid id)
        {
            _logger.LogInformation(MyLogEvents.GetItem,"Document ({id}) has successfully found", id);
            return _documentService.FindById(x => x.Id == id);
        }

        private void CheckThePathsExistenceAndCreateIfNotExist(string path)
        {
            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
            _logger.LogInformation(MyLogEvents.GetItemNotFound, "Path ({id}) NOT FOUND but CREATED", path);
        }

        private void DoesUserExist(User user, object userId)
        {
            if (user != null) return;
            _logger.LogWarning(MyLogEvents.GetItemNotFound, "User ({id}) NOT FOUND", userId);
            throw new NotFound();
        }
        
        private void DoesAuthorizedTo(User user, object userId, string permission)
        {
            if (user.PermissionList.Contains(permission)) return;
            _logger.LogWarning(MyLogEvents.UserNotAuthorized,"User ({id}) NOT AUTHORIZED to ({perm}) the document", userId, permission);
            throw new NotAuthorized();
        }

        private void DoesUploadingDocumentEmpty(RequestedDocumentsModel objFile)
        {
            if (objFile.Files.Length > 0) return;
            _logger.LogWarning(MyLogEvents.UpdateItemNotFound,"Document Cannot Be Empty");
            throw new BadRequest();
        }
        
        private void DoesDocumentExist(object document)
        {
            if (document != null) return;
            _logger.LogWarning(MyLogEvents.GetItemNotFound,"document NOT FOUND");
            throw new NotFound();
        }
        
        private void DoesMimeTypeSupported(string contentType)
        {
            if (!_mimeTypes.AcceptedMimeTypes.Contains(contentType))
            {
                throw new UnsupportedMediaType();
            }
        }
        
        private User IfLoginSuccessfulGetUser(string username, string password)
        {
            foreach (var user in _userService.Find().Where(user => user.Name == username && user.Password == password))
            {
                return user;
            }
            throw new NotFound();
        }

        private string CreateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authorizeSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(3),
                Issuer = _authorizeSettings.Issuer,
                Audience = _authorizeSettings.Audience,
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

/*[AllowAnonymous]
        [HttpPost]
        public string Login2(string name, string password)
        {
            IfLoginSuccessfulGetUser(name, password);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authorizeSettings.Key));    
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _authorizeSettings.Issuer,    
                _authorizeSettings.Audience,
                claims: null,    
                expires: DateTime.Now.AddMinutes(120),    
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);  
        }*/