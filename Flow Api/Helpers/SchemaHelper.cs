using System.Text.RegularExpressions;

namespace Flow_Api.Helpers
{
    public static class SchemaHelper
    {
        public static string GenerateSchemaName(string shopName, Guid tenantId)
        {
            // Sanitize shop name: lowercase, remove special chars, replace spaces with underscore
            var sanitized = Regex.Replace(shopName.ToLower(), @"[^a-z0-9]", "");

            // Take first 20 characters
            sanitized = sanitized.Length > 20 ? sanitized.Substring(0, 20) : sanitized;

            // Append unique identifier (first 8 characters of GUID)
            var uniqueId = tenantId.ToString("N").Substring(0, 8);

            return $"tenant_{sanitized}_{uniqueId}";
        }
    }
}
