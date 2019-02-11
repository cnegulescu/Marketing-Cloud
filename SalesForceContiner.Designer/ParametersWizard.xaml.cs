using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for FilterDataTableWizzard.xaml
    /// </summary>
    
    #region ViewModel

    public class ParametersViewModel : INotifyPropertyChanged
    {

        private int _index;

        ModelItem _parameter, _value;
        public Boolean isEnabled { get; set; } = true;

        public Visibility visi { get; set; } = Visibility.Collapsed;

        public ModelItem Parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
                NotifyPropertyChanged();
            }
        }

        public ModelItem ValueData
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    public partial class ParametersWizard : WorkflowElementDialog
    {
        #region Members
        public ObservableCollection<ParametersViewModel> Parameters { get; set; } = new ObservableCollection<ParametersViewModel>();

        private const string ParametersProperty = "Parameters";

        public string AuthToken;
        public string ServiceURL;
        public string NameDecision="";
        public string IdDecision="";
        public string ServiceName = "";
        private Boolean EmptyParameter = false;
        public Type_of_Command CmdTYPE=Type_of_Command.AddList;
        private Type_of_Command cmdTypeMandatory = Type_of_Command.AddList;
        private CheckIDWizard paramDialogWindow = null;

        #endregion

        #region Constructor 

        public ParametersWizard(ModelItem ownerActivity, Int32 CmdType,String Name,String Id)
        {
            InitializeComponent();
            this.ModelItem = ownerActivity;
            this.Context = ownerActivity.GetEditingContext();
            // Initialize Parameters
            InitializeParameters();
            if (Parameters.Count == 0)
            {
                EmptyParameter = true;
                Parameters.Add(new ParametersViewModel());
            }
            UpdateParameterIndex(0);
            if (Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN)
            {
                AuthToken = Salesforce_Marketing_Cloud_Scope.Design_AUTH;
                ServiceURL = Salesforce_Marketing_Cloud_Scope.Design_SERVICES;
            }
            CmdTYPE = (Type_of_Command)CmdType;
            NameDecision = Name;
            IdDecision = Id;

            switch (CmdTYPE)
            {
                case Type_of_Command.AddList:
                case Type_of_Command.UpdateList:
                    ServiceName = "List";
                    cmdTypeMandatory = Type_of_Command.GetMandatoryList;
                    break;
                case Type_of_Command.AddSubscriber:
                case Type_of_Command.UpdateSubscriber:
                    ServiceName = "Subscriber";
                    cmdTypeMandatory = Type_of_Command.GetMandatorySubscriber;
                    break;
                case Type_of_Command.AddDataExtension:
                case Type_of_Command.UpdateDataExtension:
                    ServiceName = "DataExtension";
                    cmdTypeMandatory = Type_of_Command.GetMandatoryDataExtension;
                    break;
                case Type_of_Command.AddDataExtensionObject:
                case Type_of_Command.UpdateDataExtensionObject:
                    ServiceName = "DataExtensionObject";
                    cmdTypeMandatory = Type_of_Command.GetMandatoryDataExtensionObject;
                    break;
                case Type_of_Command.AddCampaign:
                case Type_of_Command.UpdateCampaign:
                    ServiceName = "campaigns";
                    cmdTypeMandatory = Type_of_Command.GetMandatoryCampaign;
                    break;                
                default:                    
                    break;
            }            
            // if ((CmdTYPE == Type_of_Command.AddCustom) || (CmdTYPE == Type_of_Command.UpdateCustom))
            //     btnMandatory.IsEnabled = false;
        }

        #endregion

        #region Initialize/Save Methods

        private void InitializeParameters()
        {
            var ParamElem = ModelItem.Properties[ParametersProperty];
            var ParametersREST = ParamElem?.Value?.GetCurrentValue() as List<ParametersArgument>;
            if (ParametersREST == null)
            {
                return;
            }

            foreach (var itemREST in ParametersREST)
            {
                var param = new ParametersViewModel()
                {
                    Parameter = ArgumentToModelItem(itemREST.Parameter, Context),
                    ValueData = ArgumentToModelItem(itemREST.ValueData, Context),    
                    isEnabled = itemREST.isEnabled,
                    visi = itemREST.visi,
                };
                Parameters.Add(param);
            }
        }

        private void SaveParameters()
        {
            var res = new List<ParametersArgument>();
            foreach (var grid in Parameters)
            {
                var param = new ParametersArgument
                {
                    Parameter = ModelItemToArgument(grid.Parameter),
                    ValueData = ModelItemToArgument(grid.ValueData),
                    isEnabled = grid.isEnabled,
                    visi = grid.visi,
                };
                res.Add(param);
            }
            ModelItem.Properties[ParametersProperty].SetValue(res);
        }
        
        #endregion

        #region Event Handlers

        protected override void OnWorkflowElementDialogClosed(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                SaveParameters();
            }
            if (paramDialogWindow != null) paramDialogWindow.Close();
            base.OnWorkflowElementDialogClosed(dialogResult);            
        }


        private void ConnectClick(object sender, System.Windows.RoutedEventArgs e)
        {
            EmptyParameter = false;
            var index = Parameters.Count;
            Parameters.Add(new ParametersViewModel());
            UpdateParameterIndex(index);
        }

        public void AddParameter(String ParameterName)
        {
            var index = Parameters.Count;
            if (EmptyParameter) Parameters.RemoveAt(0);
            
            EmptyParameter = false;
            var param = new ParametersViewModel()
            {
                Parameter = ArgumentToModelItem(new InArgument<string>(ParameterName), Context),
                ValueData = ArgumentToModelItem(new InArgument<string>(""), Context),
            };
            Parameters.Add(param);
            UpdateParameterIndex(index);
        }

        private async void GetDataClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN)
            {
                Init_Connection();
                var task2 = await(new CmdRestAPI(dictionaryForUrl)).ExecuteAsync();

                if (task2.ValidConnection)
                {
                    AuthToken = task2.RespAuthToken;
                    ServiceURL = task2.RespServiceURL;
                    Salesforce_Marketing_Cloud_Scope.Design_AUTH = AuthToken;
                    Salesforce_Marketing_Cloud_Scope.Design_SERVICES = ServiceURL;
                    Salesforce_Marketing_Cloud_Scope.Design_SOAP = task2.RespSoapClient;
                    Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN = true;
                }
                System.Threading.Thread.Sleep(500);
            }
            
            paramDialogWindow = new CheckIDWizard(AuthToken,ServiceURL,CmdTYPE,NameDecision,IdDecision, this, ServiceName, cmdTypeMandatory);
            paramDialogWindow.Show();
        }

        private void RemoveParameterClick(object sender, RoutedEventArgs e)
        {
            if (Parameters.Count == 1)
            {
                if (EmptyParameter) return;
                else
                {
                    Parameters.RemoveAt(0);
                    Parameters.Add(new ParametersViewModel());
                    UpdateParameterIndex(1);
                    EmptyParameter = true;
                    return;
                }
            }
            var index = GetCurrentItemIndex(sender);
            if (Parameters[index].isEnabled)
            {
                Parameters.RemoveAt(index);
                UpdateParameterIndex(index);
            }
            else
            {
                if (index == (Parameters.Count - 1))
                {
                    Parameters.RemoveAt(index);
                    UpdateParameterIndex(index);
                }
                else if (!Parameters[index + 1].isEnabled)
                {
                    Parameters.RemoveAt(index);
                    UpdateParameterIndex(index);
                }
                else
                {
                    MessageBox.Show("Please delete first all the attributes from this Set.");
                }
            }
        }

        private void UpdateParameterIndex(int index)
        {
            for (var i = index; i < Parameters.Count; ++i)
            {
                Parameters[i].Index = i;
            }
        }

        #endregion

        #region Helpers

        protected InArgument ModelItemToArgument(ModelItem value)
        {
            var valueExpressionModelItem = value as ModelItem;
            return valueExpressionModelItem?.GetCurrentValue() as InArgument;
        }

        protected ModelItem ArgumentToModelItem(InArgument arg, EditingContext editingContext)
        {
            return arg != null ? ModelFactory.CreateItem(editingContext, arg) : null;
        }

        private int GetCurrentItemIndex(object sender)
        {
            var button = sender as Button;
            if (button == null)
            {
                return -1;
            }
            return (int)button.Tag;
        }

        private Dictionary<String,String> dictionaryForUrl;
        #endregion

        private void Init_Connection()
        {
            String sfdcConsumerkey = Salesforce_Marketing_Cloud_Scope.Design_KEY;
            String sfdcConsumerSecret = Salesforce_Marketing_Cloud_Scope.Design_SECRET;

            dictionaryForUrl = new Dictionary<String, String>
                        {
                            {"client_id", sfdcConsumerkey},
                            {"client_secret", sfdcConsumerSecret},
                        };            
        }

        private async void Run_cmd_Click(object sender, RoutedEventArgs e)
        {
            String Error = "";
            btnRunCMD.Content = "Wait ...";
            btnMandatory.IsEnabled = false;
            btnAddParam.IsEnabled = false;
            btnGetExample.IsEnabled = false;
            btnRunCMD.IsEnabled = false;

            var newDataTable = new System.Data.DataTable();
            newDataTable.Columns.Add(new DataColumn("Parameter", typeof(string)) { MaxLength = 200 });
            newDataTable.Columns.Add(new DataColumn("Value", typeof(string)) { MaxLength = 60000 });
            newDataTable.Columns.Add(new DataColumn("isEnabled", typeof(Boolean)) { DefaultValue = true });
            ConvertModelToString tmp = new ConvertModelToString();
            Boolean ValidParam;
            foreach (ParametersViewModel item in Parameters)
            {
                ValidParam = true;
                try
                {
                    var row = newDataTable.NewRow();
                    String str = tmp.ConvertModelItem(ModelItemToArgument(item.Parameter));
                    if (str.Length > 1)
                    {
                        if (str[0] == '"')
                        {
                            str = str.Remove(0, 1);
                            str = str.Remove(str.Length - 1, 1);
                        }
                    }
                    if (str.Trim().Length < 1) ValidParam = false;
                    row["Parameter"] = str;
                    str = tmp.ConvertModelItem(ModelItemToArgument(item.ValueData));
                    if (str.Length > 1)
                    {
                        if (str[0] == '"')
                        {
                            str = str.Remove(0, 1);
                            str = str.Remove(str.Length - 1, 1);
                        }
                    }
                    row["Value"] = str;
                    row["isEnabled"] = item.isEnabled;
                    if (ValidParam)
                        newDataTable.Rows.Add(row);
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                }
            }

            Boolean valid = false;
            var task = await (new CmdRestAPI(AuthToken, ServiceURL, Salesforce_Marketing_Cloud_Scope.Design_SOAP, newDataTable, ServiceName, IdDecision, NameDecision, CmdTYPE)).ExecuteAsync();

            if (task.ValidConnection) valid = true;              
            else
            {
                if (!Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN)
                {
                    Init_Connection();
                    var task2 = await (new CmdRestAPI(dictionaryForUrl)).ExecuteAsync();

                    if (task2.ValidConnection)
                    {
                        AuthToken = task2.RespAuthToken;
                        ServiceURL = task2.RespServiceURL;
                        Salesforce_Marketing_Cloud_Scope.Design_AUTH = AuthToken;
                        Salesforce_Marketing_Cloud_Scope.Design_SERVICES = ServiceURL;
                        Salesforce_Marketing_Cloud_Scope.Design_SOAP = task2.RespSoapClient;
                        Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN = true;
                    }
                    System.Threading.Thread.Sleep(500);
                    task = await (new CmdRestAPI(AuthToken, ServiceURL, task2.RespSoapClient, newDataTable, ServiceName, IdDecision, NameDecision, CmdTYPE)).ExecuteAsync();
                    if (task.ValidConnection) valid = true;
                }
                else
                {                    
                    CommandResult tmpwindow = new CommandResult(false, task.Response, task.ID, AuthToken, ServiceURL, ServiceName, CmdTYPE);
                    tmpwindow.Show();
                }
            }
            if (valid) //MessageBox.Show("Valid command." + Environment.NewLine + Environment.NewLine + "Response:      " + task.Response + Environment.NewLine + Environment.NewLine + "ID:   " + task.ID);
            {
                CommandResult tmpwindow = new CommandResult(true, task.Response, task.ID, AuthToken, ServiceURL, ServiceName, CmdTYPE);
                tmpwindow.Show();
            }

            btnRunCMD.Content = "Run command";
            btnMandatory.IsEnabled = true;
            btnAddParam.IsEnabled = true;
            btnGetExample.IsEnabled = true;
            btnRunCMD.IsEnabled = true;
        }

        private async void Get_rules_Click(object sender, RoutedEventArgs e)
        {
            btnMandatory.Content = "Wait for data...";
            btnMandatory.IsEnabled = false;
            btnAddParam.IsEnabled = false;
            btnGetExample.IsEnabled = false;
            btnRunCMD.IsEnabled = false;
            
            Boolean valid = false;
            var task = await (new CmdRestAPI(AuthToken, ServiceURL, cmdTypeMandatory, ServiceName)).ExecuteAsync();

            if (task.ValidConnection)  valid = true;            
            else
            {
                if (!Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN)
                {
                    Init_Connection();
                    var task2 = await (new CmdRestAPI(dictionaryForUrl)).ExecuteAsync();

                    if (task2.ValidConnection)
                    {
                        AuthToken = task2.RespAuthToken;
                        ServiceURL = task2.RespServiceURL;
                        Salesforce_Marketing_Cloud_Scope.Design_AUTH = AuthToken;
                        Salesforce_Marketing_Cloud_Scope.Design_SERVICES = ServiceURL;
                        Salesforce_Marketing_Cloud_Scope.Design_SOAP = task2.RespSoapClient;
                        Salesforce_Marketing_Cloud_Scope.Design_VALIDCONN = true;
                    }
                    System.Threading.Thread.Sleep(500);
                    task = await (new CmdRestAPI(AuthToken, ServiceURL, cmdTypeMandatory, ServiceName)).ExecuteAsync();
                    if (task.ValidConnection) valid = true;
                }
                else
                {
                    MessageBox.Show("Error on reading the parameters.");
                }
            }

            if ((valid)&&(task.DataTableResp.Rows.Count>0))
            {
                Parameters.Clear();
                foreach (DataRow row in task.DataTableResp.Rows)
                {
                    var param = new ParametersViewModel()
                    {
                        Parameter = ArgumentToModelItem(new InArgument<string> { Expression = new Literal<string>(row[0].ToString()) }, Context),
                        ValueData = ArgumentToModelItem(Convert.ToInt32(row[1].ToString()) == 0 ? new InArgument<string> {} : new InArgument<string> { Expression = new Literal<string>("") }, Context),
                        isEnabled = (Convert.ToInt32(row[1].ToString())==0?false:true),
                        visi = (Convert.ToInt32(row[1].ToString()) == 0 ? Visibility.Visible : Visibility.Collapsed),
                    };
                    Parameters.Add(param);
                }
                UpdateParameterIndex(0);
                SaveParameters();
                EmptyParameter = false;
            }

            btnMandatory.Content = "Get mandatory parameters";
            btnMandatory.IsEnabled = true;
            btnAddParam.IsEnabled = true;
            btnGetExample.IsEnabled = true;
            btnRunCMD.IsEnabled = true;
        }

        private void AddParameterClick2(object sender, RoutedEventArgs e)
        {            
            var index = GetCurrentItemIndex(sender)+1;         
            EmptyParameter = false;
            Parameters.Insert(index,new ParametersViewModel() {Index=index});
            UpdateParameterIndex(index+1);
        }
    }
}
