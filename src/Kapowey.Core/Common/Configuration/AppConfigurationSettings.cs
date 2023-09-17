using System.Diagnostics;
using Kapowey.Core.Imaging;

namespace Kapowey.Core.Common.Configuration;

/// <summary>
///     Configuration wrapper for the app configuration section
/// </summary>
public class AppConfigurationSettings
{
    /// <summary>
    ///     App configuration key constraint
    /// </summary>
    public const string Key = nameof(AppConfigurationSettings);
    
    /// <summary>
    ///     Contains the application secret, used for signing
    /// </summary>
    public string Secret { get; set; } = string.Empty;
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public bool BehindSSLProxy { get; set; }
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public string ProxyIP { get; set; } = string.Empty;
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public string ApplicationUrl { get; set; } = string.Empty;
    
    /// <summary>
    ///     Undocumented
    /// </summary>
    public bool Resilience { get; set; }
    
    /// <summary>
    /// Full path to use for root folder to hold data.
    /// </summary>
    public string StorageFolder { get; set; }

    /// <summary>
    /// This is the phsycial folder of the applications 'wwwroot' content folder (which holds among other things place-holder default images).
    /// </summary>
    public string WebRootPath { get; set; }

    /// <summary>
    /// Perform any checks to ensure that system is setup according to configuration.
    /// </summary>
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

    /// <summary>
    /// Size to serve thumbnail images if no size is given on request.
    /// </summary>
    ImageSize ThumbnailSize { get; set; }      
}