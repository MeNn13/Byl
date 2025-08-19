using Byl.CLI;

var code = "общ главный() {печать(1+1);}";

var compiler = new Compiler();

try
{
    var compiled = compiler.Compile(code);
    Console.WriteLine($"Компилятор выполнен");

    var result = compiler.Interpret(code);
    Console.WriteLine($"Результат интерпретатора: {result}");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}