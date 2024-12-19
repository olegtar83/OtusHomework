using System.Globalization;

namespace LegendarySocialNetwork.WebApi.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException() : base() { }

    public ForbiddenException(string message) : base(message) { }

    public ForbiddenException(string message, params object[] args)
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}
