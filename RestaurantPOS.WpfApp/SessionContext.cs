using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.WpfApp;

// Holds the logged-in user for the lifetime of the app session.
public static class SessionContext
{
    public static User? CurrentUser { get; set; }
}
