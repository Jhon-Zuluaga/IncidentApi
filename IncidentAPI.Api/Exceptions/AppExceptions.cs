
// <summary>
/// Base exception for business errors whose message is safe to show to the customer.
/// </summary>

public abstract class AppException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

// <summary> Resource not found (404). </summary>

public class NotFoundException(string message)
    : AppException(message, StatusCodes.Status404NotFound);


// <summary> Invalid request (400). </summary>

public class BadRequestException(string message)
    : AppException(message, StatusCodes.Status400BadRequest);
