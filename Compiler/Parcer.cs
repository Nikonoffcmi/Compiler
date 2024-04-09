using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class ParseError
    {
        public int idx { get; }
        public String message { get; }
        public String incorrStr {  get; }
        public int line { get; }
        public int start { get;}
        public int end { get; }

        public ParseError(int index, String msg, String incorrStr, int line, int start, int end)
        {
            idx = index;
            message = msg;
            this.incorrStr = incorrStr;
            this.line = line;
            this.start = start;
            this.end = end;
        }

    }

    public class Parser
    {
        private int id;
        private int state;
        private List<Lex> lexemes;
        private Lex lex;
        private string incorrStr;
        private string corrStr;
        private int start;
        private int end;
        private int line = 1;

        private List<ParseError> errors;

        private Dictionary<string, string> Words = new Dictionary<string, string>()
        {
            { "type", "1" },
            { "struct", "2" },
            { "int", "4" },
            { "float", "5" },
            { "string", "6" }
        };

        public List<ParseError> GetErrors()
        {
            return errors;
        }

        public bool Parse(string text)
        {
            var analyzer = new LexicalAnalyzer();
            analyzer.AnalysisText(text);
            errors = new List<ParseError>();
            lexemes = analyzer.Lexemes;
            if (lexemes.Count < 1)
            {
                id++;
                var error = new ParseError(id, "Объявите структуру", "", 1, 1, 1);
                errors.Add(error);
                incorrStr = "";
                return false;
            }
            lex = lexemes[0];
            incorrStr = "";
            state = 1;

            while (state != 15)
            {
                switch (state)
                {
                    case 1:
                        StateKeyWord("type", 2, "Ожидалось ключевое слово type.");
                        break;

                    case 2:
                        StateSpace(" ", 3, "Ожидался пробел.");
                        break;

                    case 3:
                        StateKeyWord("Идентификатор", 4, "Ожидался идентификатор.");
                        break;

                    case 4:
                        StateSpace(" ", 5, "Ожидался пробел.");
                        break;

                    case 5:
                        StateKeyWord("struct", 6, "Ожидалось ключевое слово struct.");
                        break;

                    case 6:
                        StateSpace("{", 7, "Ожидался символ '{'.");
                        break;

                    case 7:
                        StateSpace("<новая строка>", 8, "Ожидался символ <новая строка>.");
                        break;

                    case 8:
                        StateKeyWordDifEnd(["bgnhfjutydrsesfdgbnhfjmkuiluytdrsfg34256yt", " "], [9, 9], "Ожидался идентификатор.");
                        break;

                    case 9:
                        StateKeyWord("Идентификатор", 10, "Ожидался идентификатор.");
                        break;

                    case 10:
                        StateSpace(" ", 11, "Ожидался пробел.");
                        break;

                    case 11:
                        StateKeyWordDif(["int", "string", "float"], 12, "Ожидалось одно из ключевых слов: int, float, string.");
                        break;

                    case 12:
                        StateSpace("<новая строка>", 13, "Ожидался символ <новая строка>.");
                        break;

                    case 13:
                        StateKeyWordDifEnd(["}", " "], [14, 9], "Ожидался символ '}'.");
                        break;

                    default:
                        state = 15;
                        break;
                }
            }

            return true;
        }

        private void handleError(string msg, string str, int line, int start, int end)
        {
            id++;
            var error1 = new ParseError(id, msg, str, line, start, end);
            errors.Add(error1);
        }
        private bool IsEnd() 
        {
            var index = lexemes.IndexOf(lex);
            if (index + 1 == lexemes.Count)
                return true;
            else return false;
        }
        private void StateKeyWord(string value, int nextState, string msg)
        {
            string incorr = "";
            int linenew = lex.line;
            int st = lex.start;
            int endnew = lex.end;
            int index = -1;

            if (value.Equals("Идентификатор"))
            {
                if (IsWord(lex.val) && !Words.Keys.Contains(lex.val))
                {
                    while (!Words.Keys.Contains(lexemes[lexemes.IndexOf(lex) + 1].val) && !lexemes[lexemes.IndexOf(lex) + 1].val.Equals(" ") && (IsWord(lexemes[lexemes.IndexOf(lex) + 1].val) || char.IsDigit(lexemes[lexemes.IndexOf(lex) + 1].val[0])))
                    {
                        NextLex();
                    }
                    state = nextState;
                }
                else
                {
                    incorr += lex.val;
                    endnew = lex.end;
                }
            }
            else
            {
                while(!lexemes[lexemes.IndexOf(lex)].val.Equals(" "))
                {
                    index = lex.val.IndexOf(value);
                    if (nextState == 6 && lexemes[lexemes.IndexOf(lex)].val.Equals("{"))
                        break;
                    if (nextState == 2 && lexemes[lexemes.IndexOf(lex)].val.Equals(" "))
                        break;
                    if (index != -1)
                    {
                        state = nextState;
                        break;
                    }
                    else
                    {
                        incorr += lex.val;
                        endnew = lex.end;
                        NextLex();
                    }
                }
            }

            if (state != nextState)
            {
                handleError(msg, incorr, linenew, st, endnew);
                if (value.Equals("Идентификатор"))
                    state = nextState;
                else if (lex.val.Equals("{") && nextState == 6)
                    state = nextState;
                else
                {
                    state = nextState;
                }
            }
            else
            {
                if (incorr != "")
                    handleError("Ошибочный фрагмент", incorr, linenew, st, endnew);
                NextLex();
            }

        }

        private void StateSpace(string value, int nextState, string msg)
        {
            string incorr = "";
            int linenew = lex.line;
            int st = lex.start;
            int endnew = lex.end;
            int index = -1;

            index = lex.val.IndexOf(value);
            if (lex.val.Equals(value))
            {
                state = nextState;
                while (lex.val.Equals(" "))
                {
                    if (IsEnd())
                    {
                        break;
                    }
                    if (!lexemes[lexemes.IndexOf(lex) + 1].val.Equals(" "))
                        break;
                    NextLex();
                }
            }
            else
            {
                while (!lexemes[lexemes.IndexOf(lex)].val.Equals(value))
                {
                    if (nextState == 7 && lexemes[lexemes.IndexOf(lex)].val.Equals("<новая строка>"))
                        break;
                    if (nextState == 8 && lexemes[lexemes.IndexOf(lex)].val.Equals(" "))
                        break;
                    if (nextState == 13 && lexemes[lexemes.IndexOf(lex)].val.Equals(" "))
                        break;
                    if (nextState == 9 && IsWord(lexemes[lexemes.IndexOf(lex)].val))
                        break;
                    if (nextState == 13 && lex.val.Equals("}"))
                        break;
                    if (value == " " && IsWord(lexemes[lexemes.IndexOf(lex)].val))
                        break;
                    if (value.Contains(lex.val) && !lex.val.Equals(" "))
                    {
                        state = nextState;
                        break;
                    }
                    else
                    {
                        incorr += lex.val;
                        endnew = lex.end;
                        NextLex();
                        if (IsEnd())
                            break;
                    }
                }
                if (value.Equals(lex.val))
                {
                    state = nextState;
                }
                else
                {
                    incorr += lex.val;
                    endnew = lex.end;
                }
            }

            if (state != nextState)
            {
                handleError(msg, "", linenew, st, endnew);
                state = nextState;
            }
            else
            {
                if (incorr != "")
                    handleError("Ошибочный фрагмент", incorr, linenew, st, endnew);
                NextLex();
            }

        }

        private void StateKeyWordDif(string[] value, int nextState, string msg)
        {
            string incorr = "";
            int linenew = lex.line;
            int st = lex.start;
            int endnew = lex.end;
            int index = -1;

            if (value.Contains(lex.val))
            {
                state = nextState;
            }
            else
            {
                while (!lexemes[lexemes.IndexOf(lex)].val.Equals("<новая строка>"))
                {
                    if (value.Contains(lex.val))
                    {
                        state = nextState;
                        break;
                    }
                    else
                    {
                        incorr += lex.val;
                        endnew = lex.end;
                        NextLex();
                        if (IsEnd())
                            break;
                    }
                }
            }

            if (state != nextState)
            {
                handleError(msg, incorr, linenew, st, endnew);
                if (value.Equals("Идентификатор"))
                    state = nextState;
                else if (lex.val.Equals("{") && nextState == 6)
                    state = nextState;
                else if (lex.val.Equals("<новая строка>"))
                    state = nextState;
                else
                {
                    state = nextState;
                    NextLex();
                }
            }
            else
            {
                if (incorr != "")
                    handleError("Ошибочный фрагмент", incorr, linenew, st, endnew);
                NextLex();
            }

        }

        private void StateKeyWordDifEnd(string[] value, int[] nextState, string msg)
        {
            string incorr = "";
            int linenew = lex.line;
            int st = lex.start;
            int endnew = lex.end;
            int index = -1;

            if (value.Contains(lex.val))
            {
                if (lex.val.Equals(value[0]))
                    state = nextState[0];
                else if (lex.val.Equals(value[1])) 
                { 
                    state = nextState[1];
                    while (lex.val.Equals(" "))
                    {
                        if (IsEnd())
                        {
                            break;
                        }
                        if (!lexemes[lexemes.IndexOf(lex) + 1].val.Equals(" "))
                            break;
                        NextLex();
                    }
                }
            }
            else
            {
                incorr += lex.val;
                endnew = lex.end;
            }

            if (!nextState.Contains(state))
            {
                if (IsWord(lex.val))
                {
                    if (IsEnd())
                    {
                        handleError("Ожидался символ '}'.", "", linenew, st, endnew);
                        state = 15;
                    }
                    else
                    {
                        state = nextState[1];
                    }
                }
                else
                {
                    handleError(msg, "", linenew, st, endnew);
                    NextLex();
                    if (IsEnd())
                    {
                        state = 15;
                    }
                }
            }
            else
            {
                if (incorr != "")
                    handleError("Ошибочный фрагмент", incorr, linenew, st, endnew);
                NextLex();
            }

        }



        private bool NextLex()
        {
            var index = lexemes.IndexOf(lex);
            if (index + 1 == lexemes.Count)
                return false;   
            lex = lexemes[index + 1];
            return true;
        }

        private bool IsWord(string str)
        {
            foreach (var c in str)
            {
                if (!isLetter(c))
                {
                    return false;
                }
            }
            return true;
        }

        private bool isLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

    }

}
