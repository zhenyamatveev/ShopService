using ShopService.Domain.Entities;
using ShopService.ValueObjects;

var customers = new List<Customer>();
var sellers = new List<Seller>();
var products = new List<Product>();
var promotions = new List<Promotion>();

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
    var customer = EnsureCustomer();

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"=== Покупатель ({customer.Name.Value}) ===");
        Console.WriteLine($"Id: {customer.Id}");
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
                .Where(p => p.Name.Value.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine();
            if (matches.Count == 0) Console.WriteLine("Ничего не найдено.");
            else PrintProductList(matches);
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
            if (!TryPickProduct(ReadInt("Номер товара в списке: "), out var product))
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
            PrintFavorites(customer);
            if (!TryPickProduct(ReadInt("Номер товара в списке: "), out var product))
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
            PrintFavorites(customer);
            Pause();
        }
    }
}

void SellerMode()
{
    var seller = EnsureSeller();

    while (true)
    {
        Console.Clear();
        Console.WriteLine($"=== Продавец ({seller.Name.Value}) ===");
        Console.WriteLine($"Id: {seller.Id}");
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
            var desc = ToDescription(Console.ReadLine());
            var price = ReadDecimal("Цена: ");

            var product = seller.CreateProduct(name, desc, new Price(price));
            products.Add(product);
            Console.WriteLine($"Товар добавлен. Id = {product.Id}");
            Pause();
        }
        else if (choice == "2")
        {
            var sellerProducts = products.Where(p => p.Seller == seller).ToList();
            PrintProductList(sellerProducts);
            if (!TryPickFromList(sellerProducts, ReadInt("Номер товара: "), out var product))
            {
                Console.WriteLine("Товар не найден у этого продавца.");
                Pause();
                continue;
            }

            Console.Write("Новое название: ");
            var name = new Name(Console.ReadLine() ?? string.Empty);
            Console.Write("Новое описание (можно пусто): ");
            var desc = ToDescription(Console.ReadLine());
            var price = ReadDecimal("Новая цена: ");

            try
            {
                seller.EditProduct(seller, product, name, desc, new Price(price));
                Console.WriteLine("Товар обновлен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Pause();
        }
        else if (choice == "3")
        {
            var sellerProducts = products.Where(p => p.Seller == seller).ToList();
            PrintProductList(sellerProducts);
            if (!TryPickFromList(sellerProducts, ReadInt("Номер товара: "), out var product))
            {
                Console.WriteLine("Товар не найден у этого продавца.");
                Pause();
                continue;
            }

            products.Remove(product);
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
            promotions.Add(promotion);
            Console.WriteLine($"Акция создана. Id = {promotion.Id}");
            Pause();
        }
        else if (choice == "5")
        {
            var sellerPromos = promotions.Where(p => p.Seller == seller).ToList();
            PrintPromotionList(sellerPromos);
            if (!TryPickFromList(sellerPromos, ReadInt("Номер акции: "), out var promo))
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

            try
            {
                seller.EditPromotion(seller, promo, title, desc, discount, start, end);
                Console.WriteLine("Акция обновлена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Pause();
        }
        else if (choice == "6")
        {
            var sellerPromos = promotions.Where(p => p.Seller == seller).ToList();
            PrintPromotionList(sellerPromos);
            if (!TryPickFromList(sellerPromos, ReadInt("Номер акции: "), out var promo))
            {
                Console.WriteLine("Акция не найдена у этого продавца.");
                Pause();
                continue;
            }

            var end = ReadUtc("Дата завершения (UTC): ");
            try
            {
                seller.EndPromotion(seller, promo, end);
                Console.WriteLine("Акция завершена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Pause();
        }
        else if (choice == "7")
        {
            var sellerProducts = products.Where(p => p.Seller == seller).ToList();
            PrintProductList(sellerProducts);
            if (!TryPickFromList(sellerProducts, ReadInt("Номер товара: "), out var product))
            {
                Console.WriteLine("Товар не найден у этого продавца.");
                Pause();
                continue;
            }

            var sellerPromos = promotions.Where(p => p.Seller == seller).ToList();
            PrintPromotionList(sellerPromos);
            if (!TryPickFromList(sellerPromos, ReadInt("Номер акции: "), out var promo))
            {
                Console.WriteLine("Акция не найдена у этого продавца.");
                Pause();
                continue;
            }

            try
            {
                seller.AddProductToPromotion(seller, product, promo);
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
            PrintProductList(products.Where(p => p.Seller == seller).ToList());
            Pause();
        }
        else if (choice == "9")
        {
            PrintPromotionList(promotions.Where(p => p.Seller == seller).ToList());
            Pause();
        }
    }
}

Customer EnsureCustomer()
{
    if (customers.Count == 0)
    {
        Console.Write("Создай покупателя. Имя: ");
        customers.Add(new Customer(new Name(Console.ReadLine() ?? string.Empty)));
    }

    Console.WriteLine();
    Console.WriteLine("Покупатели:");
    for (var i = 0; i < customers.Count; i++)
        Console.WriteLine($"{i + 1}. {customers[i].Name.Value} ({customers[i].Id})");

    var selected = ReadInt("Выбери номер покупателя: ");
    if (selected > customers.Count)
    {
        Console.Write("Имя нового покупателя: ");
        var c = new Customer(new Name(Console.ReadLine() ?? string.Empty));
        customers.Add(c);
        return c;
    }

    return customers[selected - 1];
}

Seller EnsureSeller()
{
    if (sellers.Count == 0)
    {
        Console.Write("Создай продавца. Имя: ");
        sellers.Add(new Seller(new Name(Console.ReadLine() ?? string.Empty)));
    }

    Console.WriteLine();
    Console.WriteLine("Продавцы:");
    for (var i = 0; i < sellers.Count; i++)
        Console.WriteLine($"{i + 1}. {sellers[i].Name.Value} ({sellers[i].Id})");

    var selected = ReadInt("Выбери номер продавца: ");
    if (selected > sellers.Count)
    {
        Console.Write("Имя нового продавца: ");
        var s = new Seller(new Name(Console.ReadLine() ?? string.Empty));
        sellers.Add(s);
        return s;
    }

    return sellers[selected - 1];
}

void PrintCatalog()
{
    Console.WriteLine();
    if (products.Count == 0)
    {
        Console.WriteLine("Каталог пуст.");
        return;
    }

    Console.WriteLine("Каталог:");
    PrintProductList(products);
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
    PrintPromotionList(promotions);
}

void PrintProductList(IReadOnlyList<Product> list)
{
    if (list.Count == 0)
    {
        Console.WriteLine("Нет товаров.");
        return;
    }

    for (var i = 0; i < list.Count; i++)
    {
        var p = list[i];
        var desc = p.Description?.Value ?? "—";
        Console.WriteLine($"{i + 1}. {p.Name.Value} | {p.Price.Value} | {desc} | seller={p.Seller.Name.Value}");
    }
}

void PrintPromotionList(IReadOnlyList<Promotion> list)
{
    if (list.Count == 0)
    {
        Console.WriteLine("Нет акций.");
        return;
    }

    for (var i = 0; i < list.Count; i++)
    {
        var p = list[i];
        var discount = p.Discount?.Value.ToString() ?? "нет";
        Console.WriteLine($"{i + 1}. {p.Title.Value} | скидка={discount}% | seller={p.Seller.Name.Value}");
    }
}

void PrintFavorites(Customer customer)
{
    Console.WriteLine();
    Console.WriteLine("Избранное:");
    if (customer.Favorites.Count == 0)
    {
        Console.WriteLine("Избранное пусто.");
        return;
    }

    var favProducts = customer.Favorites.Select(f => f.Product).ToList();
    PrintProductList(favProducts);
}

bool TryPickProduct(int index, out Product product)
{
    if (!TryPickFromList(products, index, out product)) return false;
    return true;
}

bool TryPickFromList<T>(IReadOnlyList<T> list, int oneBasedIndex, out T item)
{
    if (oneBasedIndex < 1 || oneBasedIndex > list.Count)
    {
        item = default!;
        return false;
    }

    item = list[oneBasedIndex - 1];
    return true;
}

Description? ToDescription(string? text)
    => string.IsNullOrWhiteSpace(text) ? null : new Description(text);

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
