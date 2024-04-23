using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Compiler
{
    public class DGRegex
    {
        public string str {  get; }
        public int line { get; }
        public DGRegex(string str, int line)
        {
            this.str = str;
            this.line = line;
        }
    }
    internal class RegexTask
    {
        public List<DGRegex> regexList;
        public RegexTask()
        {
            regexList = new List<DGRegex>();
        }

        public void task(string text, int taskNumber)
        {
            string pattern = "";
            if (taskNumber == 1)
                pattern = @"(?=.*[а-яё])(?=.*[А-ЯЁ])(?=.*\d)(?=.*[@$!%*?&])[А-ЯЁа-яё\d@$!%*?&]{1,}";
            else if (taskNumber == 2)
                pattern = @"5[1-5][0-9]{14}|(222[1-9]|22[3-9]\\d|2[3-6]\\d{2}|27[0-1]\\d|2720)[0-9]{12}";
            else if (taskNumber == 3)
                pattern = @"(?:(?:29([-./])02(?:\1)(?:(?:(?:1[6-9]|20)(?:04|08|[2468][048]|[13579][26]))|(?:1600|2[048]00)))|(?:(?:(?:0[1-9]|1\d|2[0-8])([-./])(?:0[1-9]|1[0-2]))|(?:29|30)([-./])(?:0(?:1|[3-9])|(?:1[0-2]))|31([-./])(0[13578]|1[02]))(?:\2|\3|\4)(?:1[6-9]|2\d)\d\d)";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                regexList.Add(new DGRegex(match.Value, match.Index));
            }

        }
    }
}
