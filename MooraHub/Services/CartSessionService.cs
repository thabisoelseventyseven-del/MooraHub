using System.Text.Json;
using MooraHub.Models;

namespace MooraHub.Services;

public class CartSessionService
{
    private const string Key = "MOORAHUB_CART";

    public List<ServiceItem> GetCart(HttpContext http)
    {
        var json = http.Session.GetString(Key);
        return string.IsNullOrWhiteSpace(json)
            ? new List<ServiceItem>()
            : JsonSerializer.Deserialize<List<ServiceItem>>(json) ?? new List<ServiceItem>();
    }

    public void SaveCart(HttpContext http, List<ServiceItem> cart)
    {
        http.Session.SetString(Key, JsonSerializer.Serialize(cart));
    }

    public void Add(HttpContext http, int serviceId)
    {
        var item = ServiceCatalog.Get(serviceId);
        if (item is null) return;

        var cart = GetCart(http);
        cart.Add(item);
        SaveCart(http, cart);
    }

    public void RemoveAt(HttpContext http, int index)
    {
        var cart = GetCart(http);
        if (index < 0 || index >= cart.Count) return;

        cart.RemoveAt(index);
        SaveCart(http, cart);
    }

    public void Clear(HttpContext http) => http.Session.Remove(Key);

    public int Total(HttpContext http) => GetCart(http).Sum(x => x.Price);

    public int Count(HttpContext http) => GetCart(http).Count;
}
