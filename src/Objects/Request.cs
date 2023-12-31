﻿using System.Net;
using System.Net.Sockets;

namespace Afaser.EasyApi.Objects
{
    public class Request
    {
        public Socket Socket { get; internal set; }
        public string Method { get; internal set; }
        public string Endpoint { get; internal set; }
        public IReadOnlyDictionary<string, string> Queue { get; internal set; }
        public IReadOnlyDictionary<string, string> Headers { get; internal set; }
        public IReadOnlyDictionary<string, Cookie> Cookies { get; internal set; }
    }
}
