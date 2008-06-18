using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PipelineInterface
{
    // class for setting parameters of mp3 audio pipeline task script
    class Mp3Encoder
    {
        private ScriptParser M_EncoderScriptParser; //instance of ScriptParser class
        private string m_ScriptFilePath; // Path of DTBToAudioEncoder script file

        public Mp3Encoder()
        {
            string relativeScriptPath = "PipelineCmd\\scripts\\manipulation\\simple\\DTBAudioEncoder.taskScript";
            m_ScriptFilePath = Directory.GetParent(relativeScriptPath).ToString() + "\\" + "DTBAudioEncoder.taskScript";
                        M_EncoderScriptParser = new ScriptParser(m_ScriptFilePath);
        }
        /// <summary>
        /// gets and sets path of input DTB file
                /// </summary>
        public string InputFilePath
        {
            get
            {
                return M_EncoderScriptParser.GetParameterValue("input");
            }
            set
            {
                M_EncoderScriptParser.SetParameterValue("input", value);
            }
        }

        /// <summary>
        /// summary
        /// gets and sets path of Output directory
        /// </summary>
        public string OutputDirectory
        {
            get
            {
                return M_EncoderScriptParser.GetParameterValue("output");
            }
            set
            {
                M_EncoderScriptParser.SetParameterValue("output", value);
            }
        }


        /// <summary>
        /// summary
        /// gets and sets bit rate of mp3 files to be encoded
        /// </summary>
        public string bitrate
        {
            get
            {
                return M_EncoderScriptParser.GetParameterValue("bitrate");
            }
            set
            {
                                M_EncoderScriptParser.SetParameterValue("bitrate", value);
            }
        }

        /// <summary>
        /// summary
        ///  Starts Encoding operation
        /// </summary>
        public void ExecuteEncoder ()
        {
            if (!File.Exists(InputFilePath)
                || !Directory.Exists(OutputDirectory) || Directory.GetFiles(OutputDirectory).Length > 0
                || (bitrate != "32" && bitrate != "48" && bitrate != "64" && bitrate != "128"))
            {
                throw new System.Exception("One or more parameters are invalid");
                return;
            }
            M_EncoderScriptParser.CommitScriptChanges();

            // invoke the script
                        Process PipelineProcess = new Process();
            PipelineProcess.StartInfo.CreateNoWindow = true;
            PipelineProcess.StartInfo.ErrorDialog = true;
            //PipelineProcess.StartInfo.UseShellExecute = true;
                        
                        PipelineProcess.StartInfo.FileName =System.AppDomain.CurrentDomain.BaseDirectory+  "\\PipelineCmd\\Pipeline.bat";
                        PipelineProcess.StartInfo.Arguments = m_ScriptFilePath;
                        PipelineProcess.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "\\PipelineCmd" ;
                        
            try
            {
                PipelineProcess.Start();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            PipelineProcess.WaitForExit();
            M_EncoderScriptParser = null;
            System.Windows.Forms.MessageBox.Show("Done");
                    }


             }
}
