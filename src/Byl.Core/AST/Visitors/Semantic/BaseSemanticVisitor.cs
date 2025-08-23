using System.Xml.Linq;
using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.AST.Visitors.Semantic.Symbols;

namespace Byl.Core.AST.Visitors.Semantic;

public interface ISemanticVisitor : IAstVisitor<SemanticResult>;
public abstract class BaseSemanticVisitor : ISemanticVisitor
{
    public virtual SemanticResult Visit(ProgramNode node) => SemanticResult.Success();
    public virtual SemanticResult Visit(NamespaceDeclaration node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ImportDeclaration node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ClassDeclaration node) => SemanticResult.Success();
    public virtual SemanticResult Visit(FieldDeclaration node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ConstructorDeclaration node) => SemanticResult.Success();
    public virtual SemanticResult Visit(MethodDeclaration node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ParameterNode node) => SemanticResult.Success();
    public virtual SemanticResult Visit(BlockStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(PrintStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(AssignStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(IfStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(WhileStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ReturnStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(VariableDeclarationStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(TypeNode node) => SemanticResult.Success();
    public virtual SemanticResult Visit(BinaryExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(LiteralExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(VariableExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(InterpolatedStringExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(UnaryExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(FunctionCallExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ExpressionStatement node) => SemanticResult.Success();
    public virtual SemanticResult Visit(NewExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(MemberAccessExpression node) => SemanticResult.Success();
    public virtual SemanticResult Visit(ThisExpression node) => SemanticResult.Success();
}
