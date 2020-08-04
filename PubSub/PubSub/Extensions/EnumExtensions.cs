using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PubSub.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns specified attributes of the Enum value
        /// </summary>
        /// <typeparam name="T">Type of attributes to return</typeparam>
        /// <param name="enumVal">Enum value to get the attribute for</param>
        /// <param name="inherit">Search member's inheritance chain to find attributes</param>
        /// <returns>Attribute list</returns>
        public static IEnumerable<T> GetAttributesOfType<T>(this Enum enumVal, bool inherit = false) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo.Length == 0) return null;

            var attributes = memInfo[0].GetCustomAttributes(typeof(T), inherit);
            return attributes.Cast<T>();
        }
        
        /// <summary>
        /// Returns the attribute of the Enum value
        /// </summary>
        /// <typeparam name="T">Type of attribute to return</typeparam>
        /// <param name="enumVal">Enum value to get the attribute for</param>
        /// <param name="inherit">Search member's inheritance chain to find attribute</param>
        /// <returns>Attribute or null if not found</returns>
        public static T GetAttributeOfType<T>(this Enum enumVal, bool inherit = false) where T : Attribute
            => GetAttributesOfType<T>(enumVal, inherit).FirstOrDefault();
        
        /// <summary>
        /// Tries to get the description value of the <see ref="Description"/> attribute. Returns true, if successful and the out string being filled or false, if unsuccessful.
        /// </summary>
        /// <param name="enumVal">Enum value to get the description attribute's value for</param>
        /// <param name="value">Output value of description attribute</param>
        /// <returns>True if description found, false if not found</returns>
        public static bool TryGetDescription(this Enum enumVal, out string value)
        {
            var type = enumVal.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumVal));
            }

            var attr = GetAttributeOfType<DescriptionAttribute>(enumVal);
            value = attr?.Description;
            return attr != null;
        }
    }
}