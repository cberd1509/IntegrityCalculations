using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Macross.Json.Extensions;

[JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumMemberConverter))]  // This custom converter was placed in a system namespace.
public enum CasingSectionType
{
    [EnumMember(Value = "CASING")]
    CASING,
    [EnumMember(Value = "LINER")]
    LINER,
    [EnumMember(Value = "TUBING")]
    TUBING
}