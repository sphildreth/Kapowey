namespace Kapowey.Core.Enums
{
    /// <summary>
    /// Do not modify these definitions unless you also modify the e_status database definition.
    /// </summary>
    public enum Status
    {
        New,
        Imported,
        Ok,
        Edited,
        PendingReview,
        UnderReview,
        Locked,
        Inactive
    }
}