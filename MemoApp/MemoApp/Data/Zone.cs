using System;
using System.Collections.Generic;

namespace MemoApp.Data
{
    public partial class Zone
    {
        public long Id { get; set; }
        public string AspNetUserId { get; set; }
        public string ZoneName { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string Culture { get; set; }

        public virtual AspNetUsers AspNetUser { get; set; }
    }
}
