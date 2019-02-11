using System;
using System.Windows;
using System.Windows.Media;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for CommandResult.xaml
    /// </summary>
    public partial class CommandResult : Window
    {
        private Boolean valid;
        private String response;
        private String id;
        private String authToken;
        private String serviceURL;
        private String serviceName;
        private Type_of_Command cmdTYPE;
        public CommandResult(Boolean Valid,String Response,String ID, String AuthToken,String ServiceURL,String ServiceName, Type_of_Command CmdTYPE)
        {
            valid = Valid;
            response = Response;
            id = ID;
            authToken = AuthToken;
            serviceURL= ServiceURL;
            serviceName = ServiceName;
            cmdTYPE = CmdTYPE;
            switch (cmdTYPE)
            {
                case Type_of_Command.AddList:
                    cmdTYPE = Type_of_Command.DeleteList;
                    break;
                case Type_of_Command.AddSubscriber:
                    cmdTYPE = Type_of_Command.DeleteSubscriber;
                    break;
                case Type_of_Command.AddDataExtension:
                    cmdTYPE = Type_of_Command.DeleteDataExtension;
                    break;                
                case Type_of_Command.AddCampaign:
                    cmdTYPE = Type_of_Command.DeleteCampaign;
                    break;
                case Type_of_Command.AddDataExtensionObject:
                    cmdTYPE = Type_of_Command.DeleteDataExtensionObject;
                    break;                
                default:
                    cmdTYPE = Type_of_Command.SOQLcommand;
                    break;
            }
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();            
            this.Loaded += CommandResult_Loaded;
        }

        private void CommandResult_Loaded(object sender, RoutedEventArgs e)
        {
            if (valid)
            {
                if (cmdTYPE != Type_of_Command.Connection)
                {
                    lbStatus.Content = "Valid command";
                    lbStatus.Foreground = new SolidColorBrush(Colors.Green);
                    lbResponse.Content = "Response:";
                    lbID.Content = "ID:";
                    btDelete.Visibility = Visibility.Visible;
                }
                else
                {
                    lbStatus.Content = "Valid connection";
                    lbStatus.Foreground = new SolidColorBrush(Colors.Green);
                    lbResponse.Content = "AuthToken:";
                    lbID.Content = "ServiceURL:";
                    btDelete.Visibility = Visibility.Hidden;
                    btContinue.Margin = new Thickness(0, 0, 0, 15);
                    btContinue.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
            else
            {
                if (cmdTYPE != Type_of_Command.Connection)
                    lbStatus.Content = "Invalid command";
                else lbStatus.Content = "Invalid connection";
                lbStatus.Foreground = new SolidColorBrush(Colors.Red);
                lbResponse.Content = "Error1:";
                lbResponse.Foreground = new SolidColorBrush(Colors.Red);
                lbID.Content = "Error2:";
                lbID.Foreground = new SolidColorBrush(Colors.Red);
                btDelete.Visibility = Visibility.Hidden;
                btContinue.Margin = new Thickness(0, 0, 0, 15);
                btContinue.HorizontalAlignment = HorizontalAlignment.Center;
            }
            tbResponse.Text = response;
            tbID.Text = id;
            if (cmdTYPE == Type_of_Command.SOQLcommand)
            {
                btDelete.Visibility = Visibility.Hidden;
                btContinue.Margin = new Thickness(0, 0, 0, 15);
                btContinue.HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        private void btContinue_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btDelete_Click(object sender, RoutedEventArgs e)
        {
            btDelete.IsEnabled = false;
            btContinue.IsEnabled = false;
            var task = await(new CmdRestAPI(authToken, serviceURL, Salesforce_Marketing_Cloud_Scope.Design_SOAP, null,serviceName,tbID.Text,"",cmdTYPE)).ExecuteAsync();
            btDelete.IsEnabled = true;
            btContinue.IsEnabled = true;
            if (task.ValidConnection) Close();
        }
    }
}
