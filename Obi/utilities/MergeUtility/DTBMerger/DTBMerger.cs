using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DTBMerger
    {
    public class DTBMerger
        {
        private String []  m_InputPaths;
        private string m_OutputDirectory;

        public DTBMerger ( string[] inputPaths , string outputDirectory )
            {
            if (inputPaths.Length == 0)
                throw new System.Exception ( "No input DTBs" );

            for (int i = 0; i < inputPaths.Length; i++)
                {
                if ( !File.Exists ( inputPaths[i] ))
                    {
                    throw new System.IO.FileNotFoundException (inputPaths[i] + " do not exists") ;
                    }
                }
            m_InputPaths = inputPaths;
            
            if ( Directory.Exists (outputDirectory ) )
                {
            Directory.Delete (outputDirectory,true ) ;
                }
            Directory.CreateDirectory ( outputDirectory ) ;
            m_OutputDirectory = outputDirectory ;
            }

        public void MergeDTDs ()
            {
            List<string> inputParameterList = CopyAllDTDsToOutputDirectory ();
            RenameDTDFiles ( inputParameterList );
            DTBIntegrator integrator = new DTBIntegrator ( inputParameterList );
            integrator.IntegrateDTBs ();
            }


        private List<string> CopyAllDTDsToOutputDirectory ()
            {
            List<string> inputParameterList = new List<string> ();
            // copy first DTB to output directory
            string opfPath =  CopyDTBFiles (
                Directory.GetParent ( m_InputPaths[0] ).FullName,
                m_OutputDirectory );

            inputParameterList.Add ( opfPath );

            // copy all remaining DTBs in their folders
            for (int i = 1; i < m_InputPaths.Length; i++)
                {
                string copyToDirectory = Path.Combine ( m_OutputDirectory, i.ToString () ) ;
                Directory.CreateDirectory (copyToDirectory );

                opfPath =  CopyDTBFiles (
                Directory.GetParent ( m_InputPaths[i] ).FullName,
                copyToDirectory );

                inputParameterList.Add ( opfPath );
}

return inputParameterList;

            }

        private string CopyDTBFiles ( string sourceDirectory , string destinationDirectory )
            {
            string[] sourceFilePaths = Directory.GetFiles ( sourceDirectory , "*.*", SearchOption.TopDirectoryOnly);

            string opfPath = "";

            for (int i = 0; i < sourceFilePaths.Length; i++)
                {
                FileInfo f = new FileInfo ( sourceFilePaths[i] );
                string destinationPath = Path.Combine ( destinationDirectory, f.Name );
                f.CopyTo ( destinationPath );

                if (f.Extension == ".opf")
                    opfPath = destinationPath;
                }
            return opfPath;
            }

        private void RenameDTDFiles ( List<string> opfPathsList)
            {
            for (int i = 0; i < opfPathsList.Count; i++)
                {
                string prefix = Convert.ToChar( ((int)'a') + i ).ToString  ();
                prefix = prefix + "_";
                //MessageBox.Show ( prefix.ToString () );

                DTBRenamer renamer = new DTBRenamer ( opfPathsList[i], prefix );
                renamer.RenameDTBFilesSet ();
                }
            }


        }
    }
