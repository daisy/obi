
using System;
using System.Collections.Generic;
using System.Text;

namespace DTBMerger.PipelineInterface
    {
    public class ScriptsFunctions
        {

        
        public static void Validate ( string scriptPath, string dtbPath, string errorFilePath, int timeTolerance )
            {
            ScriptParser validatorParser = new ScriptParser ( scriptPath ); 
            
            foreach ( ScriptParameter p in validatorParser.ParameterList )
                {
                if (p.Name == "input")
                    {
                    p.ParameterValue = dtbPath;
                    }
                if (p.Name == "validatorTimeTolerance")
                    {
                    p.ParameterValue = timeTolerance.ToString ();
                    }

                }
            validatorParser.ExecuteScript (true);
            }

        public static void PrettyPrinter ( string scriptPath, string dtbPath, string outputDir )
            {
            ScriptParser prettyPrinterParser = new ScriptParser ( scriptPath );

            foreach (ScriptParameter p in prettyPrinterParser.ParameterList)
                {
                if (p.Name == "input")
                    {
                    p.ParameterValue = dtbPath;
                    }
                if (p.Name == "output")
                    {
                    p.ParameterValue = outputDir;
                    }

                }
            prettyPrinterParser.ExecuteScript (false);
            }

        }
    }
