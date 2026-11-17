namespace KT15_BlazorNavigation.Services;

public class LocalizationService
{
    public string CurrentLanguage { get; private set; } = "ru";

    public event Action? LanguageChanged;

    private readonly Dictionary<string, Dictionary<string, string>> _translations =
        new()
        {
            ["ru"] = new()
            {
                ["productCategory"] = "Категория",
                ["electronics"] = "Электроника",
                ["serviceOneDescription"] = "Создание и поддержка веб-приложений.",
                ["serviceTwoDescription"] = "Помощь с настройкой и обслуживанием систем.",
                ["appName"] = "Учебное приложение Blazor",
                ["home"] = "Главная страница",
                ["clicker"] = "Кликер студента",
                ["details"] = "Детали",
                ["products"] = "Товары",
                ["services"] = "Услуги",
                ["notFound"] = "Страница не найдена",
                ["welcome"] = "Добро пожаловать!",
                ["studentInfo"] = "Информация о студенте",
                ["age"] = "Возраст",
                ["increase"] = "Увеличить возраст",
                ["reset"] = "Сбросить возраст",
                ["route"] = "Текущий маршрут",
                ["navigation"] = "Выполнен переход",
                ["goDetails"] = "Открыть детали",
                ["goProducts"] = "Перейти к товарам",
                ["goServices"] = "Перейти к услугам",
                ["goHome"] = "На главную",
                ["category"] = "Категория",
                ["allProducts"] = "Все товары",
                ["productsText"] = "Каталог товаров",
                ["servicesText"] = "Наши услуги",
                ["dynamicContent"] = "Динамический контент",
                ["productOne"] = "Ноутбук",
                ["productTwo"] = "Смартфон",
                ["serviceOne"] = "Разработка сайтов",
                ["serviceTwo"] = "Техническая поддержка",
                ["wrongUrl"] = "Проверьте адрес страницы или вернитесь на главную.",
                ["language"] = "Язык"
            },

            ["en"] = new()
            {
                ["productCategory"] = "Category",
                ["electronics"] = "Electronics",
                ["serviceOneDescription"] = "Website development and maintenance.",
                ["serviceTwoDescription"] = "System setup and maintenance support.",
                ["appName"] = "Blazor Learning Application",
                ["home"] = "Home page",
                ["clicker"] = "Student clicker",
                ["details"] = "Details",
                ["products"] = "Products",
                ["services"] = "Services",
                ["notFound"] = "Page not found",
                ["welcome"] = "Welcome!",
                ["studentInfo"] = "Student information",
                ["age"] = "Age",
                ["increase"] = "Increase age",
                ["reset"] = "Reset age",
                ["route"] = "Current route",
                ["navigation"] = "Navigation completed",
                ["goDetails"] = "Open details",
                ["goProducts"] = "Go to products",
                ["goServices"] = "Go to services",
                ["goHome"] = "Home",
                ["category"] = "Category",
                ["allProducts"] = "All products",
                ["productsText"] = "Product catalog",
                ["servicesText"] = "Our services",
                ["dynamicContent"] = "Dynamic content",
                ["productOne"] = "Laptop",
                ["productTwo"] = "Smartphone",
                ["serviceOne"] = "Website development",
                ["serviceTwo"] = "Technical support",
                ["wrongUrl"] = "Check the page address or return to the home page.",
                ["language"] = "Language"
            }
        };

    public string Text(string key)
    {
        return _translations[CurrentLanguage].TryGetValue(key, out var value)
            ? value
            : key;
    }

    public void SetLanguage(string language)
    {
        if (!_translations.ContainsKey(language) ||
            CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;
        LanguageChanged?.Invoke();
    }
}