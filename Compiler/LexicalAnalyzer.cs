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
        public  int id {  get; set; }
        public  string lex {  get; set; }
        public  string val { get; set; }
        public int start {  get; set; }
        public int end { get; set; }

        public Lex(int id, string lex, string val, int start, int end)
        {
            this.id = id;
            this.lex = lex;
            this.val = val;
            this.start = start;
            this.end = end;
        }
    }
    internal class LexicalAnalyzer
    {
        private Dictionary<string, int> Words = new Dictionary<string, int>()
        {
            { "type", 1 },
            { "struct", 2 },
            { "int", 4 },
            { "float32", 5 },
            { "string", 6 }
        }; 
        private Dictionary<char, int> Separators = new Dictionary<char, int>()
        {
            { ' ', 7 },
            { '\n', 8 },
            { '\r', 8 },
            { '{', 9 },
            { '}', 10 },
            { '\t', 11 }
        };
        public List<Lex> Lexemes = new List<Lex>();
        private string buf = ""; 

        public void AnalysisText(string AllTextProgram)
        {
            for (int i = 0; i < AllTextProgram.Length; i++)
            {
                char c = AllTextProgram[i];
                if (Char.IsLetter(c))
                {
                    buf += c;
                }
                else if (Separators.Keys.Contains(c) && buf != "")
                {
                    Result(buf);
                    buf = c.ToString();
                    Result(buf);
                    buf = "";
                }
                else if (buf != "")
                {
                    Result(buf);
                    buf = "";
                }
                else
                {
                    Result(c.ToString());
                }
            }
        }

        private void Result(string temp)
        {
            if (Words.Keys.Contains(temp))
            {
                var lex = new Lex(Words[temp], "ключевое слово", temp, 0, 1);
                Lexemes.Add(lex);
                return;
            }
            else if (Separators.Keys.Contains(temp[0]))
            {
                string lex = "";
                string val = "";
                if (temp[0] == '\r' || temp[0] == '\t')
                    return;
                switch (Separators[temp[0]])
                {
                    case 7:
                        lex = "разделитель";
                        val = "(пробел)";
                        break;
                    case 8:
                        lex = "переход на новую строку";
                        val = "\\n";
                        break;
                    case 9:
                        lex = "начало блока данных";
                        val = "{";
                        break;
                    case 10:
                        lex = "конец блока данных";
                        val = "}";
                        break;
                    default:
                        break;
                }
                var lexem = new Lex(Separators[temp[0]], lex, val, 0, 1);
                Lexemes.Add(lexem);
                return;
            }
            else
            {
                var lex = new Lex(0, "Error недопустимый символ", temp, 0, 1);
                Lexemes.Add(lex);
                return;
            }

        }
    }
}
