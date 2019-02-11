using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CheckIDWizard : Window
    {
        private String authToken;
        private String serviceURL;
        private Type_of_Command cmdTYPE;
        private String name;
        private String id;
        private Boolean firsttime = false;
        private String nameOfService = "";
        private ParametersWizard parentWindow;        
        private Type_of_Command cmdTYPEMandatory;
        private DataTable dataTable;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private Hashtable HashGeneralData;
        public ObjectFromSalesforce objToReturn;

        public CheckIDWizard(String AuthToken, String ServiceURL, Type_of_Command CmdTYPE, String Name, String ID, ParametersWizard ParentWindow, String NameOfService, Type_of_Command CmdTYPEMandatory)
        {
            cmdTYPE = CmdTYPE;
            InitializeComponent();
            authToken = AuthToken;
            serviceURL = ServiceURL;
            id = ID;
            name = Name;
            parentWindow = ParentWindow;
            nameOfService = NameOfService;
            cmdTYPEMandatory = CmdTYPEMandatory;
            if (cmdTYPE != Type_of_Command.CheckID)
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                this.Topmost = false;
                this.Title = "Check ID in " + name;
            }
        }

        public CheckIDWizard(Type_of_Command CmdTYPE,String ObjectName, Hashtable HashData)
        {
            cmdTYPE = Type_of_Command.JustDisplay;
            HashGeneralData = HashData;
            InitializeComponent();
            this.Title = "List of fields from " + ObjectName;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            parentWindow = null;
            objToReturn = null;
        }

        private async void ButtonClickLogic()
        {
            List<String> tmpList = new List<string>();
            String Account_ID = "";
            String restCallURL = "";
            String Error = "";
            HttpRequestMessage apirequest;
            HttpResponseMessage apiCallResponse;
            String requestresponse;

            tbFilter.Clear();
            btGetData.IsEnabled = false;
            btGetData.Content = "Wait...";
            ResultGridView.IsEnabled = false;
            ckName.IsEnabled = false;
            tbName.IsEnabled = false;
            tbFilter.IsEnabled = false;

            /*HttpClient apiCallClient = new HttpClient();
            if (!firsttime)
            {
                try
                {
                    if ((ckName.IsChecked ?? false) && tbName.Text.Trim().Length > 0)
                    {
                        if ((Int32)cmdTYPE == 0)
                        {
                            restCallURL = serviceURL + "/services/data/v1/parameterizedSearch/?q=" + tbName.Text.Trim().Replace(' ', '+') + "&sobject=Account&Account.fields=id,name&Account.limit=1";
                            apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                            apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                            apiCallResponse = await apiCallClient.SendAsync(apirequest);

                            requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                            if (apiCallResponse.IsSuccessStatusCode)
                            {
                                dynamic list = JsonConvert.DeserializeObject(requestresponse);

                                foreach (var item in list.searchRecords)
                                {
                                    Account_ID = item.Id;
                                    break;
                                }
                            }
                        }
                        else Account_ID = tbName.Text.Trim();
                    }
                    else
                    {
                        if (cmdTYPE != Type_of_Command.AssignFile)
                        {
                            restCallURL = serviceURL + "/services/data/v32.0/query?q=SELECT Id FROM " + nameOfService + " LIMIT 1";

                            apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                            apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                            apiCallResponse = await apiCallClient.SendAsync(apirequest);

                            requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                            if (apiCallResponse.IsSuccessStatusCode)
                            {
                                dynamic list = JsonConvert.DeserializeObject(requestresponse);
                                foreach (var item in list)
                                {
                                    if (((String)item.Name).Contains("records"))
                                    {

                                        foreach (var item2 in item.Value)
                                        {
                                            if (cmdTYPE == Type_of_Command.AssignFile)
                                                Account_ID = item2.ContentDocumentId;
                                            else Account_ID = item2.Id;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                    firsttime = false;
                    Account_ID = id;
                }
            }
            else
            {
                firsttime = false;
                Account_ID = id;
            }*/

            dataTable = new System.Data.DataTable();
            dataTable.TableName = "TableName";
            dataTable.Columns.Add(new DataColumn("DisplayName", typeof(string)) { MaxLength = 1000 });
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)) { MaxLength = 200 });
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)) { MaxLength = 60000 });
            DataRow row;
            String Name;

            try
            {
                switch (cmdTYPE)
                {
                    case Type_of_Command.AddList:
                    case Type_of_Command.UpdateList:
                        /*listTMP.Category int
                            listTMP.CategorySpecified bool care vine cu int
                        listTMP.Client.ID int
                            listTMP.Client.IDSpecified bool
                        listTMP.CorrelationID int
                        listTMP.CreatedDate
                            listTMP.CreatedDateSpecified
                        listTMP.CustomerKey String
                        listTMP.Description  String
                        listTMP.ID  int
                            listTMP.IDSpecified bool
                        listTMP.ListName
                        listTMP.ObjectID  String
                        listTMP.ObjectState String
                        listTMP.PartnerKey String
                            listTMP.Type   Public  /  Private  /  SalesForce  /  GlobalUnsubscribe  /  Master*/


                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Category"; row["Value"] = "Number";dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Description"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ListName"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectState"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Type"; row["Value"] = "Public  /  Private  /  SalesForce  /  GlobalUnsubscribe  /  Master"; dataTable.Rows.Add(row);
                        break;
                    case Type_of_Command.AddSubscriber:
                    case Type_of_Command.UpdateSubscriber:
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "EmailAddress"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "EmailTypePreference"; row["Value"] = "TEXT / HTML"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        //add la lists
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectState"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerType"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "SubscriberKey"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "NrOfList"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ListID1"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ListID2"; row["Value"] = "Number"; dataTable.Rows.Add(row);

                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "NrOfAttributes"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name1"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Value1"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name2"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Value2"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        break;

                    case Type_of_Command.AddDataExtension:
                    case Type_of_Command.UpdateDataExtension:                        
                      
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Description"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectState"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Status"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "NrOfFields"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name1"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "DataType1"; row["Value"] = "Text / Number / Date / Boolean / Email Address / Phone / Decimal / Locale"; dataTable.Rows.Add(row);                        
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "MaxLength1"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "DefaultValue1"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name2"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "DataType2"; row["Value"] = "Text / Number / Date / Boolean / Email Address / Phone / Decimal / Locale"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "MaxLength2"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "DefaultValue2"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        break;

                    case Type_of_Command.AddDataExtensionObject:
                    case Type_of_Command.UpdateDataExtensionObject:
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        //add la lists
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectState"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Type"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "NrOfProperties"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name1"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Value1"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name2"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Value2"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        break;
                    default:
                        break;
                }                
                
            }
            catch (Exception ex)
            {
                row = dataTable.NewRow();
                row["DisplayName"] = "Error";
                row["Name"] = "Error";
                row["Value"] = ex.ToString();
                dataTable.Rows.Add(row);
                Error = ex.ToString();
            }

            btGetData.Content = "Get Data";
            ResultGridView.ItemsSource = dataTable.DefaultView;
            ResultGridView.IsEnabled = true;
            btGetData.IsEnabled = true;
            tbFilter.IsEnabled = true;
            if (cmdTYPE != Type_of_Command.CheckID) ckName.IsEnabled = true;
            if (ckName.IsChecked ?? false) tbName.IsEnabled = true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClickLogic();
        }

        private void tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = tbFilter.Text;
            if (string.IsNullOrEmpty(filter))
                dataTable.DefaultView.RowFilter = null;
            else
            { 
                if (cmdTYPE!=Type_of_Command.JustDisplay) dataTable.DefaultView.RowFilter = string.Format("Name Like '%{0}%' OR Value Like '%{0}%' OR DisplayName Like '%{0}%'", filter);
                else dataTable.DefaultView.RowFilter = string.Format("LabelName Like '%{0}%' OR ParameterName Like '%{0}%' OR Type Like '%{0}%'", filter);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (cmdTYPE != Type_of_Command.CheckID)
            {
                double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                double windowWidth = this.Width;
                double windowHeight = this.Height;
                if (cmdTYPE == Type_of_Command.JustDisplay) this.Left = 385 + (screenWidth / 2);
                else this.Left = 270 + (screenWidth / 2);
                this.Top = (screenHeight / 2) - (windowHeight / 2)-20;
            }
            Boolean decision = false;

            switch (cmdTYPE)
            {
                case Type_of_Command.AddAsset:
                    ckName.Content = "Asset ID";
                    break;
                case Type_of_Command.UpdateAsset:
                    ckName.Content = "Asset ID";
                    if (id.Trim().Length > 0)
                    {
                        firsttime = true;
                        tbName.Text = id;
                        ckName.IsChecked = true;
                        tbName.IsEnabled = true;
                    }
                    break;
                case Type_of_Command.AddDataExtension:
                    ckName.Content = "DataExtension ID";
                    break;
                case Type_of_Command.UpdateDataExtension:
                    ckName.Content = "DataExtension ID";
                    if (id.Trim().Length > 0)
                    {
                        firsttime = true;
                        tbName.Text = id;
                        ckName.IsChecked = true;
                        tbName.IsEnabled = true;
                    }
                    break;
                case Type_of_Command.AddDataExtensionObject:
                    ckName.Content = "DataExtensionObject ID";
                    break;
                case Type_of_Command.UpdateDataExtensionObject:
                    ckName.Content = "DataExtensionObject ID";
                    if (id.Trim().Length > 0)
                    {
                        firsttime = true;
                        tbName.Text = id;
                        ckName.IsChecked = true;
                        tbName.IsEnabled = true;
                    }
                    break;
                case Type_of_Command.AddSubscriber:
                    ckName.Content = "Subscriber ID";
                    break;
                case Type_of_Command.UpdateSubscriber:
                    ckName.Content = "Subscriber ID";
                    if (id.Trim().Length > 0)
                    {
                        firsttime = true;
                        tbName.Text = id;
                        ckName.IsChecked = true;
                        tbName.IsEnabled = true;
                    }
                    break;

                case Type_of_Command.AddList:
                    ckName.Content = "List ID";
                    break;
                case Type_of_Command.UpdateList:
                    ckName.Content = "List ID";
                    if (id.Trim().Length > 0)
                    {
                        firsttime = true;
                        tbName.Text = id;
                        ckName.IsChecked = true;
                        tbName.IsEnabled = true;
                    }
                    break;

                case Type_of_Command.AddCampaign:
                    ckName.Content = "Campaign ID";
                    break;
                case Type_of_Command.UpdateCampaign:
                    ckName.Content = "Campaign ID";
                    if (id.Trim().Length > 0)
                    {
                        firsttime = true;
                        tbName.Text = id;
                        ckName.IsChecked = true;
                        tbName.IsEnabled = true;
                    }
                    break;
                case Type_of_Command.CheckID:
                    ckName.Content = "ID";
                    firsttime = false;
                    tbName.Text = id;
                    ckName.IsChecked = true;
                    tbName.IsEnabled = true;
                    ckName.IsEnabled = false;
                    break;
                default:
                    break;
            }
            if (cmdTYPE != Type_of_Command.JustDisplay)
            {
                if (cmdTYPE != Type_of_Command.CheckID)
                    this.Title += " " + nameOfService;
                if (decision)
                {
                    btGetData.IsEnabled = false;
                    dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer.Tick += dispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();
                }
            }
            else
            {
                GridToHide.Visibility = Visibility.Collapsed;                
                RowToHide.MinHeight = 1;
                RowToHide.MaxHeight = 1;
                RowToHide.Height = new GridLength(1.0, GridUnitType.Star);
                dataTable = new System.Data.DataTable();
                dataTable.TableName = "TableName";
                dataTable.Columns.Add(new DataColumn("LabelName", typeof(string)) { MaxLength = 1000 });
                dataTable.Columns.Add(new DataColumn("ParameterName", typeof(string)) { MaxLength = 200 });
                dataTable.Columns.Add(new DataColumn("Type", typeof(string)) { MaxLength = 60000 });
                dataTable.Columns.Add(new DataColumn("Value", typeof(ObjectFromSalesforce)) { ColumnMapping= MappingType.Hidden });
                DataRow row;
                foreach (String item in HashGeneralData.Keys)
                {
                    row = dataTable.NewRow();
                    row["LabelName"] = ((ObjectFromSalesforce)HashGeneralData[item]).LabelName;
                    row["ParameterName"] = ((ObjectFromSalesforce)HashGeneralData[item]).ParameterName;
                    row["Type"] = ((ObjectFromSalesforce)HashGeneralData[item]).Type;
                    row["Value"] = (ObjectFromSalesforce)HashGeneralData[item];
                    dataTable.Rows.Add(row);
                }
                dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["LabelName"] };
                dataTable.DefaultView.ApplyDefaultSort = true;
                ResultGridView.ItemsSource = dataTable.DefaultView;
                ResultGridView.CanUserSortColumns = true;                
                ResultGridView.Columns[3].Visibility = Visibility.Collapsed;               
                ResultGridView.IsEnabled = true;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ButtonClickLogic();
            dispatcherTimer.Stop();
            dispatcherTimer.IsEnabled = false;
        }

        private void ckName_Checked(object sender, RoutedEventArgs e)
        {
            tbName.IsEnabled = true;
            tbName.Focus();
        }

        private void ckName_Unchecked(object sender, RoutedEventArgs e)
        {
            tbName.IsEnabled = false;
        }

        private void ResultGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            String Error = "";
            if (parentWindow != null)
            {
                try
                {
                    DataRowView row = (DataRowView)ResultGridView.SelectedItems[0];                    
                    parentWindow.AddParameter(row["Name"].ToString());
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                }
            }  
        }
    }
}
