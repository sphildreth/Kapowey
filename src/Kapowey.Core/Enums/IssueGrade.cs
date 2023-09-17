using System.ComponentModel;

namespace Kapowey.Core.Enums
{
    public enum IssueGrade
    {
        NotSpecified,

        [Description("NM")]
        NearMint,

        [Description("VF")]
        VeryFine,

        [Description("FN")]
        Fine,

        [Description("FG")]
        VeryGood,

        [Description("GD")]
        Good,

        [Description("FR")]
        Fair,

        [Description("PR")]
        Poor
    }
}