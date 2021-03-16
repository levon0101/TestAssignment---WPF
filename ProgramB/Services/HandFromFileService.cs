using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ProgramB.Services
{
    public class HandFromFileService : IHandFromFileService
    {
        private int lineNumberA;
        private int lineNumberB;

        public (string Id, string Hand) GetHandFromFileA(string filePath)
        {
            var allLines = File.ReadAllLines(filePath).Skip(lineNumberA);

            var linesToReturn = new List<string>();

            foreach (var line in allLines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    lineNumberA += 3;
                    break;
                }

                linesToReturn.Add(line);
                lineNumberA++;
            }

            if (linesToReturn.Count > 0)
            {
                var startIndexId = linesToReturn[0].IndexOf("#");
                var endIndexId = linesToReturn[0].IndexOf(":");


                var stringId = linesToReturn[0].Substring(++startIndexId, endIndexId - startIndexId);
                 
                return (stringId, string.Join("\n", linesToReturn));
            }

            return ("", "");
        }


        public (string Id, string Hand) GetHandFromFileB(string filePath)
        {
            var allLines = File.ReadAllLines(filePath).Skip(lineNumberB);

            var linesToReturn = new List<string>();

            foreach (var line in allLines)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    lineNumberB += 3;
                    break;
                }

                linesToReturn.Add(line);
                lineNumberB++;
            }

            if (linesToReturn.Count > 0)
            {
                var startIndexId = linesToReturn[0].IndexOf("#");
                var endIndexId = linesToReturn[0].IndexOf(' ', startIndexId);


                var stringId = endIndexId - startIndexId<=0 ? "" : linesToReturn[0].Substring(++startIndexId, endIndexId - startIndexId);
                 
                return (stringId, string.Join("\n", linesToReturn));
            }

            return ("", "");
        }

        public void ResetSource()
        {
            lineNumberA = 0;
            lineNumberB = 0;
        }
    }
}
