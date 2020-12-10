namespace MemoApp.Common
{
    public class Result<T> : IResult<T>
    {
        public T Value { get; set; }
        public bool Succeeded { get; set; }
    }

    public class NoValue
    {

    }
}
