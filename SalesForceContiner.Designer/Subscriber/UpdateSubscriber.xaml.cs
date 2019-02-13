using System;
using System.Windows;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for FilterDataTableDesigner.xaml
    /// </summary>
    public partial class UpdateSubscriber
    {
        public UpdateSubscriber()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConvertModelToString tmp = new ConvertModelToString();
            Int32 cmd_Type = Convert.ToInt32(ModelItem.Properties["cmdTYPE"].ComputedValue);
            String ID2 = tmp.ConvertModelItem(ModelItem.Properties["SubcriberKey"].ComputedValue);
            if (ID2 == null) ID2 = "";
            ParametersWizard wizard = new ParametersWizard(ModelItem, cmd_Type, "", ID2);
            wizard.ShowOkCancel();
        }
    }
}
