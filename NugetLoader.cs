using Org.Kevoree.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Kevoree.NugetLoader
{
    public class NugetLoader
    {
        private string basePath;
        public NugetLoader(string basePath)
        {
            this.basePath = basePath;
        }

        public T LoadRunnerFromPackage<T>(string packageName, string packageVersion) where T:IRunner
        {
            var mepdm = new MEPDirectoryManager(basePath);
            var cachePath = mepdm.getCachePath(packageName, packageVersion);
            var pluginPath = mepdm.getPluginPath(packageName, packageVersion);

            new NugetManager(pluginPath).resolve(packageName, packageVersion);


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
            ret.setPluginPath(pluginPath);
            return ret;
        }
    }
}
