using System;
using System.Collections.Generic;

namespace MemoApp.Data
{
    public partial class Memo
    {
        public Memo()
        {
            Tag = new HashSet<Tag>();
        }

        public long Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int StatusId { get; set; }

        public virtual AspNetUsers AspNetUser { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<Tag> Tag { get; set; }
    }
}
