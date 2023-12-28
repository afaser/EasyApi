using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Afaser.EasyApi
{
    public struct Cookie
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? Expires { get; set; } = null;
        public int? MaxAge { get; set; } = null;
        public string? Domain { get; set; } = null;
        public string? Path { get; set; } = null;
        public bool? IsHttpOnly { get; set; } = null;
        public bool? IsSecure { get; set; } = null;
        public SameSite? SameSite { get; set; } = null;

        public Cookie()
        {
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Key))
                throw new ArgumentNullException(nameof(Key));
            if (string.IsNullOrWhiteSpace(Value))
                throw new ArgumentNullException(nameof(Value));

            var _sb = new StringBuilder();

            _sb.Append($"{Key}={Value}");
            

            if (Expires != null)
            {
                if (Expires.Value < DateTime.UtcNow)
                    throw new ArgumentOutOfRangeException(nameof(Expires));

                var _format = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";
                var _httpTime = Expires.Value.ToString(_format, CultureInfo.InvariantCulture);
                _sb.Append($"; Expires={_httpTime}");
            }

            if (MaxAge != null)
            {
                if (MaxAge <= 0)
                    throw new ArgumentOutOfRangeException(nameof(MaxAge));

                _sb.Append($"; Max -Age={MaxAge}");
            }

            if (!string.IsNullOrWhiteSpace(Domain))
                _sb.Append($"; Domain={Domain}");

            if (!string.IsNullOrWhiteSpace(Path))
                _sb.Append($"; Path={Path}");

            if (IsSecure == true) _sb.Append("; Secure");
                
            if (IsHttpOnly == true) _sb.Append("; HttpOnly");

            if(SameSite.HasValue) _sb.Append($"; {SameSite.Value}");

            return _sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Cookie _cookie)
            {
                if(_cookie.Key != Key) return false;
                if(_cookie.Value != Value) return false;
                if(_cookie.Expires != Expires) return false;
                if(_cookie.MaxAge != MaxAge) return false;
                if(_cookie.Domain != Domain) return false;
                if(_cookie.Path != Path) return false;
                if(_cookie.IsHttpOnly != IsHttpOnly) return false;
                if(_cookie.IsSecure != IsSecure) return false;
                if(_cookie.SameSite != SameSite) return false;

                return true;
            }
            else return false;
        }
        public override int GetHashCode()
        {
            return Key.GetHashCode()
                ^ Value.GetHashCode()
                ^ Expires.GetHashCode()
                ^ MaxAge.GetHashCode()
                ^ (Domain ?? "").GetHashCode()
                ^ (Path ?? "").GetHashCode()
                ^ IsHttpOnly.GetHashCode()
                ^ IsSecure.GetHashCode()
                ^ SameSite.GetHashCode();
        }
    }
    public enum SameSite : byte
    {
        None = 0, 
        Strict = 1, 
        Lax = 2
    }
}
