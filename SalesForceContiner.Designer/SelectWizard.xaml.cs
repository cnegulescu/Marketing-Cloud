using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace UiPathTeam.Salesforce.Marketing_Cloud.Activities
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SelectWizard : Window
    {
        public String AuthToken;
        public String ServiceURL;
        public String SelectCMD;
        public Boolean SaveSearch = false;
        private String valueForAttributs;
        private String searchValue;
        private String sobject;
        private Boolean isWizard = true;
        private Boolean firstLoad = true;
        private DataTable dataTable;
        private DataTable dataTable2;

        public List<ComboBoxItem> ListEnum { get; set; }

        public List<ComboBoxItem> ListEnumCmd { get; set; }       

        public SelectWizard(String _AuthToken, String _ServiceURL, String SearchValue)
        {
            isWizard = true;
            AuthToken = _AuthToken;
            ServiceURL = _ServiceURL;
            searchValue = SearchValue;
            ListEnum = new List<ComboBoxItem>();

            ListEnum.Add(new ComboBoxItem() { ValueName = "DataExtension", ValueString = "DataExtension" });
            ListEnum.Add(new ComboBoxItem() { ValueName = "DataExtensionObject", ValueString = "DataExtensionObject" });
            ListEnum.Add(new ComboBoxItem() { ValueName = "List", ValueString = "List" });
            ListEnum.Add(new ComboBoxItem() { ValueName = "Subscriber", ValueString = "Subscriber" });

            ListEnumCmd = new List<ComboBoxItem>();
            if (searchValue.Length > 1)
            {
                ListEnumCmd.Add(new ComboBoxItem()
                {
                    ValueName = "Actual value that is save in Properties --> Input --> Search",
                    ValueString = searchValue
                });
            }

            ListEnumCmd.Add(new ComboBoxItem()
            {
                ValueName = "Select from list",
                ValueString = "SELECT ListName,Description,ID,PartnerKey,CreatedDate,ModifiedDate,Client.ID,Category,Type FROM List WHERE ID>1"
            });

            ListEnumCmd.Add(new ComboBoxItem()
            {
                ValueName = "Select from subscriber",
                ValueString = "SELECT ID,EmailAddress,CreatedDate,Client.ID,PartnerKey,SubscriberKey FROM Subscriber WHERE ID>1"
            });

            ListEnumCmd.Add(new ComboBoxItem()
            {
                ValueName = "Select from dataextention",
                ValueString = "SELECT Name,Description,PartnerKey,Client.ID,CreatedDate,CustomerKey,Status FROM Dataextension WHERE Name > 'a'"
            });

            ListEnumCmd.Add(new ComboBoxItem()
            {
                ValueName = "Select from DataExtentionObject",
                ValueString = "SELECT Name,Type,CloseDate,Amount,Owner.Name FROM Opportunity WHERE CloseDate>2018-01-01 AND IsWon=False"
            });

            DataContext = this;
            InitializeComponent();
            lbObjectValue.Visibility = Visibility.Hidden;
        }

        public SelectWizard(String _AuthToken, String _ServiceURL, String SearchValue, String Sobject)
        {
            isWizard = false;
            InitializeComponent();
            AuthToken = _AuthToken;
            ServiceURL = _ServiceURL;
            searchValue = SearchValue;
            sobject = Sobject;
            cbType.Visibility = Visibility.Hidden;            
            lbObjectValue.Content = Sobject;
            this.Title = "Get " + sobject + " Wizard";

            searchValue = searchValue.Replace(",", ",~");
            searchValue = searchValue.Replace("=", "~=~");
            searchValue = searchValue.Replace(">", "~>~");
            searchValue = searchValue.Replace("<", "~<~");
            GridFilters.Visibility = Visibility.Collapsed;
            RowToHide.MinHeight = 1;
            RowToHide.MaxHeight = 1;
            RowToHide.Height = new GridLength(1.0, GridUnitType.Star);
            if (Sobject.Trim().ToUpper().Equals("DATAEXTENSIONOBJECT"))
            {
                cbID.IsChecked = true;
                cbID.IsEnabled = false;
                ResultGridView.IsEnabled = false;
                tbDisplay.Text = "Please put above the Extrenal Key of DataExtension";
                tbDisplay.IsEnabled = false;
            }
        }

        private async void RunClickLogic()
        {
            String final = Convert_ColorText_To_CMD(StringFromRichTextBox(tbSelectCMD));
            tabControl1.SelectedIndex = 1;
            tbCommand.Text = "Waiting for the Salesforce server to respond...";
            tabControl1.IsEnabled = false;
            var task = await (new CmdRestAPI(AuthToken, ServiceURL, final)).ExecuteAsync();

            if (task.ValidConnection)
            {
                tabControl1.SelectedIndex = 1;
                tbCommand.Text = final;
                dataTable2 = task.DataTableResp;
                Int32 cnt = 0;
                foreach (DataColumn tmpC in dataTable2.Columns)
                {
                    if (tmpC.ColumnName.Contains("."))
                        dataTable2.Columns[cnt].ColumnName = tmpC.ColumnName.Replace(".","_");
                    cnt++;
                }
                ResultGridView2.ItemsSource = dataTable2.DefaultView;
                rowcount.Content = "Number of rows: " + dataTable2.Rows.Count.ToString();
            }
            else
            {
                CommandResult tmpwindow = new CommandResult(false, task.Response, task.ID, AuthToken, ServiceURL, "", Type_of_Command.SOQLcommand);
                tmpwindow.Show();
            }
            tabControl1.IsEnabled = true;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (isWizard)
            {
                cbCommand.SelectedIndex = 0;
                cbType.SelectedIndex = 0;
            }
            else
            {
                populateSelectCMD(searchValue);
            }
        }

        private void populateSelectCMD(String StrInput)
        {
            String Error = "";
            try
            {
                String str = StrInput;
                FlowDocument mcFlowDoc = new FlowDocument();
                // Create a paragraph with text  
                Paragraph para = new Paragraph();                

                //tbSelectCMD.Document = null;
                if (str.IndexOf('\'') > 0)
                {
                    String strtmp = "";
                    int cnt = 1;
                    foreach (String Item in str.Split('\''))
                    {
                        if ((cnt % 2) == 0)
                        {
                            strtmp += Item.Replace(" ", "~") + "'";
                        }
                        else
                        {
                            strtmp += Item + "'";
                        }
                        cnt++;
                    }
                    str = strtmp.Remove(strtmp.Length - 1, 1);
                }

                foreach (String Item in str.Split(' '))
                {
                    if (Item.ToUpper().Equals("SELECT") || Item.ToUpper().Equals("FROM") || Item.ToUpper().Equals("WHERE") || Item.ToUpper().Equals("LIMIT") || Item.ToUpper().Equals("AND"))
                    {
                        para = new Paragraph();
                        para.Inlines.Add(new Bold(new Run(Item)));
                        para.Foreground = System.Windows.Media.Brushes.Blue;
                        mcFlowDoc.Blocks.Add(para);
                    }
                    else
                    {
                        para = new Paragraph();
                        para.Inlines.Add(new Run("  " + Item.Replace("~", " ")));
                        para.Foreground = System.Windows.Media.Brushes.Green;
                        mcFlowDoc.Blocks.Add(para);
                    }
                }
                mcFlowDoc.FontSize = 16;
                mcFlowDoc.LineHeight = 1;
                tbSelectCMD.Document = mcFlowDoc;
                //tbSelectCMD.UpdateLayout();
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }

        private void cbCommand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String Error;
            try
            {
                String str = (cbCommand.SelectedItem as ComboBoxItem).ValueString;
                populateSelectCMD(str);
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }

        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSearch = true;            
            SelectCMD = Convert_ColorText_To_CMD(StringFromRichTextBox(tbSelectCMD));
            Close();            
        }

        private string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }


        private String Convert_ColorText_To_CMD(String str)
        {
            String final = "";
            Boolean cmdBefore = false;
            foreach (String Item in str.Split('\n'))
            {
                if (Item.Trim().ToUpper().Equals("SELECT") || Item.Trim().ToUpper().Equals("FROM") || Item.Trim().ToUpper().Equals("WHERE") || Item.Trim().ToUpper().Equals("LIMIT") || Item.Trim().ToUpper().Equals("AND"))
                {
                    if (cmdBefore) final += " " + Item.Trim() + " ";
                    else final += Item.Trim() + " ";
                    cmdBefore = true;
                }
                else final += Item.Trim();
            }
            final = final.Replace(" , ", ",");
            final = final.Replace(", ", ",");
            final = final.Replace(" = ", "=");
            final = final.Replace("= ", "=");
            final = final.Replace(" > ", ">");
            final = final.Replace("> ", ">");
            final = final.Replace(" >", ">");
            final = final.Replace(" < ", "<");
            final = final.Replace("< ", "<");
            final = final.Replace(" <", "<");
            final = final.Replace(" >= ", ">=");
            final = final.Replace(">= ", ">=");
            final = final.Replace(" >=", ">=");
            final = final.Replace(" <= ", "<=");
            final = final.Replace("<= ", "<=");
            final = final.Replace(" <=", "<=");

            if (sobject.Trim().ToUpper().Equals("DATAEXTENSIONOBJECT"))
            {
                final = final.Replace("DataExtensionObject", "DataExtensionObject["+tbID.Text+"]");
            }
            return final;
        }

        private void btRunCmd_Click(object sender, RoutedEventArgs e)
        {
            RunClickLogic();
        }
        private async void GetDataClickLogic()
        {
            List<string> tmpList = new List<string>();
            String Account_ID = "";
            String restCallURL = "";
            HttpRequestMessage apirequest;
            HttpResponseMessage apiCallResponse;
            String requestresponse;
            String Error = "";

            if (!isWizard)
                valueForAttributs = (String)lbObjectValue.Content;
            else
            {
                try
                {
                    valueForAttributs = (cbType.SelectedItem as ComboBoxItem).ValueString;
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                }
            }

            btGetData.IsEnabled = false;
            tabControl1.IsEnabled = false;

            dataTable = new System.Data.DataTable();
            dataTable.TableName = "TableName";
            dataTable.Columns.Add(new DataColumn("DisplayName", typeof(string)) { MaxLength = 2000 });
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)) { MaxLength = 200 });
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)) { MaxLength = 60000 });

            DataRow row;
            try
            {                
                switch (valueForAttributs.ToUpper())
                {
                    case "LIST":
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Category"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                       // row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Description"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ListName"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                       // row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectState"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Type"; row["Value"] = "Public  /  Private  /  SalesForce  /  GlobalUnsubscribe  /  Master"; dataTable.Rows.Add(row);
                        break;
                    case "SUBSCRIBER":
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        //row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        //row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "EmailAddress"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "EmailTypePreference"; row["Value"] = "TEXT / HTML"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        //add la lists
                        //row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "ObjectID"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "SubscriberKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        break;
                    case "DATAEXTENSIONOBJECT":                        
                        /*row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "SubscriberKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CorrelationID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        //add la lists                        
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Type"; row["Value"] = "String"; dataTable.Rows.Add(row);*/
                        break;
                    case "DATAEXTENSION":
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Name"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Description"; row["Value"] = "String"; dataTable.Rows.Add(row);                        
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "PartnerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);

                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Client.ID"; row["Value"] = "Number"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CreatedDate"; row["Value"] = "Date"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "CustomerKey"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        row = dataTable.NewRow(); row["DisplayName"] = row["Name"] = "Status"; row["Value"] = "String"; dataTable.Rows.Add(row);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                row = dataTable.NewRow();
                row["DisplayName"] = "ERROR";
                row["Name"] = "ERROR";
                row["Value"] = ex.ToString();
                dataTable.Rows.Add(row);
            }

            ResultGridView.ItemsSource = dataTable.DefaultView;
            btGetData.IsEnabled = true;
            tabControl1.IsEnabled = true;
        }

        private void btGetData_Click(object sender, RoutedEventArgs e)
        {
            GetDataClickLogic();
        }

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String Error;
            try
            {
                valueForAttributs = (cbType.SelectedItem as ComboBoxItem).ValueString;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }

        private void cbID_Checked(object sender, RoutedEventArgs e)
        {
            tbID.IsEnabled = true;
        }

        private void cbID_Unchecked(object sender, RoutedEventArgs e)
        {
            tbID.IsEnabled = false;
        }

        private void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((!isWizard) && (tabControl1.SelectedIndex == 2) && (firstLoad))
            {
                firstLoad = false;
                GetDataClickLogic();
            }
        }

        private void tbDisplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            String Error = "";
            try
            {
                string filter = tbDisplay.Text;
                if (string.IsNullOrEmpty(filter))
                    dataTable.DefaultView.RowFilter = null;
                else
                    dataTable.DefaultView.RowFilter = string.Format("Name Like '%{0}%' OR Value Like '%{0}%' OR DisplayName Like '%{0}%'", filter);
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }

        private void ResultGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            String Error = "";
            try
            {
                String addpart = "";
                FlowDocument mcFlowDoc = new FlowDocument();
                mcFlowDoc = tbSelectCMD.Document;
                Paragraph par1 = (Paragraph)mcFlowDoc.Blocks.FirstBlock;
                mcFlowDoc.Blocks.Remove(mcFlowDoc.Blocks.FirstBlock);
                Paragraph par2 = (Paragraph)mcFlowDoc.Blocks.FirstBlock;
                mcFlowDoc.Blocks.Remove(mcFlowDoc.Blocks.FirstBlock);
                Int32 cnt = 0;
                foreach (Paragraph tmp in mcFlowDoc.Blocks)
                {
                    if (cnt == 1)
                    {
                        par1 = tmp;
                        break;
                    }
                    cnt++;
                }

                String tablename = new TextRange(par1.ContentStart, par1.ContentEnd).Text.Trim();

                if (tablename.ToUpper().Equals(valueForAttributs.Trim().ToUpper())) addpart = "";
                else addpart =  valueForAttributs.Trim()+ ".";
                DataRowView row = (DataRowView)ResultGridView.SelectedItems[0];                
                addpart = addpart + row["Name"].ToString();

                String str = new TextRange(par2.ContentStart, par2.ContentEnd).Text.Trim();
                if (str.Length > 1)
                {
                    str = str + ", "+addpart;
                }
                else str = addpart;

                par1 = new Paragraph();
                par1.Inlines.Add(new Run("  " + str));
                par1.Foreground = System.Windows.Media.Brushes.Green;
                mcFlowDoc.Blocks.InsertBefore(mcFlowDoc.Blocks.FirstBlock, par1);

                par2 = new Paragraph();
                par2.Inlines.Add(new Bold(new Run("SELECT")));
                par2.Foreground = System.Windows.Media.Brushes.Blue;
                mcFlowDoc.Blocks.InsertBefore(mcFlowDoc.Blocks.FirstBlock, par2);
                tbSelectCMD.Document = mcFlowDoc;

                tabControl1.SelectedIndex = 0;
                tabControl1.UpdateLayout();
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
            
        }
    }
}
