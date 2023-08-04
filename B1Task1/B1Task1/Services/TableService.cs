using B1Task1.Models;
using Microsoft.EntityFrameworkCore;

namespace B1Task1.Services;

/// <summary>
/// Table service.
/// </summary>
public static class TableService
{
    private static readonly ApplicationDbContext DbContext = new();
    /// <summary>
    /// Executes raw sql query
    /// </summary>
    /// <param name="query">Query to execute</param>
    /// <returns>Rows affected</returns>
    public static int ExecuteQuery(string query)
    {
        return DbContext.Database.ExecuteSqlRaw(query);
    }
}