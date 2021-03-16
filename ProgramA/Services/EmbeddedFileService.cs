using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProgramA.Services
{
    public class EmbeddedFileService : IEmbeddedFileService
    {
        private int lineNumberA;
        private int lineNumberB;

        public string GetHandFromFileA()
        {
            var allLines = GetAllLinesFromEmbeddedFile("ProgramA.Assets.HH20161111 T1724073465 No Limit Hold'em $0,44 + $0,06.txt");

            var linesToReturn = new List<string>();

            foreach (var line in allLines.Skip(lineNumberA))
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    lineNumberA += 3;
                    break;
                }

                linesToReturn.Add(line);
                lineNumberA++;
            }

            return string.Join("\n", linesToReturn);
        } 
        
        public string GetHandFromFileB()
        {
            var allLines = GetAllLinesFromEmbeddedFile("ProgramA.Assets.HH20190522 CASHID-G10660770T20 TN-Agoura Hills.txt");

            var linesToReturn = new List<string>();

            foreach (var line in allLines.Skip(lineNumberB))
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    lineNumberB++;
                    break;
                }

                linesToReturn.Add(line);
                lineNumberB++;
            }

            return string.Join("\n", linesToReturn);
        }

        public void ResetSource()
        {
            lineNumberA = 0;
            lineNumberB = 0;

        }

        private static List<string> GetAllLinesFromEmbeddedFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "ProgramA.Assets.HH20161111 T1724073465 No Limit Hold'em $0,44 + $0,06.txt";

            var allLines = new List<string>();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        allLines.Add(line);
                    }
                }
            }

            return allLines;
        }
    }
}
