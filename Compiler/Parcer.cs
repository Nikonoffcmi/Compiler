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
                var error = new ParseError(id, "Ожидался буквенный символ", "", 1, 1, 1);
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

        private void handleError(string msg, string str, int line, int start, int end)
        {
            id++;
            var error1 = new ParseError(id, msg, str, line, start, end);
            errors.Add(error1);
        }
        private bool NextOrEnd() 
        {
            if (!NextLex())
            {
                state = 15;
                return false;
            }
            else
            { return true; }
        }

        private void state1()
        {
            if (lex.val == "<новая строка>" || lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, incorrStr.Length);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, incorrStr.Length);
                    incorrStr = "";
                }

                state = 2;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;

                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, incorrStr.Length);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state2()
        {
            if (!IsWord(lex.val))
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }
                incorrStr += lex.val;
                end = lex.end;

            }

            while (NextOrEnd() & !lex.val.Equals(" "))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                    {
                        start = lex.start;
                        line = lex.line;
                    }
                    incorrStr += lex.val;
                    end = lex.end;

                }
                else
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, end);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, end);
                incorrStr = "";
            }

            if (corrStr.Equals("type"))
            {
                if (!NextOrEnd() & !lex.val.Equals(" "))
                {
                    handleError("Ожидался символ <пробел>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 3;
                corrStr = "";
            }
            else
            {
                handleError("Ожидалось ключевое слово type.", "", lex.line, lex.end, lex.end);
                if (!NextOrEnd() & !lex.val.Equals(" "))
                {
                    handleError("Ожидался символ <пробел>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 3;
                corrStr = "";
            }
        }

        private void state3()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length -1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 4;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;

                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state4()
        {
            if (!IsWord(lex.val))
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" "))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                    {
                        start = lex.start;
                        line = lex.line;
                    }
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                incorrStr = "";
            }

            if (corrStr.Equals("type") || corrStr.Equals("struct") || corrStr.Equals("int") || corrStr.Equals("float") || corrStr.Equals("string"))
            {

                handleError("Имя переменной не может быть ключивым словом", corrStr, lex.line, lex.end, lex.end);
                corrStr = "";

                if (!NextOrEnd() & !lex.val.Equals(" "))
                {
                    handleError("Ожидался символ <пробел>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 5;
            }
            else
            {
                if (!NextOrEnd() & !lex.val.Equals(" "))
                {
                    handleError("Ожидался символ <пробел>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 5;
                corrStr = "";
            }
        }

        private void state5()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 6;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state6()
        {
            if (!IsWord(lex.val))
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" ") & !lex.val.Equals("{"))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                    {
                        start = lex.start;
                        line = lex.line;
                    }
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                incorrStr = "";
            }

            if (corrStr.Equals("struct"))
            {
                if (!lex.val.Equals(" ") & !lex.val.Equals("{"))
                {
                    handleError("Ожидался символ <пробел> либо '{'", "", lex.line, lex.end, lex.end);
                }
                else if (lex.val.Equals(" "))
                    state = 7;
                else if (lex.val.Equals("{"))
                {
                    state = 8;
                    if (!NextOrEnd())
                    {
                        handleError("Ожидался символ <новая строка>", "", lex.line, lex.start, lex.end);
                        incorrStr = "";
                    }
                }
                corrStr = "";
            }
            else
            {
                handleError("Ожидалось ключевое слово struct.", "", lex.line, lex.end, lex.end);

                if (!lex.val.Equals(" ") & !lex.val.Equals("{"))
                {
                    handleError("Ожидался символ <пробел> либо '{'", "", lex.line, lex.end, lex.end);
                }
                else if (lex.val.Equals(" "))
                    state = 7;
                else if (lex.val.Equals("{"))
                {
                    state = 8;

                    if (!NextOrEnd())
                    {
                        handleError("Ожидался символ <новая строка>", "", lex.line, lex.start, lex.end);
                        incorrStr = "";
                    }
                }
                corrStr = "";
            }
        }

        private void state7()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ '{'", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (lex.val == "{")
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 8;
                if (!NextOrEnd())
                {
                    handleError("Ожидался символ <новая строка>", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ '{'", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state8()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <новая строка>", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (lex.val == "<новая строка>")
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 9;
                if (!NextOrEnd())
                {
                    handleError("Ожидался символ <табуляция>", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;

                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <новая строка>", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state9()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <табуляция>", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (lex.val == "<табуляция>")
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 10; 
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;

                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <табуляция>", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state10()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 11;
                corrStr = "";
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;

                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state11()
        {
            if (!IsWord(lex.val))
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals(" "))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                    {
                        start = lex.start;
                        line = lex.line;
                    }
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                incorrStr = "";
            }

            if (corrStr.Equals("type") || corrStr.Equals("struct") || corrStr.Equals("int") || corrStr.Equals("float") || corrStr.Equals("string"))
            {

                handleError("Имя переменной не может быть ключивым словом", corrStr, lex.line, lex.end, lex.end);
                corrStr = "";

                if (!NextOrEnd() & !lex.val.Equals(" "))
                {
                    handleError("Ожидался символ <пробел>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 12;
            }
            else
            {
                if (!NextOrEnd() & !lex.val.Equals(" "))
                {
                    handleError("Ожидался символ <пробел>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 12;
                corrStr = "";
            }
        }

        private void state12()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (char.IsLetter(lex.val[0]))
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 13;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }

                incorrStr += lex.val;
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
        }

        private void state13()
        {
            if (!IsWord(lex.val))
            {
                if (incorrStr == "")
                {
                    start = lex.start;
                    line = lex.line;
                }
                incorrStr += lex.val;
            }

            while (NextOrEnd() & !lex.val.Equals("<новая строка>"))
            {
                if (!IsWord(lex.val))
                {
                    if (incorrStr == "")
                    {
                        start = lex.start;
                        line = lex.line;
                    }
                    incorrStr += lex.val;
                }
                else
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                        incorrStr = "";
                    }
                    corrStr += lex.val;
                }
            }

            if (incorrStr != "")
            {
                handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, lex.end - lex.val.Length);
                incorrStr = "";
            }

            if (corrStr.Equals("int") || corrStr.Equals("float") || corrStr.Equals("string"))
            {
                if (!lex.val.Equals("<новая строка>"))
                {
                    handleError("Ожидался символ <новая строка>", "", lex.line, lex.end, lex.end);
                }
                else 
                    state = 14;
                
                corrStr = "";
            }
            else
            {
                handleError("Ожидалось одно из ключевых слов: int, float, string.", "", lex.line, lex.end, lex.end);

                if (!lex.val.Equals("<новая строка>"))
                {
                    handleError("Ожидался символ <новая строка>", "", lex.line, lex.end, lex.end);
                }
                else
                    state = 14;

                corrStr = "";
            }
        }

        private void state14()
        {
            if (lex.val == " ")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <табуляция> либо '}'", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (lex.val == "<новая строка>")
            {
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <табуляция> либо '}'", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
            }
            else if (lex.val == "<табуляция>")
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }
                state = 10; 
                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался буквенный символ", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
                corrStr += lex.val;
            }
            else if (lex.val == "}")
            {
                if (incorrStr != "")
                {
                    handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, line, start, start + incorrStr.Length - 1);
                    incorrStr = "";
                }

                if (NextOrEnd())
                {
                    while ((lex.val == " " | lex.val == "<новая строка>"))
                    {
                        if (state == 15)
                            break;
                        NextOrEnd();
                    }
                    if (lex.val != " " && lex.val != "<новая строка>")
                        state = 1;
                }
                else
                    state = 15;
                corrStr += lex.val;
            }
            else
            {
                if (incorrStr == "")
                    start = lex.start;

                incorrStr += lex.val;

                if (!NextOrEnd())
                {
                    if (incorrStr != "")
                    {
                        handleError("Неожиданный символ. Ошибочный фрагмент", incorrStr, lex.line, start, start + incorrStr.Length - 1);
                        incorrStr = "";
                    }

                    handleError("Ожидался символ <табуляция> либо '}'", "", lex.line, lex.start, lex.end);
                    incorrStr = "";
                }
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
