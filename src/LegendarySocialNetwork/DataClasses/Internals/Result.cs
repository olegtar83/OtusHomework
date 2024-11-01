namespace LegendarySocialNetwork.DataClasses.Internals
{
    public class Result<T>
    {
        internal Result(bool succeeded, IEnumerable<string> errors, T value)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
            Value = value;
        }

        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; set; } = default!;
        public T Value { get; set; } = default!;

        public string[] Errors { get; set; }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, Array.Empty<string>(), value);
        }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, errors);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, new string[] { error });
        }
    }
}
