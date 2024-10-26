using System.Runtime.Serialization;

namespace VersionsFeedService.VersionParser.Architecture
{
    public enum Version
    {
        /// <summary>
        /// 8.0 version
        /// </summary>
        [EnumMember(Value = "8.0")] 
        Core8,

        /// <summary>
        /// 9.0 version
        /// </summary>
        [EnumMember(Value = "9.0")] 
        Core9
    }
}