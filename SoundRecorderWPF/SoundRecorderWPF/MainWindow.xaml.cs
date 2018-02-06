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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Diagnostics;
namespace SoundRecorderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private NAudio.Wave.BlockAlignReductionStream stream = null;
        private NAudio.Wave.WaveOutEvent output = null;
        private DispatcherTimer dtClockTime = null;
        private Stopwatch stopWatch = null;
        string currentTime = string.Empty;
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Sound Files (*.mp3;*.wav)|*.mp3;*.wav;";
                open.ShowDialog();

                DisposeWave();
                //wave = new NAudio.Wave.WaveFileReader(open.FileName);
                if (open.FileName.EndsWith(".mp3"))
                {
                    NAudio.Wave.WaveStream pcm = new NAudio.Wave.Mp3FileReader(open.FileName);
                    stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
                }
                else if (open.FileName.EndsWith(".wav"))
                {
                    NAudio.Wave.WaveStream pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(open.FileName));
                    stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
                }

                output = new NAudio.Wave.WaveOutEvent();
                output.Init(new NAudio.Wave.WaveChannel32(stream));
                output.Play();
                StopWatch();
                if (stopWatch.IsRunning)
                {
                    stopWatch.Stop();
                    
                }
                else stopWatch.Start();
               
                btnPausePlay.IsEnabled = true;
            }catch(ArgumentException){
                System.Windows.MessageBox.Show("Choose a real file");
            }
        }



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
                txtBox.Text = currentTime;
            }
            
        }



        private void btnPausePlay_Click(object sender, RoutedEventArgs e)
        {
           // StopWatch();
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    output.Pause();
                    stopWatch.Stop();
                    
                    
                    
                }
                else if (output.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                {
                    output.Play();
                    stopWatch.Start();
                }
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



        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            List<NAudio.Wave.WaveInCapabilities> sources = new List<NAudio.Wave.WaveInCapabilities>();
           
            sourceList.Items.Clear();
            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem(NAudio.Wave.WaveIn.GetCapabilities(i).ProductName);
                sourceList.Items.Add(item.Text);
            }
           
              
        }


        private NAudio.Wave.WaveIn SourceStream = null;
        private NAudio.Wave.DirectSoundOut waveOut = null;
        private NAudio.Wave.WaveFileWriter waveWriter = null;



        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {

            if (sourceList.SelectedItems.Count == 0)
                return;
            else
            {
                try
                {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "Wave Files (*.wav)|*.wav;";
                    save.ShowDialog();
                    save.DefaultExt = "wav";
                
                int deviceNum = sourceList.SelectedIndex;
                SourceStream = new NAudio.Wave.WaveIn();
                SourceStream.DeviceNumber = deviceNum;
                SourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNum).Channels);
                SourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
                waveWriter = new NAudio.Wave.WaveFileWriter(save.FileName, SourceStream.WaveFormat);

                SourceStream.StartRecording();
                StopWatch();
                stopWatch.Start();
                }
                catch (ArgumentException)
                {
                    System.Windows.MessageBox.Show("No file was created");
                }
            }

        }



        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (waveWriter == null) return;

            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }


       

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
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
                txtBox.Text = currentTime;
            }
            else
            {
                currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                      0, 0, 0, 0);
                txtBox.Text = currentTime;
            }
           
            
            
        }

        private void VolumeSl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (output != null)
                {
                     
                   output.Volume =(float)VolumeSl.Value;
                   

                }
            }
            catch (System.InvalidOperationException)
            {
                txtBox.Text = "ERROR OCCURRED";
            }
            catch (NullReferenceException)
            {
                txtBox.Text = "No sound";
            }
        }

        

        
     }
}
