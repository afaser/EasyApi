using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afaser.EasyApi
{
    public class Cookie
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? Expire { get; set; }
        public bool IsHttpOnly { get; set; }
        public bool IsSecure { get; set; }
    }
}
