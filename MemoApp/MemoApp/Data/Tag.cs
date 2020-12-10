using System;
using System.Collections.Generic;

namespace MemoApp.Data
{
    public partial class Tag
    {
        public long Id { get; set; }
        public long MemoId { get; set; }
        public string Name { get; set; }

        public virtual Memo Memo { get; set; }
    }
}
