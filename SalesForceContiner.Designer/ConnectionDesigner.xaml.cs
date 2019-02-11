using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation.Model;
using System.Windows;


namespace SalesforceAPI.Designer
{
    // Interaction logic for AddAccountDesigner.xaml
    public partial class ConnectionDesigner
    {
        public ConnectionDesigner()
        {
            InitializeComponent();
        }

        private void TestConnClick(object sender, RoutedEventArgs e)
        {
            ConvertModelToString tmp = new ConvertModelToString();
            String UserName = tmp.ConvertModelItem(ModelItem.Properties["UserName"].ComputedValue);
            String Password = tmp.ConvertModelItem(ModelItem.Properties["Password"].ComputedValue);
            String SecurityToken = tmp.ConvertModelItem(ModelItem.Properties["SecurityToken"].ComputedValue);
            String ConsumerKey = tmp.ConvertModelItem(ModelItem.Properties["ConsumerKey"].ComputedValue);
            String ConsumerSecret = tmp.ConvertModelItem(ModelItem.Properties["ConsumerSecret"].ComputedValue);
            Int32 Server_Type = Convert.ToInt32(ModelItem.Properties["Server_Type"].ComputedValue);

            ConnectionDialog buildDataTableWnd = new ConnectionDialog(UserName,Password,SecurityToken,ConsumerKey,ConsumerSecret,Server_Type==0?true:false);
            buildDataTableWnd.ShowDialog();
        }

    }
  
}
