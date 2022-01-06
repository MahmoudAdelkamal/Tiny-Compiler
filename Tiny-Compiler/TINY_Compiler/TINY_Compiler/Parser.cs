using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();
        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        private int TokenIndex = 0;
        List<Token> TokenStream;
        public Node root;
        private Boolean MainFunctionExecuted = false;
        public Node StartParsing(List<Token> TokenStream)
        {
            this.TokenStream = TokenStream;
            root = new Node("Root Node");
            root.Children.Add(Program());

            if (!MainFunctionExecuted)
                Errors.Error_List.Add("The code misses the main function !!!!");

            return root;
        }
        private Node ReturnStatement()
        {
            Node node = new Node("return_statement");
            node.Children.Add(Match(Token_Class.Return));
            node.Children.Add(Expression());
            node.Children.Add(Match(Token_Class.semicolon));
            return node;
        }
        private Node FunctionCall()
        {
            Node node = new Node("function_call");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(FunctionParametersPart());
            return node;
        }
        private Node Identifiers()
        {
            Node node = new Node("identifiers");
            if (IsvalidToken(Token_Class.Idenifier))
            {
                node.Children.Add(Match(Token_Class.Idenifier));
                node.Children.Add(IdentifierDetails());
                return node;
            }
            else if (IsvalidToken(Token_Class.constant))
            {
                node.Children.Add(Match(Token_Class.constant));
                node.Children.Add(IdentifierDetails());
                return node;
            }
            return null;
        }
        private Node IdentifierDetails()
        {
            Node node = new Node("Identifier_details");
            if (IsvalidToken(Token_Class.Comma))
            {
                node.Children.Add(Match(Token_Class.Comma));
                node.Children.Add(Identifiers());
                return node;
            }
            return null;
        }
        private Node Condition()
        {
            Node node = new Node("Condition");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(ConditionOperator());
            node.Children.Add(Term());
            return node;
        }
        private Node Term()
        {
            Node node = new Node("Term");
            if (IsvalidToken(Token_Class.Idenifier))
            {
                node.Children.Add(Match(Token_Class.Idenifier));
                node.Children.Add(TermFactoring());
            }
            else
                node.Children.Add(Match(Token_Class.constant));
            return node;
        }
        private Node TermFactoring()
        {
            if (IsvalidToken(Token_Class.LParanthesis))
            {
                Node node = new Node("Term_Factoring");
                node.Children.Add(FunctionParametersPart());
                return node;
            }
            return null;
        }
        private Node ConditionOperator()
        {
            Node node = new Node("Condition_Operator");
            if (IsvalidToken(Token_Class.EqualOp))
                node.Children.Add(Match(Token_Class.EqualOp));
            else if (IsvalidToken(Token_Class.GreaterThanOp))
                node.Children.Add(Match(Token_Class.GreaterThanOp));
            else if (IsvalidToken(Token_Class.LessThanOp))
                node.Children.Add(Match(Token_Class.LessThanOp));
            else
                node.Children.Add(Match(Token_Class.NotEqualOp));
            return node;
        }
        private Node DeclarationStatement()
        {
            Node node = new Node("Declaration_Statement");
            node.Children.Add(Datatype());
            node.Children.Add(DeclarationDetails());
            return node;
        }
        private Node DeclarationDetails()
        {
            Node node = new Node("Declaration_Details");
            node.Children.Add(DeclarationDetail());
            node.Children.Add(ExtraDeclarationDetails());
            return node;
        }
        private Node ExtraDeclarationDetails()
        {
            if (IsvalidToken(Token_Class.Comma))
            {
                Node node = new Node("Extra_declaration_details");
                node.Children.Add(Match(Token_Class.Comma));
                node.Children.Add(DeclarationDetails());
                return node;
            }
            return null;
        }
        private Node DeclarationDetail()
        {
            Node node = new Node("Declaration_Detail");
            node.Children.Add(Match(Token_Class.Idenifier));
            node.Children.Add(ExtraDeclerationDetail());
            return node;
        }
        private Node ExtraDeclerationDetail()
        {
            if (IsvalidToken(Token_Class.AssignOp))
            {
                Node node = new Node("Extra_Decleration_Detail");
                node.Children.Add(Assignment_Statement());
                return node;
            }
            return null;
        }

        private Node Assignment_Statement()
        {
            Node node = new Node("Assignment_Statement");
            node.Children.Add(Match(Token_Class.AssignOp));
            node.Children.Add(Expression());
            return node;
        }
        private Node AdditionalExpression()
        {
            if (IsvalidToken(Token_Class.PlusOp) || IsvalidToken(Token_Class.MinusOp) ||
                IsvalidToken(Token_Class.MultiplyOp) || IsvalidToken(Token_Class.DivideOp))
            {
                Node node = new Node("Additional_Expression");
                node.Children.Add(ArithmaticOperator());
                node.Children.Add(Equation());
                return node;
            }
            return null;
        }
        private Node Equation()
        {
            Node node = new Node("equation");
            node.Children.Add(TermEq());
            node.Children.Add(AdditionalEquation());
            return node;
        }
        private Node AdditionalEquation()
        {
            Node node = new Node("AdditionalEquation");
            if (IsvalidToken(Token_Class.PlusOp) || IsvalidToken(Token_Class.MinusOp))
            {
                node.Children.Add(AddTerm());
                node.Children.Add(AdditionalEquation());
                return node;
            }
            return null;
        }
        private Node AddTerm()
        {
            Node node = new Node("Add_Term");
            node.Children.Add(AddSubtractOperation());
            node.Children.Add(TermEq());
            return node;
        }
        private Node TermEq()
        { 
            Node node = new Node("TermEq");
            node.Children.Add(Factor());
            node.Children.Add(AdditionalTermEq());
            return node;
        }
        private Node AdditionalTermEq()
        {
            Node node = new Node("Additional_TermEq");
            if (IsvalidToken(Token_Class.MultiplyOp) || IsvalidToken(Token_Class.DivideOp))
            {
                node.Children.Add(MulDivTerm());
                node.Children.Add(AdditionalTermEq());
                return node;
            }
            return null;
        }
        private Node MulDivTerm()
        {
            Node node = new Node("Mul_Div_Term");
            node.Children.Add(MulDivOperation());
            node.Children.Add(Factor());
            return node;
        }
        private Node Factor()
        {
            Node node = new Node("Factor");
            if (IsvalidToken(Token_Class.LParanthesis))
            {
                node.Children.Add(Match(Token_Class.LParanthesis));
                node.Children.Add(Equation());
                node.Children.Add(Match(Token_Class.RParanthesis));
            }
            else
                node.Children.Add(Term());
            return node;
        }
        private Node AddSubtractOperation()
        {
            Node node = new Node("add_Subtract_Operation");
            if (IsvalidToken(Token_Class.PlusOp))
                node.Children.Add(Match(Token_Class.PlusOp));
            else
                node.Children.Add(Match(Token_Class.MinusOp));
            return node;
        }
        private Node MulDivOperation()
        {
            Node node = new Node("mulDiv_Operation");
            if (IsvalidToken(Token_Class.MultiplyOp))
                node.Children.Add(Match(Token_Class.MultiplyOp));
            else
                node.Children.Add(Match(Token_Class.DivideOp));
            return node;
        }
        private Node ArithmaticOperator()
        {
            Node node = new Node("Arithmatic_Operator");
            if (IsvalidToken(Token_Class.PlusOp))
                node.Children.Add(Match(Token_Class.PlusOp));
            else if (IsvalidToken(Token_Class.MinusOp))
                node.Children.Add(Match(Token_Class.MinusOp));
            else if (IsvalidToken(Token_Class.DivideOp))
                node.Children.Add(Match(Token_Class.DivideOp));
            else if (IsvalidToken(Token_Class.MultiplyOp))
                node.Children.Add(Match(Token_Class.MultiplyOp));
            return node;
        }
        private Node Datatype()
        {
            Node node = new Node("Datatype");
            if (IsvalidToken(Token_Class.Int))
                node.Children.Add(Match(Token_Class.Int));
            else if (IsvalidToken(Token_Class.Float))
                node.Children.Add(Match(Token_Class.Float));
            else if (IsvalidToken(Token_Class.String))
                node.Children.Add(Match(Token_Class.String));
            return node;
        }
        private Node FunctionName()
        {
            Node node = new Node("Function_Name");
            node.Children.Add(Match(Token_Class.Idenifier));
            return node;
        }
        private Node AdditionalParameters()
        {
            if(IsvalidToken(Token_Class.Comma))
            {
                Node node = new Node("Additional_Parameters");
                node.Children.Add(Match(Token_Class.Comma));
                node.Children.Add(Parameters());
                return node;
            }
            return null;
        }
        private Node Parameter()
        {
            Node node = new Node("Parameter");
            node.Children.Add(Datatype());
            node.Children.Add(Match(Token_Class.Idenifier));
            return node;
        }
        private Node Parameters()
        {
            if(IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                Node node = new Node("Parameters");
                node.Children.Add(Parameter());
                node.Children.Add(AdditionalParameters());
                return node;
            }
            return null;
        }
        private Node MainFunction()
        {
            Node node = new Node("Main_Function");
            node.Children.Add(Match(Token_Class.Main));
            node.Children.Add(Match(Token_Class.LParanthesis));
            node.Children.Add(Match(Token_Class.RParanthesis));
            MainFunctionExecuted = true;
            return node;
        }
        private Node FunctionDeclarationDetails()
        {
            Node node = new Node("Function_Declaration_Details");
            // Main Function call
            if (IsvalidToken(Token_Class.Main))
                node.Children.Add(MainFunction());
            else
            {
                // normal function
                node.Children.Add(FunctionName());
                node.Children.Add(Match(Token_Class.LParanthesis));
                node.Children.Add(Parameters());
                node.Children.Add(Match(Token_Class.RParanthesis));
            }
            return node;
        }
        private Node FunctionDeclaration()
        {
            Node node = new Node("Function_Declaration");
            node.Children.Add(Datatype());
            node.Children.Add(FunctionDeclarationDetails());
            return node;
        }
        private Node BooleanOperator()
        {
            Node node = new Node("Boolean_Operator");
            if(IsvalidToken(Token_Class.OrOp))
                node.Children.Add(Match(Token_Class.OrOp));
            else
                node.Children.Add(Match(Token_Class.AndOp));
            return node;
        }
        private Node ConditionStatement()
        {
            Node node = new Node("Condition_Statement");
            node.Children.Add(Condition());
            node.Children.Add(AdditionalConditionalStatement());
            return node;
        }
        private Node AdditionalConditionalStatement()
        {
            if(IsvalidToken(Token_Class.AndOp) || IsvalidToken(Token_Class.OrOp))
            {
                Node node = new Node("Additional_Conditional_Statement");
                node.Children.Add(BooleanOperator());
                node.Children.Add(ConditionStatement());
                return node;
            }
            return null;
        }
        private Node ElseIfStatment()
        {
            Node node = new Node("Else_If_Statment");
            node.Children.Add(Match(Token_Class.Elseif));
            node.Children.Add(ConditionStatement());
            node.Children.Add(Match(Token_Class.Then));
            node.Children.Add(Statements());
            node.Children.Add(ElseClause());
            return node;
        }
        private Node ElseStatment()
        {
            Node node = new Node("Else_Statment");
            node.Children.Add(Match(Token_Class.Else));
            node.Children.Add(Statements());
            node.Children.Add(Match(Token_Class.End));
            return node;
        }
        private Node ElseClause()
        {
            Node node = new Node("ElseClause");
            if (IsvalidToken(Token_Class.Elseif))
                node.Children.Add(ElseIfStatment());
            else if (IsvalidToken(Token_Class.Else))
                node.Children.Add(ElseStatment());
            else
                node.Children.Add(Match(Token_Class.End));
            return node;
        }
        private Node IfStatement()
        {
            Node node = new Node("If_Statement");
            node.Children.Add(Match(Token_Class.If));
            node.Children.Add(ConditionStatement());
            node.Children.Add(Match(Token_Class.Then));
            node.Children.Add(Statements());
            node.Children.Add(ElseClause());
            return node;
        }
        private Node RepeatStatement()
        {
            Node node = new Node("Repeat_Statement");
            node.Children.Add(Match(Token_Class.Repeat));
            node.Children.Add(Statements());
            node.Children.Add(Match(Token_Class.Until));
            node.Children.Add(ConditionStatement());
            return node;
        }
        private Node StatementWithNoSemiColon()
        {
            Node node = new Node("Statement_With_No_SemiColon");
            if (IsvalidToken(Token_Class.If))
                node.Children.Add(IfStatement());
            else if (IsvalidToken(Token_Class.Repeat))
                node.Children.Add(RepeatStatement());
            else
                node.Children.Add(Match(Token_Class.Comment));
            return node;
        }
        private Node FunctionParametersPart()
        {
            Node node = new Node("Parameters_part");
            node.Children.Add(Match(Token_Class.LParanthesis));
            node.Children.Add(Identifiers());
            node.Children.Add(Match(Token_Class.RParanthesis));
            return node;
        }
        private Node StatementType()
        {
            Node node = new Node("Assignment_type");
            if (IsvalidToken(Token_Class.LParanthesis))
            {
                // function call --> sum(1,5);
                node.Children.Add(FunctionParametersPart());
            }
            else
            {
                // Assignment statement --> sum = 5;
                node.Children.Add(Assignment_Statement());
            }
            return node;
        }
        private Node ReadStatement()
        {
            Node node = new Node("Read_Statement");
            node.Children.Add(Match(Token_Class.Read));
            node.Children.Add(Match(Token_Class.Idenifier));
            return node;
        }
        private Node Expression()
        {
            Node node = new Node("Expression");
            if (IsvalidToken(Token_Class.StringLiteral))
                node.Children.Add(Match(Token_Class.StringLiteral));
            else if (IsvalidToken(Token_Class.LParanthesis))
            {

                node.Children.Add(Match(Token_Class.LParanthesis));
                node.Children.Add(Equation());
                node.Children.Add(Match(Token_Class.RParanthesis));
            }
            else
            {
                node.Children.Add(Term());
                node.Children.Add(AdditionalExpression());
            }
            return node;
        }
        private Node Next()
        {
            Node node = new Node("Next");
            if (IsvalidToken(Token_Class.Endl))
                node.Children.Add(Match(Token_Class.Endl));
            else
                node.Children.Add(Expression());
            return node;
        }
        private Node WriteStatement()
        {
            Node node = new Node("Write_Statement");
            node.Children.Add(Match(Token_Class.Write));
            node.Children.Add(Next());
            return node;
        }
        private Node StatementWithSemiColon()
        {
            Node node = new Node("Statement_With_SemiColon");
            if (IsvalidToken(Token_Class.Idenifier))
            {
                node.Children.Add(Match(Token_Class.Idenifier));
                node.Children.Add(StatementType());
            }
            else if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
                node.Children.Add(DeclarationStatement());
            else if (IsvalidToken(Token_Class.Read))
                node.Children.Add(ReadStatement());
            else if (IsvalidToken(Token_Class.Write))
                node.Children.Add(WriteStatement());
            return node;
        }
        private Node Statement()
        {
            Node node = new Node("Statement");
            if (IsvalidToken(Token_Class.Comment) || IsvalidToken(Token_Class.If) || IsvalidToken(Token_Class.Repeat))
            {
                node.Children.Add(StatementWithNoSemiColon());
            }
            else
            {
                node.Children.Add(StatementWithSemiColon());
                node.Children.Add(Match(Token_Class.semicolon));
            }
            return node;
        }
        private Node Statements()
        {
            if (IsvalidToken(Token_Class.Idenifier) || IsvalidToken(Token_Class.String)
              || IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float)
             || IsvalidToken(Token_Class.If) || IsvalidToken(Token_Class.Repeat)
             || IsvalidToken(Token_Class.Read) || IsvalidToken(Token_Class.Write)
             || IsvalidToken(Token_Class.Comment))
            {
                Node node = new Node("Statements");
                node.Children.Add(Statement());
                // looking for additional statements
                node.Children.Add(Statements());
                return node;
            }
            return null;
        }
        private Node FunctionBody()
        {
            Node node = new Node("Function_Body");
            node.Children.Add(Match(Token_Class.LCurlyBraces));
            node.Children.Add(Statements());
            node.Children.Add(ReturnStatement());
            node.Children.Add(Match(Token_Class.RCurlyBraces));
            return node;
        }
        private Node FunctionStatement()
        {
            Node node = new Node("Function_Statement");
            node.Children.Add(FunctionDeclaration());
            node.Children.Add(FunctionBody());
            return node;
        }
        private Node FunctionStatements()
        {
            if (IsvalidToken(Token_Class.Int) || IsvalidToken(Token_Class.Float) || IsvalidToken(Token_Class.String))
            {
                Node node = new Node("Function_Statements");
                node.Children.Add(FunctionStatement());
                // looking for another statements
                node.Children.Add(FunctionStatements());
                return node;
            }
            else if (IsvalidToken(Token_Class.Comment))
            {
                Node node = new Node("Comment");
                node.Children.Add(Match(Token_Class.Comment));
                node.Children.Add(FunctionStatements());
                return node;
            }
            return null;
        }
        private Node Program()
        {
            Node node = new Node("Program");
            node.Children.Add(FunctionStatements());
            return node;
        }
        private bool IsvalidToken(Token_Class token)
        {
            return (TokenIndex < TokenStream.Count && TokenStream[TokenIndex].TokenType == token);
        }

        private Node Match(Token_Class ExpectedToken)
        {

            if (TokenIndex < TokenStream.Count && ExpectedToken == TokenStream[TokenIndex].TokenType)
            {
                TokenIndex++;
                Node newNode = new Node(ExpectedToken.ToString());

                return newNode;

            }
            else
            {
                if (TokenIndex < TokenStream.Count)
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[TokenIndex].TokenType.ToString() +
                        " found\r\n" 
                        + " at " + TokenStream[TokenIndex].TokenType.ToString() + "\n");

                    TokenIndex++;
                }else
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and nothing was found\r\n");

                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        public static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
