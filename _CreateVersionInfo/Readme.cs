using System;
using System.Collections.Generic;
using System.Text;

namespace _Dummy
{
    public class Readme
    {
        /* This project enables you to update and synchronize the VersionInfo in all your assemblies.
         * 
         * The versioning is done based on the VersionInfo.txt and the subversion-revision number.
         * The '*' in the VersionInfo.txt is replaced with the subversion revision number when a build is done.
         * 
         * To add the versioninfo to you assemblies do the following:
         * 
         * 1. In all your AssemblyInfo.cs's remove the lines:
         * 
         *    [assembly: AssemblyVersion("1.0.0.0")]
         *    [assembly: AssemblyFileVersion("1.0.0.0")]
         * 
         * 2. In all your project add a link to the VersionInfo.cs.
         *    1. Rightclick on the project and add an existing item.
         *    2. Browse to the VersionInfo.cs in this folder.
         *    3. Click on the down-arrow next to 'Add' and select 'Add as Link'.
         * 
         * 3. Make the _CreateVersionInfo-project compile as the first project in your solution.
         *    1. Right click on the solution and select Project Dependencies
         *    2. In the tab Build Order the _CreateVersionInfo-project must be on top
         *    3. When this is not the case add the _CreateVersionInfo-project to the dependecy of one of your projects.
         *    
         * To keep the subversion-revision number in line with SubVersion you must perform SVN Update on the trunk before you build the solution.
         */
    }
}
