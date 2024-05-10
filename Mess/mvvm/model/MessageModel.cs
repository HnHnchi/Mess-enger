using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Mess.mvvm.model
{
    internal class MessageModel
    {
        public String Username { get; set; }
        public String UsernameColor { get; set; }
        public String ImageSource { get; set; }
        public String Message { get; set; }
        public DateTime Time { get; set; }
        public bool IsNativeOrigin { get; set; }
        public bool FirstMessage { get; set; }
    }
}
