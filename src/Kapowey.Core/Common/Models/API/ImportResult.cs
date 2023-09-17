namespace Kapowey.Core.Common.Models.API
{
    public sealed class ImportResult
    {
        public int TotalRecordsImported { get; set; }

        public int TotalRecordsWithErrors { get; set; }

        public int TotalFilesImported { get; set; }

        public long TotalDuration { get; set; }

        public List<string> Messages { get; set; } = new List<string>();

        public List<string> Errors { get; set; } = new List<string>();

        public ImportResult()
        {
        }
    }
}
