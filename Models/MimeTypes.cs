using System.Collections.Generic;

namespace UserApi.Models
{
    public class MimeTypes
    {
        public readonly List<string> AcceptedMimeTypes = new List<string>()
        {
            MimeMapping.KnownMimeTypes.Pdf,
            MimeMapping.KnownMimeTypes.Png,
            MimeMapping.KnownMimeTypes.Jpeg,
            MimeMapping.KnownMimeTypes.Zip,
            MimeMapping.KnownMimeTypes.Xlsx
        };
    }
}