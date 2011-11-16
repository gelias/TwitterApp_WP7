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
using System.IO;
using System.Text;

using System.Runtime.Serialization.Json;
using System.ComponentModel;

namespace TwitterWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string TwitterSearchTweetURL = "http://twitter.com/statuses/user_timeline/{0}.{1}";
        private const string TwitterSearchHashtagURL = "http://search.twitter.com/search.json?q={0}";
        private const string XmlFormat = "xml";
        private WebClient twitter;

        public MainPage()
        {
            InitializeComponent();
            twitter = new WebClient();
            BtnFindTwitt.IsEnabled = !String.IsNullOrWhiteSpace(TxtTwitter.Text);
            BtnFindHashtag.IsEnabled = !String.IsNullOrWhiteSpace(TxtHashtag.Text);
        }

        private void loadTweets(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TxtTwitter.Text))
            {
                MessageBox.Show("Do you need type twitter alias or hastag before!");
                return;
            }

            String url = BuildSearchingForTwitterLoginURL();
            Uri endereco = new Uri(url);
            twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadTweetsFromTweetNameAsStringWhenCompleted);
            twitter.DownloadStringAsync(endereco);
        }

        private void loadHashtag(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(TxtHashtag.Text))
            {
                MessageBox.Show("Do you need type twitter alias or hastag before!");
                return;
            }

            String url = BuildSearchingForTwittersHashtag();
            Uri endereco = new Uri(url);
            twitter.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadTweetsFromHashtagAsStringWhenCompleted);
            twitter.DownloadStringAsync(endereco);
        }

        string BuildSearchingForTwitterLoginURL()
        {
            return String.Format(TwitterSearchTweetURL, TxtTwitter.Text.Trim(), XmlFormat);
        }

        string BuildSearchingForTwittersHashtag()
        {
            return String.Format(TwitterSearchHashtagURL, TxtHashtag.Text);
        }

        void DownloadTweetsFromTweetNameAsStringWhenCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (HasErrors(e))
                return;

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

        void DownloadTweetsFromHashtagAsStringWhenCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (HasErrors(e))
                return;

            MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(e.Result));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterItem));


            /*twitter.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                try
                {
                    if (HasErrors(e))
                        return;

                    // convert json result to model
                    Stream stream = e.Result;
                    DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(TwitterItem));
                    TwitterItem[] twitterResults = (TwitterItem[])dataContractJsonSerializer.ReadObject(stream);
                }
                finally
                {
                    MessageBox.Show("Finished");
                }
            };

            string encodedSearchText = HttpUtility.UrlEncode(TxtHashtag.Text);
            twitter.OpenReadAsync(new Uri(string.Format(TwitterSearchHashtagURL, encodedSearchText)));*/
        }

        #region Control Buttons

        private void EnableFindTweetButton(object sender, KeyEventArgs e)
        {
            TxtHashtag.Text = String.Empty;
            BtnFindTwitt.IsEnabled = TxtTwitter.Text.Length > 0;
        }

        private void EnableFindHashtagButton(object sender, KeyEventArgs e)
        {
            TxtTwitter.Text = String.Empty;
            BtnFindTwitt.IsEnabled = TxtTwitter.Text.Length > 0;
        }

        #endregion

        bool HasErrors(AsyncCompletedEventArgs e)
        {
            bool hasErrors = e.Error != null;
            if (hasErrors)
                MessageBox.Show("Falhou!!");

            return hasErrors;
        }
    }
}
