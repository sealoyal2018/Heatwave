using System.Data.Common;

using Heatwave.Domain;

using Npgsql;

namespace Heatwave.Infrastructure.Persistence;
internal class PostgreSqlDbAccessor : DbAccessorBase, IDbAccessor
{
    public PostgreSqlDbAccessor(AppDbContext baseDbContext)
            : base(baseDbContext)
    {
    }

    public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;

    protected override string FormatFieldName(string name)
    {
        return $"\"{name}\"";
    }

    protected override string GetSchema(string schema)
    {
        if (string.IsNullOrEmpty(schema))
            return "public";
        else
            return schema;
    }
}
