using Emgu.CV;
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
        private BitmapSource? _current;
        private BitmapSource? _original;

        public MainViewModel()
        {
            RestoreCommand = new RelayCommand(
                () =>
                {
                    CurrentImage = OriginalImage.Clone();
                },
                () => { return (_original != null); }
            );
            GrayscaleCommand = new RelayCommand(
                () =>
                {
                    Bitmap inBmp = BitmapFromSource(OriginalImage);
                    Mat img = inBmp.ToMat();

                    Bitmap outBmp = new Bitmap(100,100);
                    img.ToBitmap(outBmp);
                    CurrentImage = ConvertBitmap(outBmp);
                },
                () => { return (_current != null); }
            );
        }

        public RelayCommand RestoreCommand { get; set; }
        public RelayCommand GrayscaleCommand { get; set; }
        public BitmapSource CurrentImage {
            get 
            { 
                return _current;
            } 
            set 
            {
                _current = value;
                NotifyPropertyChanged();
                GrayscaleCommand.RaiseCanExecuteChanged();
            } 
        }

        public BitmapSource OriginalImage
        {
            get
            {
                return _original;
            }
            set
            {
                _original = value;
                NotifyPropertyChanged();
                RestoreCommand.RaiseCanExecuteChanged();
            }
        }

        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
