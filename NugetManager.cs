using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Org.Kevoree.NugetLoader
{
    class NugetManager
    {
        private string pluginPath;

        public NugetManager(string pluginPath)
        {
            this.pluginPath = pluginPath;
        }

        internal void resolve(string packageName, string packageVersion)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

            var packageManager = new PackageManager(repo, pluginPath);

            var package = repo.FindPackage(packageName, SemanticVersion.Parse(packageVersion));
            if (package != null)
            {
                packageManager.InstallPackage(package, false, true);

                this.cleanupDlls();
            }
        }

        /**
         * Déplace toutes les dll à la racine du répertoire.
         */
        private void cleanupDlls()
        {
            var files = Directory.GetFiles(pluginPath, "*.dll", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.Move(file, Path.Combine(pluginPath, Path.GetFileName(file)));
            }
        }
    }
}
