using System;
using System.IO;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace EasyTalker.Database;

public static class DatabaseManager
{
    private const string CreateDatabaseQueryFileName = "createDatabaseQuery.sql";
    private const string Database = "Database";
    private const string DatabaseNameTag = "DATABASE_NAME";
    
    public static void EnsureCreated(IConfiguration configuration)
    {
        try
        {
            Log.Information("DatabaseManager | Checking database");
            
            var connectionStringBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString(Database));
            var databaseName = connectionStringBuilder.InitialCatalog;
            connectionStringBuilder.InitialCatalog = string.Empty;
            
            using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

            if (CheckDatabaseExists(dbConnection, databaseName))
            {
                Log.Information("DatabaseManager | Database exist");
                return;
            }
            
            Log.Information("DatabaseManager | Creating database");
            var createQuery = File.ReadAllText(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, 
                CreateDatabaseQueryFileName));

            var refilledCreateQuery = createQuery.Replace(DatabaseNameTag, databaseName);
            
            dbConnection.Execute($"CREATE DATABASE [{databaseName}]");
            dbConnection.Execute(refilledCreateQuery);

            Log.Information("DatabaseManager | Database created");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "DatabaseManager | Error");
        }
    }
    
    private static bool CheckDatabaseExists(SqlConnection connection, string database)
    {
        try
        {
            using var command = new SqlCommand($"SELECT db_id('{database}')", connection);
            connection.Open();
            return command.ExecuteScalar() != DBNull.Value;
        }
        catch (Exception)
        {
            return false;
        }
    }
}