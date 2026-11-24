using Contracts.Errors;

namespace Web.Endpoints.Models
{
    public record Envelope<TValue>(TValue? Result, AppError? Error)
    {
        public DateTime GeneratedAt { get; init; } = DateTime.UtcNow;
    }

    internal static class Envelope
    {
        public static Envelope<T> Ok<T>(T result) => new(result, null);
        public static Envelope<object> Ok() => new(null, null);
        public static Envelope<object> Error(AppError error) => new(null, error);
    }
}