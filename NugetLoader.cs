using Org.Kevoree.Core.Api;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Org.Kevoree.NugetLoader
{
    [Serializable]
    public class NugetLoader
    {
        private string nugetLocalRepositoryPath;
        public NugetLoader(string nugetLocalRepositoryPath)
        {
            this.nugetLocalRepositoryPath = nugetLocalRepositoryPath;
        }

        public T LoadRunnerFromPackage<T>(string packageName, string packageVersion, string remoteRepositoryUrl) where T : IRunner
        {
            if (this.nugetLocalRepositoryPath == null || this.nugetLocalRepositoryPath == "")
            {
                this.nugetLocalRepositoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(this.nugetLocalRepositoryPath);
            }
            var mepdm = new MEPDirectoryManager(nugetLocalRepositoryPath);
            var cachePath = mepdm.getCachePath(packageName, packageVersion);
            var pluginPath = mepdm.getPluginPath(packageName, packageVersion);

            T res;
            if (new NugetManager(pluginPath).resolve(packageName, packageVersion, remoteRepositoryUrl))
            {


                // This creates a ShadowCopy of the MEF DLL's 
                // (and any other DLL's in the ShadowCopyDirectories)
                var setup = new AppDomainSetup
                {
                    CachePath = cachePath,
                    ShadowCopyFiles = "true",
                    ShadowCopyDirectories = pluginPath
                };

                // Create a new AppDomain then create a new instance 
                // of this application in the new AppDomain.            
                AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), AppDomain.CurrentDomain.Evidence, setup);
                var ret = (T)domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);

                /*var dirdirshadow = new FileInfo(pluginPath).Directory;
                foreach (var a in


                dirdirshadow.GetFiles("*.dll", SearchOption.AllDirectories)
                .Select(x => AssemblyName.GetAssemblyName(x.FullName))
                .Select(x => Assembly.Load(x.FullName)))
                {
                    BuildManager.AddReferencedAssembly(a);
                }*/

                //domain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);


                ret.setPluginPath(pluginPath);
                ret.setPackageName(packageName);
                ret.setPackageVersion(packageVersion);
                res = ret;
            }
            else
            {
                res = default(T);
            }
            return res;
        }
    }
}
