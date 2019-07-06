using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace CheckDX
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);
                                                Check();
                    }

        /// <summary>
        ///  Find out directX assemblies in GAC
                /// </summary>
        /// <returns></returns>
        private List <string>  ListDirectXAssemblies()
        {
            try
            {
                // initialise list for holding all DX assemblies on machine
                List<string> AssemblyPaths = new List<string>();

                // Get all assemblies refered by this application
                Assembly[] ReferedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                DirectoryInfo DirInfo;

                foreach (Assembly assembly in ReferedAssemblies)
                {
                    if (assembly.GlobalAssemblyCache)
                    { //1
                        DirInfo = new DirectoryInfo(Directory.GetParent(assembly.Location).FullName);

                        // get the adequate parent directory of assembly
                        for (int i = 0; i < 3; i++)
                        {//2
                            if (DirInfo.Root.Name == DirInfo.Parent.Name
                                || DirInfo.Parent.FullName == Directory.GetParent(Environment.SystemDirectory).FullName)
                                break;

                            DirInfo = DirInfo.Parent;
                        }//-2

                        // add assembly directories to list only if it is not duplicate and its parent directory is not in list
                        if ((!AssemblyPaths.Contains(DirInfo.FullName)
                            && !ParentExists(AssemblyPaths, DirInfo.FullName))
                            ||
                            AssemblyPaths.Count == 0)
                            AssemblyPaths.Add(DirInfo.FullName);
                    }//-1
                }

                //if (AssemblyPaths != null)
                //{
                //for (int i = 0; i < AssemblyPaths.Count; i++)
                //MessageBox.Show(AssemblyPaths[i]);
                //}

                return FindDirectXPaths(AssemblyPaths);
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }

/// <summary>
///  check if the parrent directory is already in list
/// </summary>
/// <param name="DirList"></param>
/// <param name="path"></param>
/// <returns></returns>
        private bool ParentExists(List<string> DirList, string path)
        {
            bool Exists = false;
            
            for (int i = 0; i < DirList.Count; i++)
            {
                if (path.Trim ().StartsWith(DirList[i].Trim ()  , StringComparison.CurrentCultureIgnoreCase))
                                                                            Exists = true;
                            }
            return  Exists ;
        }
/// <summary>
///  find all directX assemblies contained by directory list supplied
/// </summary>
/// <param name="DirPaths"></param>
/// <returns></returns>
private List <string>  FindDirectXPaths(List<string> DirPaths)
        {
                        List<string> DXPaths = new List<string>();

            for (int i = 0; i < DirPaths.Count; i++)
            {
                string[] FileList = Directory.GetFiles(DirPaths[i], "Microsoft.DirectX.DirectSound.dll" , SearchOption.AllDirectories);
                                DXPaths.AddRange(FileList);
            }
            
            return DXPaths;
        }

        /// <summary>
        /// Checks DirectX version and opens download page if suitable version is not found on deployment machine
                /// </summary>
        public void Check()
        {
            /*
            RegistryKey directXKey = Registry.LocalMachine.OpenSubKey ("SOFTWARE\\Microsoft\\DirectX,Version=4.09.00.0904") ;
            if (directXKey == null)
            {
                MessageBox.Show("Direct x 9c not found");
            }
            else
            {
                MessageBox.Show("DirectX found");
            }
            return ;
             */ 
            CheckForDirectXManaged ( ListDirectXAssemblies() );
        }

        /// <summary>
        /// check for right version of DirectX
                        /// </summary>
        /// <param name="DXList"></param>
        private void CheckForDirectXManaged(List<string> DXList)
        {
            bool IsRightVersion = false;
            if (DXList != null)
            {//1
                for (int i = 0; i < DXList.Count; i++)
                {//2
                    string Version = "   ";
                    try
                    {
                        Version = (Assembly.ReflectionOnlyLoad(DXList[i]).ImageRuntimeVersion);
                    }
                    catch (System.Exception)
                    {
                        //OpenDirectXLink();
                    }

                    if (Version != ""
                        && Version.Substring(0, 3) != "v1."
                 && Version.Substring(0, 3) != "v2.")
                    {
                        IsRightVersion = true;
                    }
                }// end for
            } //end if
            else
            {
                RegistryKey directXKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\DirectX");
                string version = null;
                foreach (string name in directXKey.GetValueNames())
                {
                    if (name == "Version")
                    {
                        version = directXKey.GetValue(name).ToString();
                    }
                }
                if (version == "4.09.00.0904")
                {
                    IsRightVersion = true;
                }
            }

            if (!IsRightVersion)
                OpenDirectXLink();

        }//end function

        /// <summary>
        /// Old function for checking DirectX version: This uses depricated dotnet functions.
                /// </summary>
        private void CheckForDirectXManaged()
        {
            string Version = "   ";
            try
            {
                Version = (Assembly.LoadWithPartialName("Microsoft.DirectX.DirectSound").ImageRuntimeVersion);
            }
             catch (System.Exception )
            {
                //OpenDirectXLink();
            }
                        
            if ( Version != "" 
                && Version.Substring(0, 3) != "v1."
         && Version.Substring(0, 3) != "v2.")
            {
                OpenDirectXLink();
            }
                                }// end function

        private void OpenDirectXLink()
        {
        string installerSourceDirPath = Context.Parameters["SourcePath"];
        string DXPath = "";

            string messageBoxString = "Installation complete.\n But application may not function properly because Required DirectX version ( ver 9 c managed ) not Installed. \n Do you want to install it now?" ;

        if (installerSourceDirPath != null && Directory.Exists ( installerSourceDirPath ))
            {
            DXPath =  Path.Combine ( Directory.GetParent ( installerSourceDirPath ).Parent.FullName,
                "Prereq\\DX9C\\DXSETUP.exe" );
            }
        
        if (DXPath == "" || !File.Exists ( DXPath ))
            {
            //DXPath = "http://www.microsoft.com/downloads/details.aspx?familyid=2da43d38-db71-4c1b-bc6a-9b6652cd92a3"; // old path
                DXPath = "https://www.microsoft.com/en-us/download/details.aspx?id=35&nowin10"; // updated on July 12, 2016
            messageBoxString = "Installation complete.\n But application may not function properly because Required DirectX version ( ver 9 c managed ) not Installed. \n Do you want to download it from internet?" ;
            }


            DialogResult result = MessageBox.Show(messageBoxString , "DirectX error!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = DXPath;
                p.Start();
            }
        }

    }
}