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
using Microsoft.Kinect;
using System.IO;
using System.Collections;
using System.Timers;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net.Mail;
using System.Globalization;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;

namespace SkeletonCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Int32 noOfSkeletons = 0; //no of interested skeletons
        //private Int32 myNumberOfFrames = 0;
        ArrayList mySkeletonIDs = new ArrayList();
        ArrayList myFrameTimers = new ArrayList();
        private KinectSensor sensor;
        //private Boolean fireFlag = true;
        ArrayList myFlags = new ArrayList();
        private WriteableBitmap colorBitmap;
        private Int32 noOfInterestedPeople = 0;
        private byte[] colorPixels;
        static WriteableBitmap testBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {   // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    //MessageBox.Show("Connected");
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(this.sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data

                this.sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;
                this.sensor.ColorFrameReady += Sensor_ColorFrameReady;
                // Start the sensor!
                try
                {
                    this.sensor.Start();
                    //MessageBox.Show("Hello There!");
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.counterTextBlock.Text = "Kinect is not ready";
            }
        }

        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }

        private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            //fpsTextBlock.Text = myNumberOfFrames.ToString();
            if(myFrameTimers.Count > 0)
            {
                fpsTextBlock.Text = "";
                foreach(int tempCounter in myFrameTimers)
                {
                    fpsTextBlock.Text += "Skel. " + mySkeletonIDs[myFrameTimers.IndexOf(tempCounter)].ToString() + ": " + tempCounter.ToString() + "\n";
                    Console.Write("Skel. " + mySkeletonIDs[myFrameTimers.IndexOf(tempCounter)].ToString() + ": " + tempCounter.ToString() + " | ");
                }
                Console.WriteLine();
            }
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length != 0)
            {
                foreach (Skeleton skel in skeletons)
                {
                    
                    if (skel.TrackingState == SkeletonTrackingState.Tracked || skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        Console.WriteLine("Z: " + skel.Position.Z);
                        if (skel.Position.Z <= 3.0)
                        {
                            if (mySkeletonIDs.IndexOf(skel.TrackingId) == -1)
                            {
                                //myNumberOfFrames = 0;
                                //fireFlag = true;
                                mySkeletonIDs.Add(skel.TrackingId);
                                myFrameTimers.Insert(mySkeletonIDs.IndexOf(skel.TrackingId), 0);
                                myFlags.Insert(mySkeletonIDs.IndexOf(skel.TrackingId), true);
                                Console.WriteLine("Skeleton ID: " + skel.TrackingId.ToString());
                                noOfSkeletons++;
                            }
                            else
                            {
                                //myNumberOfFrames++;
                                //if((myNumberOfFrames >= 300) && (fireFlag==true))
                                int tempTimer = (int)myFrameTimers[mySkeletonIDs.IndexOf(skel.TrackingId)];
                                tempTimer++;
                                myFrameTimers[mySkeletonIDs.IndexOf(skel.TrackingId)] = tempTimer;
                                if ((tempTimer >= 300) && ((Boolean)myFlags[mySkeletonIDs.IndexOf(skel.TrackingId)] == true))
                                {
                                    interestedSkeleton(mySkeletonIDs.IndexOf(skel.TrackingId));

                                }
                            }
                        }

                        //counterTextBlock.Text = noOfSkeletons.ToString();
                    }
                }
            }
            counterTextBlock.Text = "Total People: " + noOfSkeletons.ToString() + " | Interested People: " + noOfInterestedPeople.ToString();
        }

        

        private void interestedSkeleton(int pos)
        {

            //fireFlag = false;
            noOfInterestedPeople++;
            myFlags[pos] = false;
            MessageBox.Show("Interested");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("intellimannstorage_AzureStorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("myimages");
            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);

            string path = "KinectSnapshot"+time+".jpeg";
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);
            // Create or overwrite the "myblob" blob with contents from a local file.
        
            BitmapEncoder encoder = new JpegBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(this.colorBitmap));
            

            // write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    encoder.Save(fs);
                }

            }
            catch (IOException)
            {
            }

            using (var fileStream = System.IO.File.OpenRead(@"KinectSnapshot"+time+".jpeg"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        
        private void Window_Closed(object sender, EventArgs e)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
    CloudConfigurationManager.GetSetting("intellimannstorage_AzureStorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("myinfo");
            container.CreateIfNotExists();
            string path = "info.txt";
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);


            // write the new file to disk
            try
            {
                using (StreamWriter sw = new StreamWriter("info.txt"))
                {

                    sw.WriteLine(noOfSkeletons.ToString());
                    sw.WriteLine(noOfInterestedPeople.ToString());
                }

            }
            catch (IOException)
            {
            }

            using (var fileStream = System.IO.File.OpenRead(@"info.txt"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }
    }
}
