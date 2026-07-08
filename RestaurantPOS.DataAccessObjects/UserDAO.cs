using Microsoft.EntityFrameworkCore;
using RestaurantPOS.BusinessObjects;

namespace RestaurantPOS.DataAccessObjects;

public class UserDAO
{
    public static User? GetUserByUsername(string username)
    {
        using var context = new AppDbContext();
        return context.Users.FirstOrDefault(u => u.Username == username);
    }

    public static List<User> GetUsers()
    {
        using var context = new AppDbContext();
        return context.Users.ToList();
    }

    public static bool SaveUser(User user)
    {
        using var context = new AppDbContext();
        try
        {
            context.Users.Add(user);
            context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) as needed
            return false;
        }
    }
}
