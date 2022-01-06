using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Int, Float, String, Read, Write,Main,
    Repeat, Until, If, Elseif, Else, Then, Return, Endl, End, Dot,
    semicolon, Comma, LParanthesis, RParanthesis,
    EqualOp, NotEqualOp, LessThanOp, GreaterThanOp, AndOp, OrOp, 
    PlusOp, MinusOp, MultiplyOp, DivideOp, AssignOp, Idenifier, Number, Comment, LCurlyBraces, RCurlyBraces, constant, StringLiteral
}

namespace TINY_Compiler
{
    

    public class Token
    {
       public string Lex;
       public Token_Class TokenType;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.Int);
            ReservedWords.Add("float", Token_Class.Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add(";", Token_Class.semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add("{", Token_Class.LCurlyBraces);
            Operators.Add("}", Token_Class.RCurlyBraces);
            Operators.Add("=", Token_Class.EqualOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("||", Token_Class.OrOp);
            Operators.Add("&&", Token_Class.AndOp);
            Operators.Add(":=", Token_Class.AssignOp);
        }
    public void StartScanning(string SourceCode)
    {
            Tokens.Clear();
            Errors.Error_List.Clear();
            int lastIndex = -1;
            for (int i = 0; i < SourceCode.Length - 1;)
            {
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();
                int j = i + 1;
                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n' || CurrentChar == '\t') 
                {
                    i = j;
                    lastIndex = j;
                    continue;
                }
                if(char.IsLetter(CurrentChar)) 
                {
                    CurrentChar = SourceCode[j];
                    while (isLetter(CurrentChar) || isDigit(CurrentChar)) 
                    {
                        CurrentLexeme+=CurrentChar.ToString();
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                        else
                            break;
                    }
                }
                else if(isDigit(CurrentChar)) 
                {
                    CurrentChar = SourceCode[j];
                    while (isDigit(CurrentChar) || CurrentChar == '.') 
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                        else
                            break;
                    }
                }
                else if(CurrentChar == '/')
                {
                    CurrentChar = SourceCode[j];
                    if(CurrentChar == '*') 
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        CurrentChar = SourceCode[j];
                        int k = j + 1;
                        char NextChar;
                        while (j < SourceCode.Length && k < SourceCode.Length)
                        {
                            NextChar = SourceCode[k];
                            if (CurrentChar == '*' && NextChar == '/') 
                            {
                                j += 2;
                                CurrentLexeme += "*/";
                                break;
                            }
                            else
                            {
                                CurrentLexeme += CurrentChar.ToString();
                                j++;
                                k++;
                                CurrentChar = NextChar;
                            }
                        }
                    }
                }
                else if(CurrentChar == '"') 
                {
                    CurrentChar = SourceCode[j];
                    while (CurrentChar != '"')
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                        if (j < SourceCode.Length)
                            CurrentChar = SourceCode[j];
                        else
                            break;
                    }
                    if (j < SourceCode.Length && CurrentChar == '"') 
                    {
                        CurrentLexeme += CurrentChar.ToString();
                        j++;
                    }
                } 
                else
                { 
                    // checking the operators
                    char NextChar = SourceCode[j];
                    if(CurrentChar == '&' && NextChar == '&' ||
                       CurrentChar == '|' && NextChar == '|' ||
                       CurrentChar == '<' && NextChar == '>' ||
                       CurrentChar == ':' && NextChar == '=')
                    {
                        CurrentLexeme += NextChar.ToString();
                        j++;
                    }
                }
                FindTokenClass(CurrentLexeme);
                i = j;
                lastIndex = j;
            }
            if(lastIndex == SourceCode.Length - 1)
                FindTokenClass(SourceCode[lastIndex].ToString());
            TINY_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            // empty lexeme
            if(Lex.Length==0)
                return;
            Token_Class tokenClass;
            Token token = new Token();
            token.Lex = Lex;
            if(ReservedWords.ContainsKey(token.Lex)) 
            {
                tokenClass = ReservedWords[token.Lex];
                token.TokenType = tokenClass;
                Tokens.Add(token);
            }
            else if(IsIdentifier(token.Lex))
            {
                tokenClass = Token_Class.Idenifier;
                token.TokenType = tokenClass;
                Tokens.Add(token);
            }
            else if(isConstant(token.Lex)) 
            {
                tokenClass = Token_Class.constant;
                token.TokenType = tokenClass;
                Tokens.Add(token);
            }
            else if(Operators.ContainsKey(token.Lex))
            {
                tokenClass = Operators[token.Lex];
                token.TokenType = tokenClass;
                Tokens.Add(token);
            }
            else if(isStringLiteral(token.Lex))
            {
                tokenClass = Token_Class.StringLiteral;
                token.TokenType = tokenClass;
                Tokens.Add(token);
            }
            else if(IsComment(token.Lex))
            {
                tokenClass = Token_Class.Comment;
                token.TokenType = tokenClass;
                Tokens.Add(token);
            }
            else
                Errors.Error_List.Add(Lex);
        }
        public bool IsIdentifier(string lex)
        {
            Regex reg = new Regex(@"^([a-zA-Z])([0-9a-zA-Z])*$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }
        bool isConstant(string lex)
        {
            bool isValid = true;
            bool isDecimal = false;
            int i = 0;
            while (i < lex.Length && isDigit(lex[i]))
                i++;
            if(i!= lex.Length)
            {
                if (lex[i] != '.')
                    isValid = false;
                else 
                {
                    i++;
                    isDecimal = true;
                }
            }
            if(isDecimal) 
            {
                if (i == lex.Length)
                    isValid = false;
                else
                {
                    while (i < lex.Length && isDigit(lex[i]))
                        i++;
                }
            }
            if(i!=lex.Length)
                isValid = false;
            return isValid;
        }
        bool isStringLiteral(string lex)
        {
            bool isValid = true;
            int len = lex.Length;
            if (!(lex[0] == '"' && lex[len - 1] == '"'))
                isValid = false;
            return isValid;
        }
        public bool IsComment(string lex)
        {
            return (lex.Length >= 4 && lex.StartsWith("/*") && lex.EndsWith("*/"));
        }
        bool isLetter(char c)
        {
            return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
        }
        public bool isDigit(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}
