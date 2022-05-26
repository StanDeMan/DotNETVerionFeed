using System.Runtime.Serialization;

namespace DotNETVersionFeed.VersionParser.Extensions
{
    /// <summary>
    /// <see cref="T:System.Enum" /> extensions.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts an enumeration value into a string, using the <see cref="T:System.Runtime.Serialization.EnumMemberAttribute" />
        /// value if one was specified for the value in the enumeration type definition otherwise
        /// the default enumeration value name will be returned.
        /// </summary>
        /// <param name="value">The enumeration value to be converted.</param>
        /// <returns>The member or default string value.</returns>
        /// <remarks>
        /// This is useful because <see cref="M:System.Enum.ToString" /> ignores any <see cref="T:System.Runtime.Serialization.EnumMemberAttribute" />
        /// attributes.
        /// </remarks>
        public static string ToMemberString(this Enum value)
        {
            var customAttributes = (EnumMemberAttribute[]) value
                .GetType()
                .GetField(value.ToString())!
                .GetCustomAttributes(typeof (EnumMemberAttribute), false);
            
            return (customAttributes.Length != 0 
                ? customAttributes.First().Value 
                : value.ToString()) 
                   ?? string.Empty;
        }
    }
}