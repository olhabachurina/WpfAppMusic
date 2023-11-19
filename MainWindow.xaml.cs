using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using static WpfAppMusic.ViewModel;

namespace WpfAppMusic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Album
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
    }

    public class TrackInfo
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public int PlayCount { get; set; }
    }

    public partial class MainWindow : Window
    {
        private string selectedArtist;
       
        public MainWindow()
        {
            InitializeComponent();
           
            ObservableCollection<string> artists = GetArtists(); 
            foreach (var artist in artists)
            {
                artistsListBox.Items.Add(artist);
            }
        }

        private ObservableCollection<string> GetArtists()
        {
            return new ObservableCollection<string> { "Sting", "Hauser",  };
        }

        private void ClearAlbumsAndTracks()
        {
            albumsListBox.Items.Clear();
            ClearTracks();
        }

        private void ArtistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedArtist = artistsListBox.SelectedItem?.ToString();
            ClearAlbumsAndTracks();

            if (!string.IsNullOrEmpty(selectedArtist))
            {
                ObservableCollection<Album> albums = GetAlbumsForArtist(selectedArtist);
                foreach (var album in albums)
                {
                    string imagePath = System.IO.Path.Combine(@"C:\Users\Acer\source\repos\WpfAppMusic\", album.ImagePath);

                    try
                    {
                        if (File.Exists(imagePath))
                        {
                            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                            bitmapImage.DownloadFailed += (s, args) =>
                            {
                                MessageBox.Show($"Failed to download image: {args.ErrorException.Message}");
                            };

                            Image albumCover = new Image
                            {
                                Source = bitmapImage,
                                Width = 100,
                                Height = 100,
                                Margin = new Thickness(0, 0, 0, 10)
                            };

                            TextBlock albumTitleTextBlock = new TextBlock
                            {
                                Text = album.Title,
                                HorizontalAlignment = HorizontalAlignment.Center,
                                Margin = new Thickness(0, 0, 0, 5)
                            };

                            albumCover.MouseDown += AlbumCover_MouseDown;
                            albumCover.Tag = album;

                            
                            albumsListBox.Items.Add(albumCover);
                            albumsListBox.Items.Add(albumTitleTextBlock);
                        }
                        else
                        {
                            MessageBox.Show($"Image file not found: {imagePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading image: {ex.Message}");
                    }
                }
            }
        }

    private ObservableCollection<Album> GetAlbumsForArtist(string artist)
        {
            switch (artist)
            {
                case "Sting":
                    return new ObservableCollection<Album>
    {
        new Album { Artist = "Sting", Title = "The very best of Sting and The Police", ImagePath = "Sting1.jpg" },
        new Album { Artist = "Sting", Title = "The best of Sting", ImagePath = "Sting2.jpg" }
    };
                   
                case "Hauser":
                    return new ObservableCollection<Album>
            {
                new Album { Artist = "Hauser", Title = "Classic",ImagePath = "Hauser1.jpg" },
                new Album { Artist = "Hauser", Title = "Christmas",ImagePath = "Hauser2.jpg" },
            };
                    break;
                default:
                    return new ObservableCollection<Album>();
            }
        }

        private void AlbumCover_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Album selectedAlbum = (sender as Image).Tag as Album;

            ClearTracks();

            if (selectedAlbum != null)
            {
                ObservableCollection<TrackInfo> tracks = GetTracksInfoForAlbum(selectedAlbum.Artist, selectedAlbum.Title);

                foreach (var track in tracks)
                {
                    TextBlock trackTextBlock = new TextBlock
                    {
                        Text = $"{track.Title} - {track.Duration} - {track.PlayCount} прослушиваний",
                        Margin = new Thickness(0, 0, 0, 5)
                    };

                    trackTextBlock.MouseDown += TrackTextBlock_MouseDown;
                    trackTextBlock.Tag = track;

                    tracksListBox.Items.Add(trackTextBlock);
                }
            }
        }

        private void ClearTracks()
        {
            tracksListBox.Items.Clear();
           
        }

       

        private ObservableCollection<TrackInfo> GetTracksInfoForAlbum(string artist, string album)
        {
            ObservableCollection<TrackInfo> tracksInfo = new ObservableCollection<TrackInfo>();

            if (artist == "Sting" && album == "The very best of Sting and The Police")
            {
                tracksInfo.Add(new TrackInfo { Title = "Seven days", Duration = "4:39", PlayCount = 150159 });
                tracksInfo.Add(new TrackInfo { Title = "Fragile", Duration = "3:53", PlayCount = 382451 });
            }
            else if (artist == "Sting" && album == "The best of Sting")
            {
                tracksInfo.Add(new TrackInfo { Title = "Message in a Bottle", Duration = "4:48", PlayCount = 10374448 });
                tracksInfo.Add(new TrackInfo { Title = "Can't Stand Losing You", Duration = "2:55", PlayCount = 512069 });
            }
            else if (artist == "Hauser" && album == "Classic") 
            {
                tracksInfo.Add(new TrackInfo { Title = "Swan Lake", Duration = "02:53", PlayCount = 658000 });
                tracksInfo.Add(new TrackInfo { Title = "Caruso", Duration = "05:30", PlayCount = 459653});
            }
            else if (artist == "Hauser" && album == "Christmas") 
            {
                tracksInfo.Add(new TrackInfo { Title = "The first Noel", Duration = "04:13", PlayCount = 356459});
                tracksInfo.Add(new TrackInfo { Title = "White Christmas", Duration = "02;44", PlayCount = 468150 });
            }

            return tracksInfo;
        }

        private static string GetAlbumCoverPath(string selectedArtist, string album)
        {
            selectedArtist = selectedArtist.Replace(" ", "_");
            album = album.Replace(" ", "_");
            string imagePath = System.IO.Path.Combine(@"C:\Users\Acer\source\repos\WpfAppMusic\", $"{selectedArtist}-{album}.jpg");
            return imagePath;
        }
    
            private void TrackTextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
        }
    }
}