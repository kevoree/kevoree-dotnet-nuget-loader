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

        internal bool resolve(string packageName, string packageVersion, string remoteRepositoryUrl)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository(remoteRepositoryUrl);

            var packageManager = new PackageManager(repo, pluginPath);

            var package = repo.FindPackage(packageName, SemanticVersion.Parse(packageVersion), true, true);
            bool ret;
            if (package != null)
            {
                packageManager.InstallPackage(package, false, true);

                ret = true;
            }
            else { ret = false; }
            return ret;
        }
    }
}
