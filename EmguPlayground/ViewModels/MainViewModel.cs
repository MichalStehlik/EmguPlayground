using Emgu.CV;
using Emgu.CV.CvEnum;
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
                    Mat img = new Mat(Preview.Size, DepthType.Cv8U, 1);
                    CvInvoke.CvtColor(Preview, img, ColorConversion.Rgba2Gray);
                    Preview = img;
                    GrayscaleCommand?.RaiseCanExecuteChanged();
                },
                () => { return (!Preview.IsEmpty && Preview.NumberOfChannels == 3); }
            );
            BlurCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat(Preview.Size, DepthType.Cv8U, 3);
                    CvInvoke.GaussianBlur(Preview, img, new System.Drawing.Size(5, 5), 1.5);
                    Preview = img;
                },
                () => { return (!Preview.IsEmpty); }
            );
            CannyCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat(Preview.Size, DepthType.Cv8U, 3);
                    CvInvoke.Canny(Preview, img, 100, 200);
                    Preview = img;
                },
                () => { return (!Preview.IsEmpty); }
            );
            FacesCommand = new RelayCommand(
                () =>
                {
                    Mat img = new Mat(Preview.Size, DepthType.Cv8U, 3);
                    CvInvoke.Canny(Preview, img, 100, 200);
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

        public void LoadImage(string filename)
        {
            Preview = CvInvoke.Imread(filename, ImreadModes.AnyColor);
            _original = Preview.Clone();
            RestoreCommand.RaiseCanExecuteChanged();
            GrayscaleCommand.RaiseCanExecuteChanged();
            BlurCommand.RaiseCanExecuteChanged();
            CannyCommand.RaiseCanExecuteChanged();
            FacesCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
