using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

public class AccessTokenInterceptor : DbConnectionInterceptor
{
    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        var token = Environment.GetEnvironmentVariable("SQL_ACCESS_TOKEN");

        if (!string.IsNullOrEmpty(token) && connection is SqlConnection sqlConnection)
        {
            sqlConnection.AccessToken = token;
        }

        return base.ConnectionOpening(connection, eventData, result);
    }
}
