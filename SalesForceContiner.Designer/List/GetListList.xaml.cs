using System;
using System.Activities;
using System.Activities.Expressions;
using System.Windows;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for FilterDataTableDesigner.xaml
    /// </summary>
    public partial class GetListList
    {
        private String AuthToken = "";
        private String ServiceURL = "";
        public GetListList()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN)
            {
                AuthToken = Salesforce_Marketing_Cloud_Scope.Design_AUTH;
                ServiceURL = Salesforce_Marketing_Cloud_Scope.Design_SERVICES;
            }
            ConvertModelToString tmp = new ConvertModelToString();
            String search = tmp.ConvertModelItem(ModelItem.Properties["Search"].ComputedValue);
            if (search == null) search = "";            
            if (search.Trim().Length < 2)
                search = "SELECT ListName,Description,ID,PartnerKey,CreatedDate,ModifiedDate,Client.ID,Category,Type FROM List WHERE ID>1";
            SelectWizard wizard = new SelectWizard(AuthToken, ServiceURL, search, "List");
            
            wizard.ShowDialog();
            if (wizard.SaveSearch)
                ModelItem.Properties["Search"].SetValue(new InArgument<string> { Expression = new Literal<string>(wizard.SelectCMD) });
        }
    }
}
