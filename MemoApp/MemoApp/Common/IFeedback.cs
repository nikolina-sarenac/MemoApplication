using System;
using System.Collections.Generic;
using System.Text;

namespace MemoApp.Common
{
    public interface IFeedback<T>
    {
        T Value { get; set; }
        StatusEnum Status { get; set; }
        string Message { get; set; }
    }
}
