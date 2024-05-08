using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Rec
    {
        public string result { get; private set; }
        public Rec(string result)
        {
            this.result = result;
        }
    }

    public class Recursive
    {
        public string result = "";
        private List<Lex> lexemes;
        private Lex currLex;
        private int CurrIndex;
        private int MaxIndex;
        private const string sep = " - ";

        public List<Rec> RecursiveParser(string expression)
        {
            if (expression == "")
                throw new Exception("Поле не содержит вырожения");
            
            var lexes = new LexicalAnalyzer();
            lexes.AnalysisText(expression);
            lexemes = lexes.Lexemes;
            lexemes = lexemes.Where(x => x.lex != LexemeType.Invalid).ToList();
            CurrIndex = 0;
            MaxIndex = lexemes.Count - 1;
            currLex = lexemes[CurrIndex];
            result = string.Empty;

            try
            {
                stmt();
            }
            catch (SyntaxErrorException)
            {
                result = ("Syntax Error: Обнаружено ошибочное выражение.");
            }

            result = result.Remove(result.Length - 2);
            var ret = new List<Rec>();
            ret.Add(new Rec(result));
            return ret;
        }

        private bool stmt()
        {
            if (CurrIndex == MaxIndex)
            {
                return false;
            }

            result += "stmt" + sep;
            if (currLex.lex == LexemeType.If)
            {
                result += "IF" + sep;
                if (CanGetNext())
                    ChangeCurrentLex();
                else
                    return false;

                result += exp("");
                stmt();
                if (currLex.lex == LexemeType.Else)
                {
                    result += "ELSE" + sep;

                    if (CanGetNext())
                        ChangeCurrentLex();
                    else
                        return false;

                    stmt();
                }
            }
            else if (currLex.lex == LexemeType.Id)
            {
                result += "ID" + sep;
                if (CanGetNext())
                    ChangeCurrentLex();
                else
                    return false;


                if (currLex.lex == LexemeType.Assign)
                {
                    result += "ASSIGN" + sep;
                    if (CanGetNext())
                        ChangeCurrentLex();
                    else
                        return false;

                }

                result += exp("");
                if (currLex.lex == LexemeType.Semicolon)
                {
                    result += "SEMICOLON" + sep;
                    if (CanGetNext())
                        ChangeCurrentLex();
                    else
                        return false;

                    return true;
                }
            }

            return true;
        }

        private string exp(string str_exp)
        {
            if (CurrIndex == MaxIndex)
            {
                return str_exp;
            }
            str_exp += "exp" + sep;

            if (currLex.lex == LexemeType.Exp)
            {
                if (currLex.val == "TRUE")
                    str_exp += "TRUE" + sep;
                else
                    str_exp += "FALSE" + sep;

                if (CanGetNext())
                    ChangeCurrentLex();
                else
                    return str_exp;


                if (currLex.lex == LexemeType.Operator)
                {
                    if (currLex.val == "AND")
                        str_exp += "AND" + sep;
                    else if (currLex.val == "OR")
                        str_exp += "OR" + sep;

                    str_exp = "exp" + sep + str_exp;
                    if (CanGetNext())
                        ChangeCurrentLex();
                    else
                        return str_exp;
                    str_exp = exp(str_exp);
                }
                else if (currLex.lex == LexemeType.Exp)
                {
                    if (CanGetNext())
                        ChangeCurrentLex();
                    else
                        return str_exp;
                }
            }

            if (currLex.val == "NOT")
            {
                str_exp += "NOT" + sep;

                if (CanGetNext())
                    ChangeCurrentLex();
                else
                    return str_exp;

                str_exp = exp(str_exp);
            }

            return str_exp;
        }


        private void ChangeCurrentLex()
        {
            if (CanGetNext())
            {
                CurrIndex++;
                currLex = lexemes[CurrIndex];
            }
            else
            {
                throw new SyntaxErrorException();
            }
        }

        private bool CanGetNext() => CurrIndex < MaxIndex;
    }
}
