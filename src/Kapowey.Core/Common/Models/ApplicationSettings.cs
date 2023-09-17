using System.Diagnostics;
using Kapowey.Core.Imaging;

namespace Kapowey.Core.Common.Models
{
    public sealed class ApplicationSettings 
    {
        public bool UseSSLBehindProxy { get; set; }

        public string BehindProxyHost { get; set; }

        public string WebRootPath { get; set; }

        public string StorageFolder { get; set; } = "%CUR_DIR%\\Kapowey";

        public ImageSize ThumbnailSize { get; set; }

        public string UserImageFolder
        {
            get
            {
                return Path.Combine(StorageFolder, "users");
            }
        }

        public ApplicationSettings()
        {
            ThumbnailSize = new ImageSize();
        }

        public void EnsureSetup()
        {
            StorageFolder = StorageFolder.Replace("%CUR_DIR%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                                         .Replace("%APPDATA%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            if (!Directory.Exists(StorageFolder))
            {
                Directory.CreateDirectory(StorageFolder);
                Trace.WriteLine($"+ Created Storage Folder [{ StorageFolder }]");
            }
        }
    }
}