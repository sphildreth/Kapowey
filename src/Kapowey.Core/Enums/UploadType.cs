using System.ComponentModel;

namespace Kapowey.Core.Enums;

public enum UploadType : byte
{
    [Description(@"Products")]
    Product,
    [Description(@"ProfilePictures")]
    ProfilePicture,
    [Description(@"Documents")]
    Document
}
