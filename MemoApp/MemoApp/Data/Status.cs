using System;
using System.Collections.Generic;

namespace MemoApp.Data
{
    public partial class Status
    {
        public Status()
        {
            Memo = new HashSet<Memo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Memo> Memo { get; set; }
    }
}
