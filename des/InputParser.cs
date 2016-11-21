using System;
using System.Collections.Generic;
using System.Linq;

namespace des
{
    public class InputParser
    {
        public List<Tuple<int, List<int>>> Members { get; private set; }

        public int FirstSenderId { get; private set; }

        public int FirstReceiverId { get; private set; }

        public void ParseTopology(string filePath)
        {
            var topology = GetFileContent(filePath);
            ParseMembers(topology);
        }

        public void ParseScenario(string filePath)
        {
            var scenario = GetFileContent(filePath);
            var columns = scenario.Split(' ');
            FirstSenderId = int.Parse(columns[0]);
            FirstReceiverId = int.Parse(columns[1]);
        }

        private string GetFileContent(string path)
        {
            return System.IO.File.ReadAllText(path);
        }

        private void ParseMembers(string topology)
        {
            Members = new List<Tuple<int, List<int>>>();

            var lines = SplitToLines(topology).Skip(1);
            foreach (var line in lines)
            {
                Members.Add(ParseMember(line));
            }
        }

        private Tuple<int, List<int>> ParseMember(string line)
        {
            var columns = line.Split(' ');
            var person = columns.Take(1).Select(p => int.Parse(p)).Single();
            var contacts = columns.Skip(1).Select(c => int.Parse(c)).ToList();

            return new Tuple<int, List<int>>(person, contacts);
        }

        private IEnumerable<string> SplitToLines(string input)
        {
            if (input == null) yield break;

            using (System.IO.StringReader reader = new System.IO.StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
