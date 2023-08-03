using B1Task1.Models;

namespace B1Task1.Services;

public static class TableService
{
    private static readonly ApplicationDbContext DbContext = new();

    public static int Add(Table table)
    {
        DbContext.Add(table);
        return Save();
    }

    private static int Save()
    {
        var saved = DbContext.SaveChanges();
        return saved;
    }
}