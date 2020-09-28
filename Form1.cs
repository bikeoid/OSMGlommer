using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using OsmGlommer;

namespace OSMGlommer
{
    public partial class Form1 : Form
    {
        bool terminating = false;
        private Thread glomThread = null;
        private bool addOsmEditAttributes = false;
        private bool performGlom = false;

        private OSMDataset osmDataSet;

        public delegate void LogMessageDelegate(string status, bool error);


        public Form1()
        {
            InitializeComponent();
        }

        private void btBrowseFolder_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = tbFilename.Text;
            DialogResult result = openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "OSM Map Vector Files(*.osm;*.xml)|*.osm;*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;

            if (result == DialogResult.OK)
            {
                tbFilename.Text = openFileDialog1.FileName;
            }
        }




        private void LoadOSMFile(string xmlFilename)
        {
            osmDataSet = new OSMDataset();
            osmDataSet.ReadOSMDocument(xmlFilename);

            CleanupBadWays();

            LogMessage("Finished Reading: " + osmDataSet.osmWays.Count.ToString() + " ways", false);

            LogMessage("Finished Reading: " + osmDataSet.osmNodes.Count.ToString() + " nodes", false);
        }

        private void CleanupBadWays()
        {
            var removeList = new List<Int64>();
            foreach (var way in osmDataSet.osmWays.Values)
            {
                if (way.NodeList.Count < 2)
                {
                    removeList.Add(way.ID);

                    //  Alert - too few nodes in way
                    string streetName = "";
                    if (way.Tags.ContainsKey("name"))
                    {
                        streetName = way.Tags["name"];
                    }
                    LogMessage("Warning - too few nodes in way " + streetName, false);
                }

            }

            foreach (var id in removeList)
            {
                osmDataSet.osmWays.Remove(id);
            }

        }





        private void LinkWays()
        {
            var glommer = new Glommer();
            glommer.DoGlom(osmDataSet);
        }




        private void SaveOSM(string inFilename)
        {
            var outFilename = inFilename;
            var dir = Path.GetDirectoryName(outFilename);

            var baseFileName = Path.GetFileNameWithoutExtension(outFilename);
            var ext = Path.GetExtension(outFilename);
            var action = "_glommed";
            if (addOsmEditAttributes)
            {
                action = "_fixed";
            }
            var fileName = baseFileName + action + ext;
            var saveFilePath = Path.Combine(dir, fileName);

            var setList = new List<OSMDataset>();
            setList.Add(osmDataSet);
            WriteOSM.WriteDocument(saveFilePath, setList);

            LogMessage("Wrote " + osmDataSet.osmWays.Count + " OSM Ways to " + fileName, false);
        }


        private void GlomFile(string osmFilename)
        {
            LogMessage("Glom started for " + osmFilename, false);
            DateTime startTime = DateTime.Now;
            LoadOSMFile(osmFilename);
            if (performGlom)
            {
                LogMessage("Linking ways ...", false);
                LinkWays();
            }
            SaveOSM(osmFilename);

            DateTime endTime = DateTime.Now;
            var elapsed = endTime - startTime;
            LogMessage("Glom complete.  Elapsed time: " + elapsed.ToString(), false);

        }

        private void GlomThread(object objOSMFilename)
        {
            try
            {
                string osmFilename = (string)objOSMFilename;
                if (osmFilename.EndsWith(".osm"))
                {
                    // Single file
                    GlomFile(osmFilename);
                }
                else
                {
                    // Directory specification
                    var fileList = Directory.GetFiles(osmFilename, "*.osm", SearchOption.AllDirectories);
                    foreach (var file in fileList)
                    {
                        GlomFile(file);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during glom operation: " + ex.Message);
            }
            EndGlomThread();

        }

        private void EndGlomThread()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(EndGlomThread));
                return;
            }
            btGlom.Enabled = true;
        }




        private void btGlom_Click(object sender, EventArgs e)
        {

            try
            {
                addOsmEditAttributes = false; // cbAddOsmEditAttributes.Checked;
                performGlom = true; // cbPerformGlom.Checked;
                string osmFilename = tbFilename.Text;
                glomThread = new Thread(GlomThread);
                glomThread.Name = "Glom Thread";
                glomThread.IsBackground = true;
                glomThread.Start(osmFilename);
                btGlom.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting glom operation: " + ex.Message);
                btGlom.Enabled = true;
            }

        }


        public void LogMessage(string msg, bool errorStatus)
        {

            const int MaxLogLines = 1000;

            if (terminating)
            {
                return;
            }
            if (InvokeRequired)
            {

                object[] arglist = new object[2];
                arglist[0] = msg;
                arglist[1] = errorStatus;
                Invoke(new LogMessageDelegate(LogMessage), arglist);
                return;
            }


            if (richTextLogBox.Lines.GetLength(0) > MaxLogLines)
            {
                // Delete the first line and reposition cursor to end
                richTextLogBox.Select(0, richTextLogBox.Lines[0].Length + 1);
                richTextLogBox.SelectedText = "";
                richTextLogBox.Select(richTextLogBox.TextLength, 0);
            }

            msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ": " + msg + "\n";


            if (errorStatus)
            {

                richTextLogBox.SelectionColor = Color.White;
                richTextLogBox.SelectionBackColor = Color.Red;
            }
            else
            {
                richTextLogBox.SelectionColor = Color.Black;
                richTextLogBox.SelectionBackColor = Color.White;
            }

            richTextLogBox.AppendText(msg);
            richTextLogBox.ScrollToCaret();

            Application.DoEvents();

        }


    }

}
