using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EmguPlayground.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private Mat _current = new Mat();
        private Mat _original = new Mat();
        private VideoCapture _cap = new VideoCapture();

        public MainViewModel()
        {
            RestoreCommand = new RelayCommand(
                () =>
                {
                    Preview = _original.Clone();
                    GrayscaleCommand?.RaiseCanExecuteChanged();
                },
                () => { return (!_original.IsEmpty); }
            );
            GrayscaleCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat();
                    CvInvoke.CvtColor(Preview, img, ColorConversion.Rgba2Gray);
                    Preview = img;
                    GrayscaleCommand?.RaiseCanExecuteChanged();
                },
                () => { return (!Preview.IsEmpty && Preview.NumberOfChannels == 3); }
            );
            BlurCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat();
                    CvInvoke.GaussianBlur(Preview, img, new System.Drawing.Size(5, 5), 1.5);
                    Preview = img;
                },
                () => { return (!Preview.IsEmpty); }
            );
            CannyCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat();
                    CvInvoke.Canny(Preview, img, 100, 200);
                    Preview = img;
                },
                () => { return (!Preview.IsEmpty); }
            );
            FacesCommand = new RelayCommand(
                () =>
                {
                    Mat img = Preview;
                    var faceDetector = new CascadeClassifier("haarcascade_frontalface_default.xml");
                    var faces = faceDetector.DetectMultiScale(img,1.1,3,new System.Drawing.Size(100,100));
                    foreach (var face in faces)
                    {
                        CvInvoke.Rectangle(img, new Rectangle(face.Left, face.Top, face.Width, face.Height), new Rgb(0, 0, 255).MCvScalar);
                    }
                    Preview = img;
                },
                () => { return (!Preview.IsEmpty); }
            );
            NormalizeGrayscaleCommand = new RelayCommand(
                () =>
                {
                    Mat gray = new Mat();
                    if (Preview.NumberOfChannels == 3)
                    {
                        CvInvoke.CvtColor(Preview, gray, ColorConversion.Rgba2Gray);
                    }
                    else
                    {
                        gray = Preview;
                    }
                    CvInvoke.EqualizeHist(gray, gray);
                    Preview = gray;
                },
                () => { return (!Preview.IsEmpty); }
            );
            RectangleCommand = new RelayCommand(
                () =>
                {

                    Mat img = Preview;
                    Random rng = new Random();
                    int x = rng.Next(img.Width);
                    int y = rng.Next(img.Height);
                    int w = rng.Next(0,img.Width - x);
                    int h = rng.Next(0,img.Height - y);
                    CvInvoke.Rectangle(img, new Rectangle(x,y,w,h), new Rgb(0, 0, 255).MCvScalar);
                    Preview = img;
                },
                () => { return (!Preview.IsEmpty); }
            );
            CaptureCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat();
                    _cap.Retrieve(img);
                    Preview = img;
                    RestoreCommand.RaiseCanExecuteChanged();
                    GrayscaleCommand.RaiseCanExecuteChanged();
                    BlurCommand.RaiseCanExecuteChanged();
                    CannyCommand.RaiseCanExecuteChanged();
                    FacesCommand.RaiseCanExecuteChanged();
                    NormalizeGrayscaleCommand.RaiseCanExecuteChanged();
                    RectangleCommand.RaiseCanExecuteChanged();
                },
                () => { return true; }
            );
        }

        public Mat Preview 
        {
            get 
            {
                return _current;
            }
            private set
            {
                _current = value;
                NotifyPropertyChanged();
            } 
        }

        public RelayCommand RestoreCommand { get; set; }
        public RelayCommand GrayscaleCommand { get; set; }
        public RelayCommand BlurCommand { get; set; }
        public RelayCommand CannyCommand { get; set; }
        public RelayCommand FacesCommand { get; set; }
        public RelayCommand CaptureCommand { get; set; }
        public RelayCommand NormalizeGrayscaleCommand { get; set; }
        public RelayCommand RectangleCommand { get; set; }

        public void LoadImage(string filename)
        {
            Preview = CvInvoke.Imread(filename, ImreadModes.AnyColor);
            _original = Preview.Clone();
            RestoreCommand.RaiseCanExecuteChanged();
            GrayscaleCommand.RaiseCanExecuteChanged();
            BlurCommand.RaiseCanExecuteChanged();
            CannyCommand.RaiseCanExecuteChanged();
            FacesCommand.RaiseCanExecuteChanged();
            NormalizeGrayscaleCommand.RaiseCanExecuteChanged();
            RectangleCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
