using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Lex
    {
        public  string id {  get; set; }
        public  string lex {  get; set; }
        public  string val { get; set; }
        public int line { get; set; }
        public int start {  get; set; }
        public int end {  get; set; }

        public Lex(string id, string lex, string val,  int start, int end, int line)
        {
            this.id = id;
            this.lex = lex;
            this.val = val;
            this.start = start;
            this.end = end;
            this.line = line;
        }
    }
    internal class LexicalAnalyzer
    {
        private Dictionary<string, string> Words = new Dictionary<string, string>()
        {
            { "type", "1" },
            { "struct", "2" },
            { "int", "4" },
            { "float", "5" },
            { "string", "6" }
        }; 
        private Dictionary<char, string> Separators = new Dictionary<char, string>()
        {
            { ' ', "7" },
            { '\n', "8" },
            { '\r', "8" },
            { '{', "9" },
            { '}', "10" },
            { '\t', "11" }
        };
        public List<Lex> Lexemes = new List<Lex>();
        private string buf = ""; 

        public void AnalysisText(string AllTextProgram)
        {
            var lines = AllTextProgram.Split('\n');
            int lineCount = 1;
            int start = 0, end = 0;
            foreach (var line in lines)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (Char.IsLetter(c))
                    {
                        if (buf == "")
                            start = i + 1;
                        buf += c;                        
                    }
                    else if (Separators.Keys.Contains(c))
                    {
                        end = i;
                        if (buf != "")
                            Result(buf, lineCount, start, end);
                        buf = c.ToString();
                        start = end = i + 1;
                        Result(buf, lineCount, start, end);
                        buf = "";
                    }
                    else if (buf != "")
                    {
                        end = i;
                        Result(buf, lineCount, start, end);
                        start = end = i + 1;
                        buf = "";
                        var lex = new Lex("ERROR" , "Недопустимый символ", buf + c.ToString(), start, end, lineCount );
                        Lexemes.Add(lex);
                        buf = "";
                    }
                    else
                    {
                        start = end = i + 1;
                        var lex = new Lex("ERROR", "Недопустимый символ", c.ToString(), start, end, lineCount);
                        Lexemes.Add(lex);
                    }
                }
                if (buf != "")
                {
                    if (buf != "")
                        Result(buf, lineCount, start, line.Length);
                    buf = "";
                }
                lineCount++; 
            }
        }

        private void Result(string temp, int line, int start, int end)
        {
            if (Words.Keys.Contains(temp))
            {
                var lex = new Lex(Words[temp], "Ключевое слово", temp, start, end, line);
                Lexemes.Add(lex);
                return;
            }
            else if (Separators.Keys.Contains(temp[0]))
            {
                string lex = "";
                string val = "";
                if (temp[0] == '\n')
                    return;
                switch (Separators[temp[0]])
                {
                    case "7":
                        lex = "Разделитель";
                        val = " ";
                        break;
                    case "8":
                        lex = "Переход на новую строку";
                        val = "<новая строка>";
                        break;
                    case "9":
                        lex = "Начало блока данных";
                        val = "{";
                        break;
                    case "10":
                        lex = "Конец блока данных";
                        val = "}";
                        break;
                    case "11":
                        lex = "Табуляция";
                        val = "<табуляция>";
                        break;
                    default:
                        break;
                }
                var lexem = new Lex(Separators[temp[0]], lex, val, start, end, line);
                Lexemes.Add(lexem);
                return;
            }
            else
            {
                var lex = new Lex("3", "Идентификатор", temp, start, end, line);
                Lexemes.Add(lex);
                return;
            }

        }
    }
}
