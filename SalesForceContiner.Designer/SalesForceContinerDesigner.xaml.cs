using System;
using System.Windows;


namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    // Interaction logic for ActivityDesigner1.xaml
    public partial class SalesforceLightningDesigner
    {
        public SalesforceLightningDesigner()
        {
            InitializeComponent();
        }

        private async void btConnection_Click(object sender, RoutedEventArgs e)
        {
            ConvertModelToString tmp = new ConvertModelToString();
            Salesforce_Marketing_Cloud_Scope.Design_ServiceURL = ""+tmp.ConvertModelItem(ModelItem.Properties["ServiceURL"].ComputedValue);
            Salesforce_Marketing_Cloud_Scope.Design_KEY = ""+tmp.ConvertModelItem(ModelItem.Properties["ConsumerKey"].ComputedValue);
            Salesforce_Marketing_Cloud_Scope.Design_SECRET = ""+tmp.ConvertModelItem(ModelItem.Properties["ConsumerSecret"].ComputedValue);
            Salesforce_Marketing_Cloud_Scope.Design_USER = "" + tmp.ConvertModelItem(ModelItem.Properties["UserName"].ComputedValue);
            Salesforce_Marketing_Cloud_Scope.Design_PASSWORD = "" + tmp.ConvertModelItem(ModelItem.Properties["Password"].ComputedValue);

            btConnection.IsEnabled = false;
            var task2 = await (new CmdRestAPI(new InitConnectionData().ReturnDict())).ExecuteAsync();

            CommandResult tmpwindow;
            if (task2.ValidConnection)
            {
                Salesforce_Marketing_Cloud_Scope.Design_AUTH = task2.RespAuthToken;
                Salesforce_Marketing_Cloud_Scope.Design_SERVICES = task2.RespServiceURL;
                Salesforce_Marketing_Cloud_Scope.Design_SOAP = task2.RespSoapClient;
                Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN = true;
                tmpwindow = new CommandResult(true, task2.RespAuthToken, task2.RespServiceURL, task2.RespAuthToken, task2.RespServiceURL, "", Type_of_Command.Connection);
            }
            else
            {
                Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN = false;
                tmpwindow = new CommandResult(false, task2.RespAuthToken, task2.RespServiceURL, task2.RespAuthToken, task2.RespServiceURL, "", Type_of_Command.Connection);
            }
            tmpwindow.Show();
            btConnection.IsEnabled = true;
        }
    }
}

