using Byl.Core.AST.Nodes;
using Byl.Core.AST.Visitors;
using Byl.Core.AST.Visitors.Optimizers;
using Byl.Core.AST.Visitors.Semantic;
using Byl.Core.Lexer;
using Byl.Core.Parser;

namespace Byl.CLI;

internal class Compiler
{
    public string Compile(string code)
    {
        // 1. Лексический анализ
        var lexer = new Lexer();
        var tokens = lexer.Tokenize(code);

        // 2. Синтаксический анализ
        var parser = new Parser(tokens);
        var ast = parser.ParseProgram();

        // 3. Семантический анализ
        var semanticAnalyzer = new SemanticAnalyzer();
        var result = ast.Accept(semanticAnalyzer);
        if (!result.IsValid)
            throw new Exception($"Семантическая ошибка: {result.ErrorMessage}");

        // 4. Оптимизация AST (универсальный оптимизатор)
        var optimizer = new AstOptimizer();
        var optimizedAst = (ProgramNode)ast.Accept(optimizer);

        Console.WriteLine($"Применено {optimizer.AppliedOptimizations.Count} оптимизаций:");
        foreach (var optimization in optimizer.AppliedOptimizations)
        {
            Console.WriteLine($"  • {optimization}");
        }

        // 5. Генерация кода
        var codeGenerator = new CodeGenerator();
        return codeGenerator.Visit(optimizedAst);
    }
}
