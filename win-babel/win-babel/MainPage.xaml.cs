using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace win_babel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private MobileServiceUser user;
        private HubConnection hubConnection;
        private IHubProxy proxy;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            hubConnection = new HubConnection(App.MobileService.ApplicationUri.AbsoluteUri);
            if (user != null)
            {
                hubConnection.Headers["x-zumo-auth"] = user.MobileServiceAuthenticationToken;
            }
            else
            {
                hubConnection.Headers["x-zumo-application"] = App.MobileService.ApplicationKey;
            }
            proxy = hubConnection.CreateHubProxy("ChatHub");
        }

        private async Task ConnectToSignalR()
        {

            
            await hubConnection.Start(new LongPollingTransport());

            proxy.On<string>("hello", async msg =>
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
               {
                   Chats.Text += msg + "\n";
                   
               });
            });
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
           // await AuthenticateAsync();
     
            await ConnectToSignalR();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var m = new ChatMessage { From = App.User, Message = Message.Text };
            var token = JToken.FromObject(m);

            await App.MobileService.InvokeApiAsync("Chat",token, HttpMethod.Post, null);
        }
    }

    public class ChatMessage
    {
        public string From { get; set; }
        public string Message { get; set; }
    }
}
