using B1Task1.Models;
using Microsoft.EntityFrameworkCore;

namespace B1Task1.Services;

public static class TableService
{
    private static readonly ApplicationDbContext DbContext = new();

    public static void Add(Table table)
    {
        DbContext.Add(table);
    }

    public static void AddRange(IEnumerable<Table> tables)
    {
        DbContext.AddRange(tables);
    }

    public static int Save()
    {
        var saved = DbContext.SaveChanges();
        return saved;
    }

    public static int ExecuteQuery(string query)
    {
        return DbContext.Database.ExecuteSqlRaw(query);
    }
}