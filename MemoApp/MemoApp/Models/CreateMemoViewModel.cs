using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemoApp.Models
{
    public class CreateMemoViewModel
    {
        public string Title { get; set; }
        public string Note { get; set; }
        public string Tags { get; set; }
        public string userId { get; set; }


    }
}
