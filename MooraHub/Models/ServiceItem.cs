namespace MooraHub.Models;

// Service model used everywhere (Dashboard, Cart, Checkout)
public record ServiceItem(int Id, string Icon, string Name, int Price);
