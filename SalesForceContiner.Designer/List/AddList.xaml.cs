using System;
using System.Windows;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for FilterDataTableDesigner.xaml
    /// </summary>
    public partial class AddList
    {
        public AddList()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConvertModelToString tmp = new ConvertModelToString();
            Int32 cmd_Type = Convert.ToInt32(ModelItem.Properties["cmdTYPE"].ComputedValue);
            ParametersWizard wizard = new ParametersWizard(ModelItem, cmd_Type, "", "");
            wizard.ShowOkCancel();
        }
    }
}
