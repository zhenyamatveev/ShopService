using ShopService.Domain.Entities;
using ShopService.ValueObjects;

var customers = new Dictionary<int, Customer>();
var sellers = new Dictionary<int, Seller>();
var products = new Dictionary<int, Product>();
var promotions = new Dictionary<int, Promotion>();

var nextCustomerId = 1;
var nextSellerId = 1;
var nextProductId = 1;
var nextPromotionId = 1;

while (true)
{
    Console.Clear();
    Console.WriteLine("=== ShopService DomainApp ===");
    Console.WriteLine("1. Режим покупателя");
    Console.WriteLine("2. Режим продавца");
    Console.WriteLine("0. Выход");
    Console.Write("Выбор: ");
    var choice = Console.ReadLine();

    if (choice == "0") return;
    if (choice == "1") CustomerMode();
    if (choice == "2") SellerMode();
}

void CustomerMode()
{
    var customerId = EnsureCustomer();
    var customer = customers[customerId];

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"=== Покупатель #{customerId} ({customer.Name.Value}) ===");
        Console.WriteLine("1. Просмотр каталога товаров");
        Console.WriteLine("2. Поиск товаров по названию");
        Console.WriteLine("3. Просмотр акций и скидок");
        Console.WriteLine("4. Добавить товар в избранное");
        Console.WriteLine("5. Удалить товар из избранного");
        Console.WriteLine("6. Показать избранное");
        Console.WriteLine("0. Назад");
        Console.Write("Выбор: ");

        var choice = Console.ReadLine();
        if (choice == "0") return;

        if (choice == "1")
        {
            PrintCatalog();
            Pause();
        }
        else if (choice == "2")
        {
            Console.Write("Строка поиска: ");
            var q = (Console.ReadLine() ?? string.Empty).Trim();
            var matches = products
                .Where(x => x.Value.Name.Value.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine();
            if (matches.Count == 0) Console.WriteLine("Ничего не найдено.");
            else
            {
                foreach (var (id, p) in matches)
                    Console.WriteLine($"- #{id}: {p.Name.Value} | {p.Price.Value} | seller={GetSellerIdForProduct(p)}");
            }

            Pause();
        }
        else if (choice == "3")
        {
            PrintPromotions();
            Pause();
        }
        else if (choice == "4")
        {
            PrintCatalog();
            var pid = ReadInt("ID товара: ");
            if (!products.TryGetValue(pid, out var product))
            {
                Console.WriteLine("Товар не найден.");
                Pause();
                continue;
            }

            try
            {
                customer.AddToFavorites(product);
                Console.WriteLine("Добавлено в избранное.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Pause();
        }
        else if (choice == "5")
        {
            PrintFavorites(customerId);
            var pid = ReadInt("ID товара: ");
            if (!products.TryGetValue(pid, out var product))
            {
                Console.WriteLine("Товар не найден.");
                Pause();
                continue;
            }

            try
            {
                customer.RemoveFromFavorites(product);
                Console.WriteLine("Удалено из избранного.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Pause();
        }
        else if (choice == "6")
        {
            PrintFavorites(customerId);
            Pause();
        }
    }
}

void SellerMode()
{
    var sellerId = EnsureSeller();
    var seller = sellers[sellerId];

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"=== Продавец #{sellerId} ({seller.Name.Value}) ===");
        Console.WriteLine("1. Добавить товар");
        Console.WriteLine("2. Редактировать товар");
        Console.WriteLine("3. Удалить товар (из каталога DomainApp)");
        Console.WriteLine("4. Создать акцию");
        Console.WriteLine("5. Изменить акцию");
        Console.WriteLine("6. Завершить акцию");
        Console.WriteLine("7. Привязать товар к акции");
        Console.WriteLine("8. Показать мои товары");
        Console.WriteLine("9. Показать мои акции");
        Console.WriteLine("0. Назад");
        Console.Write("Выбор: ");

        var choice = Console.ReadLine();
        if (choice == "0") return;

        if (choice == "1")
        {
            Console.Write("Название товара: ");
            var name = new Name(Console.ReadLine() ?? string.Empty);
            Console.Write("Описание (можно пусто): ");
            var desc = Console.ReadLine();
            var price = ReadDecimal("Цена: ");

            var product = seller.CreateProduct(name, desc, new Price(price));
            products[nextProductId] = product;
            Console.WriteLine($"Товар добавлен. ID (DomainApp) = {nextProductId}");
            nextProductId++;
            Pause();
        }
        else if (choice == "2")
        {
            PrintSellerProducts(sellerId);
            var pid = ReadInt("ID товара: ");
            if (!products.TryGetValue(pid, out var product) || GetSellerIdForProduct(product) != sellerId)
            {
                Console.WriteLine("Товар не найден у этого продавца.");
                Pause();
                continue;
            }

            Console.Write("Новое название: ");
            var name = new Name(Console.ReadLine() ?? string.Empty);
            Console.Write("Новое описание (можно пусто): ");
            var desc = Console.ReadLine();
            var price = ReadDecimal("Новая цена: ");
            product.Edit(name, desc, new Price(price));
            Console.WriteLine("Товар обновлен.");
            Pause();
        }
        else if (choice == "3")
        {
            PrintSellerProducts(sellerId);
            var pid = ReadInt("ID товара: ");
            if (!products.TryGetValue(pid, out var product) || GetSellerIdForProduct(product) != sellerId)
            {
                Console.WriteLine("Товар не найден у этого продавца.");
                Pause();
                continue;
            }

            products.Remove(pid);
            Console.WriteLine("Товар удален из каталога DomainApp.");
            Pause();
        }
        else if (choice == "4")
        {
            Console.Write("Заголовок акции: ");
            var title = new Title(Console.ReadLine() ?? string.Empty);
            Console.Write("Описание (можно пусто): ");
            var desc = Console.ReadLine();

            Console.Write("Скидка % (Enter чтобы без скидки): ");
            var discountText = (Console.ReadLine() ?? string.Empty).Trim();
            Discount? discount = string.IsNullOrEmpty(discountText) ? null : new Discount(decimal.Parse(discountText));

            var start = ReadOptionalUtc("Дата начала (UTC, Enter чтобы пусто): ");
            var end = ReadOptionalUtc("Дата конца (UTC, Enter чтобы пусто): ");

            var promotion = seller.CreatePromotion(title, desc, discount, start, end);
            promotions[nextPromotionId] = promotion;
            Console.WriteLine($"Акция создана. ID (DomainApp) = {nextPromotionId}");
            nextPromotionId++;
            Pause();
        }
        else if (choice == "5")
        {
            PrintSellerPromotions(sellerId);
            var promoId = ReadInt("ID акции: ");
            if (!promotions.TryGetValue(promoId, out var promo) || GetSellerIdForPromotion(promo) != sellerId)
            {
                Console.WriteLine("Акция не найдена у этого продавца.");
                Pause();
                continue;
            }

            Console.Write("Новый заголовок: ");
            var title = new Title(Console.ReadLine() ?? string.Empty);
            Console.Write("Новое описание (можно пусто): ");
            var desc = Console.ReadLine();

            Console.Write("Скидка % (Enter чтобы без скидки): ");
            var discountText = (Console.ReadLine() ?? string.Empty).Trim();
            Discount? discount = string.IsNullOrEmpty(discountText) ? null : new Discount(decimal.Parse(discountText));

            var start = ReadOptionalUtc("Дата начала (UTC, Enter чтобы пусто): ");
            var end = ReadOptionalUtc("Дата конца (UTC, Enter чтобы пусто): ");

            promo.Edit(title, desc, discount, start, end);
            Console.WriteLine("Акция обновлена.");
            Pause();
        }
        else if (choice == "6")
        {
            PrintSellerPromotions(sellerId);
            var promoId = ReadInt("ID акции: ");
            if (!promotions.TryGetValue(promoId, out var promo) || GetSellerIdForPromotion(promo) != sellerId)
            {
                Console.WriteLine("Акция не найдена у этого продавца.");
                Pause();
                continue;
            }

            var end = ReadUtc("Дата завершения (UTC): ");
            promo.End(end);
            Console.WriteLine("Акция завершена.");
            Pause();
        }
        else if (choice == "7")
        {
            PrintSellerProducts(sellerId);
            var pid = ReadInt("ID товара: ");
            if (!products.TryGetValue(pid, out var product) || GetSellerIdForProduct(product) != sellerId)
            {
                Console.WriteLine("Товар не найден у этого продавца.");
                Pause();
                continue;
            }

            PrintSellerPromotions(sellerId);
            var promoId = ReadInt("ID акции: ");
            if (!promotions.TryGetValue(promoId, out var promo) || GetSellerIdForPromotion(promo) != sellerId)
            {
                Console.WriteLine("Акция не найдена у этого продавца.");
                Pause();
                continue;
            }

            try
            {
                seller.AddProductToPromotion(product, promo);
                Console.WriteLine("Товар привязан к акции.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Pause();
        }
        else if (choice == "8")
        {
            PrintSellerProducts(sellerId);
            Pause();
        }
        else if (choice == "9")
        {
            PrintSellerPromotions(sellerId);
            Pause();
        }
    }
}

int EnsureCustomer()
{
    if (customers.Count == 0)
    {
        Console.Write("Создай покупателя. Имя: ");
        customers[nextCustomerId] = new Customer(new Name(Console.ReadLine() ?? string.Empty));
        nextCustomerId++;
    }

    Console.WriteLine();
    Console.WriteLine("Покупатели:");
    foreach (var (id, c) in customers)
        Console.WriteLine($"- #{id}: {c.Name.Value}");

    var selected = ReadInt("Выбери ID покупателя: ");
    if (!customers.ContainsKey(selected))
    {
        Console.Write("Имя нового покупателя: ");
        customers[nextCustomerId] = new Customer(new Name(Console.ReadLine() ?? string.Empty));
        selected = nextCustomerId;
        nextCustomerId++;
    }

    return selected;
}

int EnsureSeller()
{
    if (sellers.Count == 0)
    {
        Console.Write("Создай продавца. Имя: ");
        sellers[nextSellerId] = new Seller(new Name(Console.ReadLine() ?? string.Empty));
        nextSellerId++;
    }

    Console.WriteLine();
    Console.WriteLine("Продавцы:");
    foreach (var (id, s) in sellers)
        Console.WriteLine($"- #{id}: {s.Name.Value}");

    var selected = ReadInt("Выбери ID продавца: ");
    if (!sellers.ContainsKey(selected))
    {
        Console.Write("Имя нового продавца: ");
        sellers[nextSellerId] = new Seller(new Name(Console.ReadLine() ?? string.Empty));
        selected = nextSellerId;
        nextSellerId++;
    }

    return selected;
}

int GetSellerIdForProduct(Product product)
    => sellers.First(x => ReferenceEquals(x.Value, product.Seller)).Key;

int GetSellerIdForPromotion(Promotion promotion)
    => sellers.First(x => ReferenceEquals(x.Value, promotion.Seller)).Key;

void PrintCatalog()
{
    Console.WriteLine();
    if (products.Count == 0)
    {
        Console.WriteLine("Каталог пуст.");
        return;
    }

    Console.WriteLine("Каталог:");
    foreach (var (id, p) in products)
        Console.WriteLine($"- #{id}: {p.Name.Value} | {p.Price.Value} | seller={GetSellerIdForProduct(p)}");
}

void PrintPromotions()
{
    Console.WriteLine();
    if (promotions.Count == 0)
    {
        Console.WriteLine("Акций нет.");
        return;
    }

    Console.WriteLine("Акции:");
    foreach (var (id, promo) in promotions)
    {
        var discount = promo.Discount?.Value.ToString() ?? "нет";
        Console.WriteLine($"- #{id}: {promo.Title.Value} | скидка={discount}% | seller={GetSellerIdForPromotion(promo)}");
    }
}

void PrintSellerProducts(int sellerId)
{
    Console.WriteLine();
    Console.WriteLine("Мои товары:");
    var list = products.Where(x => GetSellerIdForProduct(x.Value) == sellerId).ToList();
    if (list.Count == 0)
    {
        Console.WriteLine("Нет товаров.");
        return;
    }
    foreach (var (id, p) in list)
        Console.WriteLine($"- #{id}: {p.Name.Value} | {p.Price.Value}");
}

void PrintSellerPromotions(int sellerId)
{
    Console.WriteLine();
    Console.WriteLine("Мои акции:");
    var list = promotions.Where(x => GetSellerIdForPromotion(x.Value) == sellerId).ToList();
    if (list.Count == 0)
    {
        Console.WriteLine("Нет акций.");
        return;
    }
    foreach (var (id, p) in list)
    {
        var discount = p.Discount?.Value.ToString() ?? "нет";
        Console.WriteLine($"- #{id}: {p.Title.Value} | скидка={discount}%");
    }
}

void PrintFavorites(int customerId)
{
    Console.WriteLine();
    Console.WriteLine("Избранное:");
    var customer = customers[customerId];
    var favProductRefs = customer.Favorites.Select(x => x.Product).ToHashSet();
    var list = products.Where(x => favProductRefs.Contains(x.Value)).ToList();
    if (list.Count == 0)
    {
        Console.WriteLine("Избранное пусто.");
        return;
    }
    foreach (var (id, p) in list)
        Console.WriteLine($"- #{id}: {p.Name.Value} | {p.Price.Value}");
}

int ReadInt(string label)
{
    while (true)
    {
        Console.Write(label);
        var text = Console.ReadLine();
        if (int.TryParse(text, out var value) && value > 0) return value;
        Console.WriteLine("Некорректное число.");
    }
}

decimal ReadDecimal(string label)
{
    while (true)
    {
        Console.Write(label);
        var text = Console.ReadLine();
        if (decimal.TryParse(text, out var value)) return value;
        Console.WriteLine("Некорректное число.");
    }
}

DateTime ReadUtc(string label)
{
    while (true)
    {
        Console.Write(label);
        var text = (Console.ReadLine() ?? string.Empty).Trim();
        if (DateTime.TryParse(text, out var dt))
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        Console.WriteLine("Некорректная дата.");
    }
}

DateTime? ReadOptionalUtc(string label)
{
    Console.Write(label);
    var text = (Console.ReadLine() ?? string.Empty).Trim();
    if (string.IsNullOrEmpty(text)) return null;
    if (DateTime.TryParse(text, out var dt))
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    Console.WriteLine("Некорректная дата. Значение будет пустым.");
    return null;
}

void Pause()
{
    Console.WriteLine();
    Console.WriteLine("Нажми Enter чтобы продолжить...");
    Console.ReadLine();
}
