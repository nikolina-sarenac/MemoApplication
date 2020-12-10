using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Models
{
    public class MemoViewModel
    {
        public long Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Title { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int StatusId { get; set; }

        public string Status { get; set; }
        public string CreatedAtStr { get; set; }
        public string UpdatedAtStr { get; set; }
        public string Tags { get; set; }
    }
}
