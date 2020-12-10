using System;
using System.Collections.Generic;
using System.Text;

namespace MemoApp.Common
{
    class Feedback<T> : IFeedback<T>
    {
        public T Value { get; set; }
        public StatusEnum Status { get; set; }
        public string Message { get; set; }
    }

}
