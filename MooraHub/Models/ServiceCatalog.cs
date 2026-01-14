namespace MooraHub.Models;

public static class ServiceCatalog
{
    public static readonly List<ServiceItem> Items = new()
    {
        new(1, "💼", "Business Templates", 250),
        new(2, "🎨", "Portfolio Templates", 200),
        new(3, "🌐", "Landing Pages", 300),
        new(4, "📣", "Marketing Materials", 180),
        new(5, "📝", "CV Creation", 100),
        new(6, "🎓", "University Applications", 150),
        new(7, "📊", "Report Templates", 120),
        new(8, "💻", "Web Templates", 350),
    };

    public static ServiceItem? Get(int id) => Items.FirstOrDefault(x => x.Id == id);
}
