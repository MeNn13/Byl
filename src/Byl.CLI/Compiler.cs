using Byl.Core.AST.Nodes;
using Byl.Core.AST.Visitors;
using Byl.Core.AST.Visitors.Optimize;
using Byl.Core.AST.Visitors.Semantic;
using Byl.Core.Lexer;
using Byl.Core.Parser;

namespace Byl.CLI
{
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
            var semanticResult = semanticAnalyzer.Visit(ast);
            if (!semanticResult.IsValid)
                throw new Exception($"Семантическая ошибка: {semanticResult.ErrorMessage}");

            // 4. Оптимизация AST (универсальный оптимизатор)
            var optimizer = new UniversalOptimizer();
            var optimizedAst = (ProgramNode)ast.Accept(optimizer);

            // Логируем выполненные оптимизации
            if (optimizer.Optimizations.Count > 0)
            {
                Console.WriteLine("Оптимизированно:");
                foreach (var optimization in optimizer.Optimizations)
                {
                    Console.WriteLine($"  - {optimization}");
                }
            }

            // 5. Генерация кода
            var codeGenerator = new CodeGenerator();
            return codeGenerator.Visit(optimizedAst);
        }

        public object Interpret(string code)
        {
            // 1-3. Анализ как выше
            var lexer = new Lexer();
            var tokens = lexer.Tokenize(code);
            var parser = new Parser(tokens);
            var ast = parser.ParseProgram();

            var semanticAnalyzer = new SemanticAnalyzer();
            var semanticResult = semanticAnalyzer.Visit(ast);
            if (!semanticResult.IsValid)
                throw new Exception($"Семантическая ошибка: {semanticResult.ErrorMessage}");

            // 4. Оптимизация для интерпретатора
            var optimizer = new UniversalOptimizer();
            var optimizedAst = (ProgramNode)ast.Accept(optimizer);

            // 4. Интерпретация
            var interpreter = new Interpreter();
            return interpreter.Visit(ast);
        }
    }
}
