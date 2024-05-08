using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public enum LexemeType
    {
        If,
        Else,
        Id,
        Operator,
        Assign,
        Invalid,
        Semicolon,
        Exp
    }

    class Lex
    {
        public  LexemeType lex {  get; set; }
        public  string val { get; set; }
        public int start {  get; set; }
        public int end {  get; set; }

        public Lex(LexemeType lex, string val,  int start, int end)
        {
            this.lex = lex;
            this.val = val;
            this.start = start;
            this.end = end;
        }
    }
    internal class LexicalAnalyzer
    {

        private Dictionary<char, string> ASSIGN = new Dictionary<char, string>()
        {
            { '=', "1" },
            { '<', "2" },
            { '>', "3" },
            { '!', "4" },
            { ';', "5" },

        };

        public List<Lex> Lexemes = new List<Lex>();
        private string buf = ""; 

        public void AnalysisText(string AllTextProgram)
        {
            int i;
            string value;

            Lexemes.Clear();

            for (i = 0; i < AllTextProgram.Length; i++)
            {
                value = string.Empty + AllTextProgram[i];

                if (char.IsLetter(AllTextProgram[i]))
                {
                    int startIndex = i;

                    while ((i + 1) < AllTextProgram.Length && (char.IsLetter(AllTextProgram[i + 1]) || char.IsDigit(AllTextProgram[i + 1])))
                    {
                        i++;
                        value += AllTextProgram[i];
                    }

                    switch (value)
                    {
                        case "IF":
                            Lexemes.Add(new Lex(LexemeType.If, value, startIndex + 1, i + 1));
                            break;
                        case "ELSE":
                            Lexemes.Add(new Lex(LexemeType.Else, value, startIndex + 1, i + 1));
                            break;
                        case "TRUE":
                            Lexemes.Add(new Lex(LexemeType.Exp, value, startIndex + 1, i + 1));
                            break;
                        case "FALSE":
                            Lexemes.Add(new Lex(LexemeType.Exp, value, startIndex + 1, i + 1));
                            break;
                        case "OR":
                            Lexemes.Add(new Lex(LexemeType.Operator, value, startIndex + 1, i + 1));
                            break;
                        case "AND":
                            Lexemes.Add(new Lex(LexemeType.Operator, value, startIndex + 1, i + 1));
                            break;
                        case "NOT":
                            Lexemes.Add(new Lex(LexemeType.Operator, value, startIndex + 1, i + 1));
                            break;
                        default:
                            Lexemes.Add(new Lex(LexemeType.Id, value, startIndex + 1, i + 1));
                            break;
                    }
                }
                else
                {
                    switch (AllTextProgram[i])
                    {
                        case '\t':
                        case ' ':
                            break;
                        case (char)13:
                            if ((i + 1) < AllTextProgram.Length && AllTextProgram[i + 1] == (char)10)
                            {
                                i++;
                                value = "\\n";
                            }
                            break;
                        case '>':
                        case '<':
                        case '!':
                            if ((i + 1) < AllTextProgram.Length && AllTextProgram[i + 1] == '=')
                            {
                                i++;
                                value += AllTextProgram[i];
                                Lexemes.Add(new Lex(LexemeType.Assign, value, i, i + 1));
                            }
                            else
                            {
                                if (AllTextProgram[i] == '!')
                                {
                                    Lexemes.Add(new Lex(LexemeType.Invalid, value, i + 1, i + 1));
                                }
                                else
                                {
                                    Lexemes.Add(new Lex(LexemeType.Assign, value, i + 1, i + 1));
                                }
                            }
                            break;
                        case '=':
                            if ((i + 1) < AllTextProgram.Length && AllTextProgram[i + 1] == '=')
                            {
                                i++;
                                value += AllTextProgram[i];
                                Lexemes.Add(new Lex(LexemeType.Assign, value, i, i + 1));
                            }
                            else
                            {
                                Lexemes.Add(new Lex(LexemeType.Invalid, value, i, i + 1));
                            }
                            break;
                        case ';':
                            Lexemes.Add(new Lex(LexemeType.Semicolon, value, i + 1, i + 1));
                            break;
                        default:
                            Lexemes.Add(new Lex(LexemeType.Invalid, value, i + 1, i + 1));
                            break;
                    }
                }
            }

        }
    }
}
