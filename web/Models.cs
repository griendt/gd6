using engine.Models;
using Microsoft.EntityFrameworkCore;

namespace web;

public class Gd6DbContext : DbContext
{
    public DbSet<Territory> Territories { get; set; }

    private static string DbPath => "/home/alex/projects/gd6/gd6.db";
    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
}