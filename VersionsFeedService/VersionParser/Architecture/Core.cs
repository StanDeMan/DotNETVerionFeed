using System.Runtime.Serialization;

namespace VersionsFeedService.VersionParser.Architecture
{
    public enum Version
    {
        /// <summary>
        /// 6.0 version
        /// </summary>
        [EnumMember(Value = "6.0")] 
        Core6,

        /// <summary>
        /// 7.0 version
        /// </summary>
        [EnumMember(Value = "7.0")] 
        Core7,

        /// <summary>
        /// 8.0 version
        /// </summary>
        [EnumMember(Value = "8.0")] 
        Core8
    }
}