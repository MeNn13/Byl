using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Byl.Core.Lexer;

namespace Byl.Core.AST.Visitors.Semantic;

internal class TypeSystem
{
    public static readonly Dictionary<string, string[]> CompatibleTypes = new()
    {
        ["общ"] = ["цел", "лог", "стр", "общ"],
        ["цел"] = ["цел", "общ"],
        ["лог"] = ["лог", "общ"],
        ["стр"] = ["стр", "общ"]
    };

    public static readonly Dictionary<TokenType, string[]> AllowedOperations = new()
    {
        [TokenType.Plus] = ["цел", "стр", "общ"],
        [TokenType.Minus] = ["цел", "общ"],
        [TokenType.Multiply] = ["цел", "общ"],
        [TokenType.Divide] = ["цел", "общ"],
        [TokenType.Equal] = ["цел", "лог", "стр", "общ"],
        [TokenType.NotEqual] = ["цел", "лог", "стр", "общ"],
        [TokenType.GreaterThan] = ["цел", "общ"],
        [TokenType.And] = ["лог", "общ"],
        [TokenType.Or] = ["лог", "общ"],
        [TokenType.Not] = ["лог", "общ"]
    };

    public static bool AreTypesCompatible(string type1, string type2)
    {
        if (type1 == type2) return true;
        return CompatibleTypes.TryGetValue(type1, out var compatible) &&
               compatible.Contains(type2);
    }

    public static bool IsOperationAllowed(TokenType operation, string type) =>
        AllowedOperations.TryGetValue(operation, out var allowed) &&
               allowed.Contains(type);

    public static string GetBinaryOperationResultType(string leftType, string rightType, TokenType operation)
    {
        if (!AreTypesCompatible(leftType, rightType))
            return "несовместимо";

        // Для арифметических операций
        if (operation is TokenType.Plus or TokenType.Minus or
            TokenType.Multiply or TokenType.Divide)
        {
            if (leftType == "стр" || rightType == "стр") return "стр";
            if (leftType == "цел" && rightType == "цел") return "цел";
            return "общ";
        }

        // Для сравнений
        if (operation is TokenType.Equal or TokenType.NotEqual or
            TokenType.GreaterThan or TokenType.LessThan)
        {
            return "лог";
        }

        // Для логических операций
        if (operation is TokenType.And or TokenType.Or)
        {
            return "лог";
        }

        return "общ";
    }
}
