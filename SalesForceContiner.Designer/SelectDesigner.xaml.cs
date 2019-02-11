using System;
using System.Activities;
using System.Activities.Expressions;
using System.Data;
using System.Windows;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for FilterDataTableDesigner.xaml
    /// </summary>
    public partial class SelectDesigner
    {
        public SelectDesigner()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            String Error = "";
            btTest.IsEnabled = false;
            try
            {
                if ((Salesforce_Marketing_Cloud_Scope.Design_AUTH == null) || (Salesforce_Marketing_Cloud_Scope.Design_AUTH.Trim().Length < 1))
                {
                    var task2 = await(new CmdRestAPI(new InitConnectionData().ReturnDict())).ExecuteAsync();
                    if (task2.ValidConnection)
                    {
                        Salesforce_Marketing_Cloud_Scope.Design_AUTH = task2.RespAuthToken;
                        Salesforce_Marketing_Cloud_Scope.Design_SERVICES = task2.RespServiceURL;
                        Salesforce_Marketing_Cloud_Scope.Design_SOAP = task2.RespSoapClient;
                        Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }            
            btTest.IsEnabled = true;
            ConvertModelToString tmp = new ConvertModelToString();
            String search = tmp.ConvertModelItem(ModelItem.Properties["Search"].ComputedValue);
            if (search == null) search = "";
            SelectWizard wizard = new SelectWizard(Salesforce_Marketing_Cloud_Scope.Design_AUTH, Salesforce_Marketing_Cloud_Scope.Design_SERVICES, search);
            wizard.ShowDialog();
            if (wizard.SaveSearch)
                ModelItem.Properties["Search"].SetValue(new InArgument<string> { Expression = new Literal<string>(wizard.SelectCMD) });
        }
    }
}