using System.Text.RegularExpressions;

namespace RazorPagesDotCMS.Models
{
    public class ProxyConfig
    {
        public bool Enabled { get; set; }
        public string Path { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;

        public bool IsMatch(string path)
        {
            if (!Enabled)
                return false;

            // Convert the path pattern to a regex pattern
            // Replace * with .* for wildcard matching
            string pattern = "^" + Regex.Escape(Path).Replace("\\*", ".*") + "$";
            return Regex.IsMatch(path, pattern, RegexOptions.IgnoreCase);
        }
    }

    public class ProxySettings
    {
        public List<ProxyConfig> Proxy { get; set; } = new List<ProxyConfig>();
    }
}
