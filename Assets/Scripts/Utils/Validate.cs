using System.Collections;

namespace RT
{
    public class Validate
    {
        public static bool IsEmpty(string text)
        {
            return string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text.Trim());
        }

        public static bool IsNotEmpty(string text)
        {
            return !IsEmpty(text);
        }

        public static bool IsEmpty(ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNotEmpty(ICollection collection)
        {
            return collection != null && collection.Count > 0;
        }
    }
}
