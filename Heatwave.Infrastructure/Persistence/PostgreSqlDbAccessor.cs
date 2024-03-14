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
}
