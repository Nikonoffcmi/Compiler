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

        private List<ParseError> errors;

        public List<ParseError> GetErrors()
        {
            return errors;
        }

        public bool Parse(string text)
        {
            var analyzer = new LexicalAnalyzer();
            analyzer.AnalysisText(text);
            lexemes = analyzer.Lexemes;
            lex = lexemes[0];
            state = 1;
            incorrStr = "";
            errors = new List<ParseError>();

            while (state != 15)
            {
                switch (state)
                {
                    case 1:
                        state1();
                        break;

                    case 2:
                        state2();
                        break;

                    case 3:
                        state3();
                        break;

                    case 4:
                        state4();
                        break;

                    case 5:
                        state5();
                        break;

                    case 6:
                        state6();
                        break;

                    case 7:
                        state7();
                        break;

                    case 8:
                        state8();
                        break;

                    case 9:
                        state9();
                        break;

                    case 10:
                        state10();
                        break;

                    case 11:
                        state11();
                        break;

                    case 12:
                        state12();
                        break;
                    case 13:
                        state13();
                        break;
                    case 14:
                        state14();
                        break;


                }
            }

            return true;
        }



        private void handleError()
        {
            

        }
        private bool NextOrEnd() 
        {
            if (!NextLex())
            {
                state = 13;
                return false;
            }
            else
            { return true; }
        }

        private void state1()
        {
            if (lex.val == "<новая строка>")
            {
                NextOrEnd();
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 2;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state2()
        {
            if (!IsWord(lex.val))
            {
                start = lex.start;
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" "))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                        start = lex.start;
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        id++;
                        var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                        errors.Add(error1);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                id++;
                var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                errors.Add(error1);
                incorrStr = "";
            }

            if (corrStr.Equals("type"))
            {
                state = 3;
            }
            else
            {
                start = lex.start;
                id++;
                var error = new ParseError(id, "Ожидалось ключевое слово type.", lex.val, lex.line, start, lex.end);
                errors.Add(error);
                state = 3;
                corrStr = "";
            }
        }

        private void state3()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 4;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state4()
        {
            if (!IsWord(lex.val))
            {
                start = lex.start;
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" "))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                        start = lex.start;
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        id++;
                        var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                        errors.Add(error1);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                id++;
                var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                errors.Add(error1);
                incorrStr = "";
            }

            state = 5;
            corrStr = "";
        }

        private void state5()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 6;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state6()
        {
            if (!IsWord(lex.val))
            {
                start = lex.start;
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" ") & !lex.val.Equals("{"))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                        start = lex.start;
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        id++;
                        var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                        errors.Add(error1);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                id++;
                var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                errors.Add(error1);
                incorrStr = "";
            }

            if (corrStr.Equals("struct"))
            {
                if (lex.val == " ")
                    state = 7;
                else
                {
                    state = 8;
                    NextOrEnd();
                }
            }
            else
            {
                start = lex.start;
                id++;
                var error = new ParseError(id, "Ожидалось ключевое слово struct.", lex.val, lex.line, start, lex.end);
                errors.Add(error);
                if (lex.val == " ")
                    state = 7;
                else
                {
                    state = 8;
                    NextOrEnd();
                }
                corrStr = "";
            }
        }

        private void state7()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (lex.val == "{")
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 8;
                NextOrEnd();
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state8()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (lex.val == "<новая строка>")
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 9;
                NextOrEnd();
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state9()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (lex.val == "<табуляция>")
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 10;
                NextOrEnd();
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state10()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 11;
                corrStr = "";
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state11()
        {
            if (!IsWord(lex.val))
            {
                start = lex.start;
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" "))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                        start = lex.start;
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        id++;
                        var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                        errors.Add(error1);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                id++;
                var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                errors.Add(error1);
                incorrStr = "";
            }

            state = 12;
            corrStr = "";
        }

        private void state12()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 13;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
            }
        }

        private void state13()
        {
            if (!IsWord(lex.val))
            {
                start = lex.start;
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" ") & !lex.val.Equals("<новая строка>"))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                        start = lex.start;
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        id++;
                        var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                        errors.Add(error1);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                id++;
                var error1 = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, lex.end - lex.val.Length);
                errors.Add(error1);
                incorrStr = "";
            }

            if (corrStr.Equals("int") || corrStr.Equals("float") || corrStr.Equals("string"))
            {
                state = 14;
            }
            else
            {
                start = lex.start;
                id++;
                var error = new ParseError(id, "Ожидалось одно из ключевых слов: int, float, string.", lex.val, lex.line, start, lex.end);
                errors.Add(error);
                state = 14;
                corrStr = "";
            }
        }

        private void state14()
        {
            if (lex.val == " ")
            {
                NextOrEnd();
            }
            else if (lex.val == "<новая строка>")
            {
                NextOrEnd();
            }
            else if (lex.val == "<табуляция>")
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 10;
                NextOrEnd();
                corrStr += lex.val;
            }
            else if (lex.val == "}")
            {
                if (incorrStr != "")
                {
                    id++;
                    var error = new ParseError(id, "Неожиданный символ", incorrStr, lex.line, start, incorrStr.Length);
                    errors.Add(error);
                    incorrStr = "";
                }
                state = 15;
                NextOrEnd();
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;
                NextOrEnd();
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
                if (!Char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }
    }

}
