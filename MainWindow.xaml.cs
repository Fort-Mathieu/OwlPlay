using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OwlSongs
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isPlaying;
        private bool isFullScreen;
        private bool userIsDraggingSlider = false;
        private List<MediaItem> items = new List<MediaItem>();

        public MainWindow()
        {
            //Init
            InitializeComponent();
            isPlaying = false;
            isFullScreen = false;
            mediaElement.LoadedBehavior = MediaState.Manual;

            //Handle The media time
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        #region timer

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mediaElement.Source != null) && (mediaElement.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliderTime.Minimum = 0;
                sliderTime.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                sliderTime.Value = mediaElement.Position.TotalSeconds;
            }
        }

        #endregion

        #region event functions

        private void OnClickBefore(object sender, RoutedEventArgs e)
        {
            before();
        }

        private void OnClickStop(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void OnClickPlay(object sender, RoutedEventArgs e)
        {
            Play();
        }

        private void OnClickNext(object sender, RoutedEventArgs e)
        {
            next();
        }

        private void OnClickOpen(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Fichier Audio/Video|*.mp3;*.wav;*.mp4;*.3gp;*.avi;*.mov;*.flv;*.wmv;*.mpg|all files|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.FilterIndex = 1;
                openFileDialog.ShowDialog();

                if (openFileDialog.FileNames != null)
                {
                    stop();
                    items = new List<MediaItem>();
                    menuPlayList.ItemsSource = new List<MediaItem>();
                    menuPlayList2.ItemsSource = new List<MediaItem>();

                    foreach (string filename in openFileDialog.FileNames)
                    {
                        items.Add(new MediaItem(filename));
                    }
                    menuPlayList.ItemsSource = items;
                    menuPlayList2.ItemsSource = items;
                    mediaElement.Source = new Uri(@"" + openFileDialog.FileName);
                    Play();
                    buttonToEnable();
                }
            }
            catch (Exception)
            {
                Console.WriteLine(e.Source);
            }

        }

        private void OnValueChangedSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBlockTime.Text = TimeSpan.FromSeconds(sliderTime.Value).ToString(@"hh\:mm\:ss");
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            mediaElement.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void onDragStartedSlider(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void onDragCompletedSlider(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaElement.Position = TimeSpan.FromSeconds(sliderTime.Value);
        }

        private void onMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            userIsDraggingSlider = false;
            mediaElement.Position = TimeSpan.FromSeconds(sliderTime.Value);
        }

        private void OnMouseLeftButtonDownMedia(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                setFullScreen();
            }
        }

        private void OnClickForward(object sender, RoutedEventArgs e)
        {
            forward();
        }

        private void OnClickBackward(object sender, RoutedEventArgs e)
        {
            backward();
        }

        private void ListBoxMediaSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (menuPlayList.SelectedItem != null)
            {
                stop();
                mediaElement.Source = new Uri(@"" + (menuPlayList.SelectedItem as MediaItem).Media);
                Play();
            }
            else if (menuPlayList2.SelectedItem != null)
            {
                stop();
                mediaElement.Source = new Uri(@"" + (menuPlayList2.SelectedItem as MediaItem).Media);
                Play();
            }
        }

        private void OnMediaEnded(object sender, RoutedEventArgs e)
        {
            next();
        }

        //key event
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Space))
            {
                Play();
            }
            else if (e.Key.Equals(Key.Escape))
            {
                if (isFullScreen)
                {
                    setFullScreen();
                }
            }
            else if(e.Key.Equals(Key.Left))
            {
                backward();
            }
            else if (e.Key.Equals(Key.Right))
            {
                forward();
            }
        }

        #endregion

        #region App functions

        private void buttonToEnable()
        {
            buttonBefore.IsEnabled = true;
            buttonNext.IsEnabled = true;
            buttonPlay.IsEnabled = true;
            buttonStop.IsEnabled = true;
            menuForPlayList.IsEnabled = true;
            menuForPlayback.IsEnabled = true;
        }

        private void Play()
        {
            try
            {
                if (!isPlaying)
                {
                    /*var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/pause.png"));
                    buttonPlay.Background = brush;*/
                

                    mediaElement.Play();
                    mediaElement.Visibility = Visibility.Visible;
                    isPlaying = true;

                    buttonPlay.ToolTip = "Pause";
                    MenuPlay.Header = "Pause";
                }
                else
                {
                    /*var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/play.png"));
                    buttonPlay.Background = brush;*/
                    mediaElement.Pause();
                    isPlaying = false;

                    buttonPlay.ToolTip = "Play";
                    MenuPlay.Header = "Play";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur:" + ex.Message);
            }
        }

        private void setFullScreen()
        {
            if (!isFullScreen)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Normal;
                WindowState = WindowState.Maximized;
                ResizeMode = ResizeMode.NoResize;
                menuHeader.Visibility = Visibility.Collapsed;
                expander.Visibility = Visibility.Collapsed;
                isFullScreen = true;
                Cursor = Cursors.None;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                ResizeMode = ResizeMode.CanResize;
                menuHeader.Visibility = Visibility.Visible;
                expander.Visibility = Visibility.Visible;
                isFullScreen = false;
                Cursor = Cursors.Arrow;
            }
        }

        private void backward()
        {
            mediaElement.Position = TimeSpan.FromSeconds(sliderTime.Value) - TimeSpan.FromSeconds(30);
        }

        private void forward()
        {
            mediaElement.Position = TimeSpan.FromSeconds(sliderTime.Value) + TimeSpan.FromSeconds(30);
        }

        private void stop()
        {
            try
            {
                if (isPlaying)
                {
                    mediaElement.Position = TimeSpan.FromSeconds(0);
                    mediaElement.Visibility = Visibility.Hidden;
                    mediaElement.Stop();
                    isPlaying = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur:" + ex.Message);
            }
        }

        private void next()
        {
            Uri before = mediaElement.Source;
            bool isNext = false;
            foreach (MediaItem item in items)
            {
                Uri newUri = new Uri(@"" + item.Media);
                if (!isNext)
                {
                    if (newUri.Equals(before))
                    {
                        isNext = true;
                    }
                }
                else
                {
                    mediaElement.Source = new Uri(@"" + item.Media);
                    isNext = false;
                }

            }
        }

        private void before()
        {
            Uri actual = mediaElement.Source;
            Uri beforeUri = mediaElement.Source; ;
                foreach (MediaItem item in items)
                {
                    Uri newUri = new Uri(@"" + item.Media);
                    if (actual.Equals(newUri))
                    {
                        mediaElement.Source = beforeUri;
                    }
                    else
                    {
                        beforeUri = newUri;   
                    }

                }
            
        }

        #endregion

        private void OnClickDecrease(object sender, RoutedEventArgs e)
        {
            mediaElement.IsMuted = false;
            mediaElement.Volume -= 0.1;
        }

        private void OnClickIncrease(object sender, RoutedEventArgs e)
        {
            mediaElement.IsMuted = false;
            mediaElement.Volume += 0.1;
        }

        private void OnClickQuit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OnClickMute(object sender, RoutedEventArgs e)
        {
            if (mediaElement.IsMuted)
            {
                mediaElement.IsMuted = false;
            }
            else
            {
                mediaElement.IsMuted = true;
            }
        }

        private void OnDropFile(object sender, DragEventArgs e)
        {
            try
            {
                string[] droppedFiles = null;
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                }

                if ((null == droppedFiles) || (!droppedFiles.Any())) { return; }

                foreach (string s in droppedFiles)
                {
                    items.Add(new MediaItem(s));
                }

                menuPlayList.ItemsSource = items;
                menuPlayList2.ItemsSource = items;
                if (!isPlaying)
                {
                    mediaElement.Source = new Uri(@"" + items[0].Media);
                    Play();
                    buttonToEnable();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("erreur");
            }
        }
    }
}
