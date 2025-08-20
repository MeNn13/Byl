using Byl.CLI;

Run(args);

static void Run(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Используйте: byl <filename.byl>");
        Console.WriteLine("Или перетащите .byl файл на exe");
        return;
    }

    string filePath = args[0];

    // Если файл не существует, проверяем в текущей директории
    if (!File.Exists(filePath))
    {
        string currentDirFile = Path.Combine(Directory.GetCurrentDirectory(), filePath);
        if (File.Exists(currentDirFile))
        {
            filePath = currentDirFile;
        }
        else
        {
            Console.WriteLine($"Ошибка: Файл '{filePath}' не найден");
            return;
        }
    }

    if (!filePath.EndsWith(".byl"))
    {
        Console.WriteLine("Ошибка: Файл должен быть с .byl расширением");
        return;
    }

    try
    {
        string code = File.ReadAllText(filePath);
        Compiler compiler = new();
        string result = compiler.Compile(code);

        string outputFile = Path.ChangeExtension(filePath, ".c");
        File.WriteAllText(outputFile, result);

        Console.WriteLine($"Успешно! Скомпилировано в: {outputFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}
