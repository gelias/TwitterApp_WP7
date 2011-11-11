using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;

namespace TwitterWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void loadTweets_click(object sender, RoutedEventArgs e)
        {
            WebClient twitter = new WebClient();
	    String url = String.Format("http://twitter.com/statuses/user_timeline/{0}.{1}", txtTwitter.Text, "xml");
            Uri endereco = new Uri(url);
            twitter.DownloadStringCompleted +=new DownloadStringCompletedEventHandler(twitter_DownloadStringCompleted);
            twitter.DownloadStringAsync(endereco);
        }

        void twitter_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Falhou!!");
                return;
            }

            XElement xmlTweets = XElement.Parse(e.Result);

            var tweets = from tweet in xmlTweets.Descendants("status")
                         select new TwitterItem
                         {
                            ImageSource = tweet.Element("user").Element("profile_image_url").Value,
                            Message = tweet.Element("text").Value,
                            UserName = tweet.Element("user").Element("screen_name").Value
                         };

            lstTweets.ItemsSource = tweets;
        }
    }
}
