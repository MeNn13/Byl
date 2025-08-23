using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;
using Byl.Core.AST.Visitors.Semantic.Visitors;

namespace Byl.Core.AST.Visitors.Semantic;

public class SemanticAnalyzer : ISemanticVisitor
{

    private readonly ProgramSemantic _programVisitor;

    public SemanticAnalyzer()
    {
        _programVisitor = new ProgramSemantic();
    }

    public SemanticResult Visit(ProgramNode node) => _programVisitor.Visit(node);

    public SemanticResult Visit(NamespaceDeclaration node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ImportDeclaration node) => _programVisitor.Visit(node);
    public SemanticResult Visit(MethodDeclaration node) => _programVisitor.Visit(node);
    public SemanticResult Visit(TypeNode node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ClassDeclaration node) => _programVisitor.Visit(node);
    public SemanticResult Visit(FieldDeclaration node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ConstructorDeclaration node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ParameterNode node) => _programVisitor.Visit(node);
    public SemanticResult Visit(BlockStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(PrintStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(AssignStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(IfStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(WhileStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ReturnStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(VariableDeclarationStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ExpressionStatement node) => _programVisitor.Visit(node);
    public SemanticResult Visit(BinaryExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(LiteralExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(VariableExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(UnaryExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(InterpolatedStringExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(FunctionCallExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(NewExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(MemberAccessExpression node) => _programVisitor.Visit(node);
    public SemanticResult Visit(ThisExpression node) => _programVisitor.Visit(node);
}
