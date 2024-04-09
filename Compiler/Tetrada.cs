using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Tetrada
    {
        public char op { get; private set; }
        public string arg1 { get; private set; }
        public string arg2 { get; private set; }
        public string result { get; private set; }

        public Tetrada(char op, string arg1, string arg2, string result)
        {
            this.op = op;
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.result = result;
        }

    }

    public class TetradaParser
    {
        public List<Tetrada> tetradas = new List<Tetrada>();
        public string postfixForm { get; private set; }
        private List<Lex> lexemes;
        private Lex lex;

        private Dictionary<char, int> opPriority = new() {
            {'=', 0},
            {'+', 1},
            {'-', 1},
            {'*', 2},
            {'/', 2},
            {'~', 3},
        };

        public TetradaParser(string expression)
        {
            if (expression == "")
                throw new Exception("Поле не содержит вырожения");
            var lexes = new LexicalAnalyzer();
            lexes.AnalysisText(expression);
            lexemes = lexes.Lexemes;
            TetPaeser();
            postfixForm = ToPostfix();
        }

        private void TetPaeser()
        {
            lex = lexemes[0];
            Efunc();
        }

        private void Efunc()
        {
            Tfunc();
            Afunc();
        }

        private void Afunc()
        {
            if (lex.val.Equals("+"))
            {
                NextLex();
                if (lex.val.Equals("-"))
                    NextLex();
                Tfunc();
                Afunc();
            }
            else if (lex.val.Equals("="))
            {
                NextLex();
                if (lex.val.Equals("-"))
                    NextLex();
                Tfunc();
                Afunc();
            }
            else if (lex.val.Equals("-"))
            {
                NextLex();
                Tfunc();
                Afunc();
            }
            else if (!lex.val.Equals(""))
            {
                if (lex.id.Equals("ERROR")) 
                    throw new Exception(lex.lex + " '" + lex.val + "'");
                return;
            }
        }

        private void Tfunc()
        {
            Ofunc();
            Bfunc();
        }

        private void Bfunc()
        {
            if (lex.val.Equals("*"))
            {
                NextLex();
                if (lex.val.Equals("-"))
                    NextLex();
                Tfunc();
                Afunc();
            }
            else if (lex.val.Equals("/"))
            {
                NextLex();
                if (lex.val.Equals("-"))
                    NextLex();
                Tfunc();
                Afunc();
            }
            else if (!lex.val.Equals(""))
            {
                if (lex.id.Equals("ERROR"))
                    throw new Exception(lex.lex + " '" + lex.val+"'");
                return;
            }
        }

        private void Ofunc()
        {
            if (lex.lex.Equals("Идентификатор"))
            {
                NextLex();
            }
            else if (opPriority.Keys.Contains(lex.val[0]))
                throw new Exception("Лишний символ '" + lex.val + "'");
            else if (lex.id.Equals("ERROR"))
                throw new Exception(lex.lex + " '" + lex.val + "'");
            else
            {
                Efunc();
            }
        }

        private string GetArg(string expr, ref int pos)
        {
            string strNumber = "";

            for (; pos < expr.Length; pos++)
            {
                char num = expr[pos];

                if (Char.IsLetter(num))
                    strNumber += num;
                else
                {
                    pos--;
                    break;
                }
            }

            return strNumber;
        }


        private string ToPostfix()
        {
            string postfixExpr = "";
            Stack<char> stack = new();

            for (int i = 0; i < lexemes.Count; i++)
            {
                Lex lexi = lexemes[i];

                if (lexi.lex == "Идентификатор")
                {
                    postfixExpr += lexi.val + " ";
                }
                else if (opPriority.ContainsKey(lexi.val[0]))
                {
                    char op = lexi.val[0];
                    if (op == '-' && (i == 0 || (i > 1 && opPriority.ContainsKey(lexemes[i - 1].val[0]))))
                        op = '~';

                    while (stack.Count > 0 && (opPriority[stack.Peek()] >= opPriority[op]))
                        postfixExpr += stack.Pop() + " ";
                    stack.Push(op);
                }
            }
            foreach (char op in stack)
                postfixExpr += op + " ";

            return postfixExpr;
        }

        public void PrintTetrads()
        {
            Stack<string> locals = new();
            int counter = 0;

            for (int i = 0; i < postfixForm.Length; i++)
            {
                char c = postfixForm[i];

                if (Char.IsLetter(c))
                {
                    string arg = GetArg(postfixForm, ref i);
                    locals.Push(Convert.ToString(arg));
                }
                else if (opPriority.ContainsKey(c))
                {
                    counter += 1;
                    if (c == '~')
                    {
                        string last = locals.Count > 0 ? locals.Pop() : "";
                        var tetradaminus = new Tetrada('-', last, "", "t" + counter.ToString());
                        tetradas.Add(tetradaminus);
                        locals.Push("t" + counter.ToString());
                    }
                    else if (c == '=')
                    {
                        string equal = locals.Count > 0 ? locals.Pop() : "",
                        result = locals.Count > 0 ? locals.Pop() : "";

                        var tetradaEqual = new Tetrada(c, equal, "", result);
                        tetradas.Add(tetradaEqual);
                        locals.Push("t" + counter.ToString());
                    }
                    else
                    {
                        string second = locals.Count > 0 ? locals.Pop() : "",
                        first = locals.Count > 0 ? locals.Pop() : "";

                        var tetrada = new Tetrada(c, first, second, "t" + counter.ToString());
                        tetradas.Add(tetrada);
                        locals.Push("t" + counter.ToString());
                    }
                }
            }
        }

        private bool IsEnd()
        {
            var index = lexemes.IndexOf(lex);
            if (index + 1 == lexemes.Count)
                return true;
            else return false;
        }

        private bool NextLex()
        {
            var index = lexemes.IndexOf(lex);
            if (index + 1 == lexemes.Count)
                return false;
            lex = lexemes[index + 1];
            return true;
        }
    }
}
