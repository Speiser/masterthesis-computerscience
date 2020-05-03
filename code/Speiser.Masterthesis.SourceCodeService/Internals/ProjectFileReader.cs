using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.SourceCodeService.Internals
{
    internal class ProjectFileReader : IProjectFileReader
    {
        public ProjectFile ParseProjectFile(string path)
        {
            var lines = File.ReadAllLines(path);

            // Read all values from PackageReference and ProjectReference
            var libraries = new List<string>();
            foreach (var line in lines)
            {
                if (line.Contains("<PackageReference") || line.Contains("<ProjectReference"))
                {
                    libraries.Add(ExtractValue("Include", line));
                }
            }

            var sdk = lines[0].Replace("<Project Sdk=\"", string.Empty).Replace("\">", string.Empty);

            return new ProjectFile
            {
                Sdk = sdk,
                FilePath = path,
                Libraries = libraries
            };
        }

        private static string ExtractValue(string attrName, string line)
        {
            var split = line.Split(new[] { $"{attrName}=\"" }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                throw new ArgumentException($"Could not find {attrName} (or multiple)");
            return split[1].Split('"').First();
        }
    }
}
