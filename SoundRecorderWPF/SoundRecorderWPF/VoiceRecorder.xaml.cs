using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Diagnostics;
using System.Speech.Recognition;
using System.IO;
using System.Xml;

namespace SoundRecorderWPF
{
    /// <summary>
    /// Interaction logic for VoiceRecorder.xaml
    /// </summary>
    public partial class VoiceRecorder : Window
    {
        private bool recordSwitch = false;
        public VoiceRecorder()
        {
            InitializeComponent();
        }
        // Handle the SpeechRecognized event.
        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Windows.MessageBox.Show("Recognized text: " + e.Result.Text);
        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Windows.MessageBox.Show("Speech recognized: " + e.Result.Text);
        }

        private SpeechRecognitionEngine _recognizer = null;
        private SpeechRecognizer sr = null;
        private NAudio.Wave.BlockAlignReductionStream stream = null;
        private NAudio.Wave.WaveOutEvent output = null;
        private DispatcherTimer dtClockTime = null;
        private Stopwatch stopWatch = null;
        string currentTime = string.Empty;
        private NAudio.Wave.WaveIn SourceStream = null;
        private NAudio.Wave.DirectSoundOut waveOut = null;
        private NAudio.Wave.WaveFileWriter waveWriter = null;

        public void StopWatch()
        {
            stopWatch = new Stopwatch();
            dtClockTime = new DispatcherTimer();
            dtClockTime.Tick += new EventHandler(dt_Tick);
            dtClockTime.Interval = new TimeSpan(0, 0, 0, 0, 1); //in Hour, Minutes, Second.
            dtClockTime.Start();
        }
        void dt_Tick(object sender, EventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                TimeSpan ts = stopWatch.Elapsed;
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                //txtBox.Text = currentTime;
            }

        }
        private void DisposeWave()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    output.Stop();
                    output.Dispose();
                    output = null;
                }
            }
        }
        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            txtSearchResults.Text = "";
            txtSearchInput.Text = "";            
            if(recordSwitch)
            {
                lblbtnRecord.Text = "Record";
                recordSwitch = false;
            }
            else
            {
                lblStatus.Content = "Listening....";
                lblbtnRecord.Text = "Listening....";
                recordSwitch = true;
            }
            InitializeSpeechEngine();
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }
        private void StartAudio()
        {

            try
            {
                //List<NAudio.Wave.WaveInCapabilities> sources = new List<NAudio.Wave.WaveInCapabilities>();

                //for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
                //{
                //    System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem(NAudio.Wave.WaveIn.GetCapabilities(i).ProductName);
                //    //sourceList.Items.Add(item.Text);
                //}

                //SaveFileDialog save = new SaveFileDialog();
                //save.Filter = "Wave Files (*.wav)|*.wav;";
                //save.ShowDialog();
                //save.DefaultExt = "wav";
                btnRecord.Content = "Recording...";
                //int deviceNum = sourceList.SelectedIndex;
                SourceStream = new NAudio.Wave.WaveIn();
                //SourceStream.DeviceNumber = deviceNum;
                SourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(0).Channels);
                SourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
                //waveWriter = new NAudio.Wave.WaveFileWriter(save.FileName, SourceStream.WaveFormat);
                waveWriter = new NAudio.Wave.WaveFileWriter(@"D:\Recordings\" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".wav", SourceStream.WaveFormat);

                SourceStream.StartRecording();
                StopWatch();
                stopWatch.Start();
            }
            catch (ArgumentException)
            {
                System.Windows.MessageBox.Show("No file was created");
                btnRecord.Content = "Record";
            }
        }
        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (waveWriter == null) return;

            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }
        private void StopRecording()
        {

            btnRecord.Content = "Record";
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }
            if (SourceStream != null)
            {
                SourceStream.StopRecording();
                SourceStream.Dispose();
            }
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing || output.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                {
                    output.Stop();
                    output.Dispose();
                    output = null;
                }
            }
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                       0, 0, 0, 0);
                //txtBox.Text = currentTime;
            }
            else
            {
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                      0, 0, 0, 0);
                //txtBox.Text = currentTime;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Content = "Search in progress....";
            this.Cursor = System.Windows.Input.Cursors.Wait;
            ExecuteCommandSync(txtSearchInput.Text);
            lblStatus.Content = "Seach results updated.";
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }
        public void ExecuteCommandSync(object command)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\WinPython-64bit-3.5.3.1Zero\python-3.5.3.amd64\python.exe";
            start.Arguments = string.Format("\"{0}\" \"{1}\"", @"C:\WinPython-64bit-3.5.3.1Zero\python-3.5.3.amd64\train_test_nb_svm.py", command);
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)

            lblStatus.Content = "Executing search....";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")

                    txtSearchResults.Text = GetCommandText(result.Trim());
                }
            }
        }
        public string GetCommandText(string txtCommand)
        {
            lblStatus.Content = "Fetching command template....";
            string returnCommandText = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Environment.CurrentDirectory + @"\data\Result.xml");
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/root/command");
            foreach (XmlNode node in nodeList)
            {
                if (node.Attributes["name"].Value.ToLower() == txtCommand)
                    return returnCommandText = node.InnerText;
            }
            return returnCommandText;
        }

        private void txtSearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (txtSearchInput.Text != string.Empty)
            //{
            //    btnRecord.IsEnabled = false;
            //}
            //else
            //{
            //    btnRecord.IsEnabled = true;
            //}

            lblStatus.Content = "";
            txtSearchResults.Text = "";
        }
        
        void InitializeSpeechEngine()
        {
            if (recordSwitch)
            {
                lblStatus.Content = "Initializing Speech Engine....";
                _recognizer = new SpeechRecognitionEngine();
                LoadGrammar();
                _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("exit"))); // load a "exit" grammar
                _recognizer.SpeechRecognized += _recognizer_SpeechRecognized;
                _recognizer.SpeechRecognitionRejected += _recognizer_SpeechRecognitionRejected;
                _recognizer.SetInputToDefaultAudioDevice(); // set the input of the speech recognizer to the default audio device
                _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
            }
            else
            {
                if (_recognizer != null)
                {
                    _recognizer.Dispose(); // dispose the speech recognition engine
                }
            }
        }
        public void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            lblStatus.Content = "Speech Recognized!";
            txtSearchInput.Text = e.Result.Text;
            ExecuteCommandSync(e.Result.Text);
            lblStatus.Content = "Speech Recognized, updated search results!";
        }
        public void _recognizer_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result.Alternates.Count == 0)
            {
                lblStatus.Content = "Speech rejected. No candidate phrases found."; ;
                return;
            }
        }
        private void LoadGrammar()
        {
            try
            {
                Choices texts = new Choices();
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "\\Grammer.txt");
                foreach (string line in lines)
                {
                    // skip commentblocks and empty lines..
                    if (line.StartsWith("--") || line == String.Empty) continue;

                    // add the text to the known choices of speechengine
                    texts.Add(line);
                }
                Grammar wordsList = new Grammar(new GrammarBuilder(texts));
                _recognizer.LoadGrammar(wordsList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
