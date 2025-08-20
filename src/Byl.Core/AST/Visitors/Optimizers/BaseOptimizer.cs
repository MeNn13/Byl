using Byl.Core.AST.Nodes;
using Byl.Core.AST.Nodes.Declaration;
using Byl.Core.AST.Nodes.Expression;
using Byl.Core.AST.Nodes.Statement;

namespace Byl.Core.AST.Visitors.Optimizers;

internal abstract class BaseOptimizer : IOptimizer
{
    public abstract string Name { get; }

    public abstract bool CanOptimize(Node node);

    public Node Visit(Node node) => node switch
    {
        BinaryExpression binary => VisitBinary(binary),
        UnaryExpression unary => VisitUnary(unary),
        LiteralExpression literal => VisitLiteral(literal),
        VariableExpression variable => VisitVariable(variable),
        InterpolatedStringExpression interpolated => VisitInterpolated(interpolated),
        PrintStatement print => VisitPrint(print),
        AssignStatement assign => VisitAssign(assign),
        IfStatement ifStmt => VisitIf(ifStmt),
        WhileStatement whileStmt => VisitWhile(whileStmt),
        ReturnStatement returnStmt => VisitReturn(returnStmt),
        VariableDeclarationStatement varDecl => VisitVarDecl(varDecl),
        BlockStatement block => VisitBlock(block),
        FunctionDeclaration func => VisitFunction(func),
        FunctionCallExpression funcCall => VisitFunctionCall(funcCall),
        ProgramNode program => VisitProgram(program),
        ExpressionStatement program => VisitExpressionStatement(program),
        _ => node
    };

    protected virtual Node VisitBinary(BinaryExpression node) => node;
    protected virtual Node VisitUnary(UnaryExpression node) => node;
    protected virtual Node VisitLiteral(LiteralExpression node) => node;
    protected virtual Node VisitVariable(VariableExpression node) => node;
    protected virtual Node VisitInterpolated(InterpolatedStringExpression node) => node;
    protected virtual Node VisitPrint(PrintStatement node) => node;
    protected virtual Node VisitAssign(AssignStatement node) => node;
    protected virtual Node VisitIf(IfStatement node) => node;
    protected virtual Node VisitWhile(WhileStatement node) => node;
    protected virtual Node VisitReturn(ReturnStatement node) => node;
    protected virtual Node VisitVarDecl(VariableDeclarationStatement node) => node;
    protected virtual Node VisitBlock(BlockStatement node) => node;
    protected virtual Node VisitFunction(FunctionDeclaration node) => node;
    protected virtual Node VisitProgram(ProgramNode node) => node;
    protected virtual Node VisitFunctionCall(FunctionCallExpression node) => node;
    protected virtual Node VisitExpressionStatement(ExpressionStatement node) => node;
}
