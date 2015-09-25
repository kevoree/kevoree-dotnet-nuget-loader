using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.NugetLoader
{
    public class MEPDirectoryManager
    {
        private string basePath;

        public MEPDirectoryManager(string basePath)
        {
            this.basePath = basePath;
        }

        public string getCachePath(string packageName, string packageVersion)
        {
            return this.getPath(basePath, packageName, packageVersion, "shadow");
        }

        public string getPluginPath(string packageName, string packageVersion)
        {
            return this.getPath(basePath, packageName, packageVersion, "plugin");
        }

        private string getPath(string basePath, string packageName, string packageVersion, string type)
        {
            var dir = Path.Combine(basePath, packageName, packageVersion, type);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
