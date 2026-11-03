using System;

Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №5: КЕРУВАННЯ ДОСТУПОМ (RBAC + ABAC) ===\n");

// Створюємо користувачів (RBAC)
User admin = new User { Id = 1, Username = "admin_bohdan", UserRole = Role.Admin };
User realtor1 = new User { Id = 2, Username = "realtor_ivan", UserRole = Role.Realtor };
User realtor2 = new User { Id = 3, Username = "realtor_petro", UserRole = Role.Realtor };

// Створюємо об'єкт нерухомості (Власник — realtor1 з ID = 2)
PropertyObject apartment = new PropertyObject { Id = 101, Title = "Квартира в центрі", OwnerId = 2 };

// Сценарій 1: Рієлтор 1 редагує СВІЙ об'єкт (Дозволено за ABAC)
SecurityService.ExecuteEdit(realtor1, apartment, "Розкішна квартира в центрі");

// Сценарій 2: Рієлтор 2 намагається відредагувати ЧУЖИЙ об'єкт (Заборонено за ABAC)
SecurityService.ExecuteEdit(realtor2, apartment, "Хаканута квартира");

// Сценарій 3: Адміністратор редагує БУДЬ-ЯКИЙ об'єкт (Дозволено за RBAC)
SecurityService.ExecuteEdit(admin, apartment, "Квартира (Перевірено Адміном)");

// --- ОГОЛОШЕННЯ ТИПІВ ТА КЛАСІВ (повинні бути внизу файлу) ---
public enum Role { Admin, Realtor }

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public Role UserRole { get; set; }
}

public class PropertyObject
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int OwnerId { get; set; }
}

public class SecurityService
{
    public static bool CanEditObject(User user, PropertyObject obj)
    {
        if (user.UserRole == Role.Admin) return true;
        if (user.UserRole == Role.Realtor && obj.OwnerId == user.Id) return true;
        return false;
    }

    public static void ExecuteEdit(User user, PropertyObject obj, string newTitle)
    {
        Console.WriteLine($"[Спроба операції]: Користувач '{user.Username}' ({user.UserRole}) намагається змінити об'єкт #{obj.Id} (Власник ID: {obj.OwnerId})");
        
        if (!CanEditObject(user, obj))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ВІДМОВА У ДОСТУПІ 403]: Користувачу '{user.Username}' заборонено редагувати цей об'єкт!\n");
            Console.ResetColor();
            return;
        }

        obj.Title = newTitle;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[УСПІХ 200]: Об'єкт успішно відредаговано! Нова назва: '{obj.Title}'\n");
        Console.ResetColor();
    }
}