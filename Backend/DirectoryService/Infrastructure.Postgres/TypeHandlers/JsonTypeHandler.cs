using Dapper;
using System.Data;
using System.Text.Json;

namespace Infrastructure.Postgres.TypeHandlers
{
    /// <summary>
    /// Маппит jsonb-колонку в объект типа <typeparamref name="T"/> и обратно.
    /// </summary>
    internal sealed class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        };

        public override T? Parse(object value)
        {
            return value is string json
                ? JsonSerializer.Deserialize<T>(json, _options)
                : default;
        }

        public override void SetValue(IDbDataParameter parameter, T? value)
        {
            parameter.Value = value is null
                ? DBNull.Value
                : JsonSerializer.Serialize(value, _options);
        }
    }
}
