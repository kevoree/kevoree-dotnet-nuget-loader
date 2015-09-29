using NuGet;
using System.IO;

namespace Org.Kevoree.NugetLoader
{
    class NugetManager
    {
        private string pluginPath;

        public NugetManager(string pluginPath)
        {
            this.pluginPath = pluginPath;
        }

        internal void resolve(string packageName, string packageVersion, string remoteRepositoryUrl)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository(remoteRepositoryUrl);

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
                var target = Path.Combine(pluginPath, Path.GetFileName(file));
                if (!File.Exists(target))
                {
                    File.Move(file, target);
                }
            }
        }
    }
}
