using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST;

public interface IAstVisitor<TResult>
{
    // Types
    TResult Visit(TypeNode node);

    // Declarations
    TResult Visit(ProgramNode node);
    TResult Visit(MethodDeclaration node);
    TResult Visit(NamespaceDeclaration node);
    TResult Visit(ClassDeclaration node);
    TResult Visit(ImportDeclaration node);
    TResult Visit(FieldDeclaration node);
    TResult Visit(ConstructorDeclaration node);
    TResult Visit(ParameterNode node);

    // Statements
    TResult Visit(BlockStatement node);
    TResult Visit(PrintStatement node);
    TResult Visit(AssignStatement node);
    TResult Visit(IfStatement node);
    TResult Visit(WhileStatement node);
    TResult Visit(ReturnStatement node);
    TResult Visit(VariableDeclarationStatement node);
    TResult Visit(ExpressionStatement node);

    // Expressions
    TResult Visit(BinaryExpression node);
    TResult Visit(LiteralExpression node);
    TResult Visit(VariableExpression node);
    TResult Visit(UnaryExpression node);
    TResult Visit(InterpolatedStringExpression node);
    TResult Visit(FunctionCallExpression node);
    TResult Visit(NewExpression node);
    TResult Visit(MemberAccessExpression node);
    TResult Visit(ThisExpression node);
}
