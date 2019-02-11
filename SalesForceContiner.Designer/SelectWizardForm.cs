using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SalesforceAPI.Designer
{
    public partial class SelectWizardForm : Form
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
        public SelectWizardForm(String _AuthToken, String _ServiceURL,String SearchValue)
        {
            isWizard = true;
            InitializeComponent();
            AuthToken = _AuthToken;
            ServiceURL = _ServiceURL;
            searchValue = SearchValue;
            lbObjectValue.Visible = false;
            if (Salesforce_Application_Scope.Desing_LISTOBJECT.Count>1)
              HandleTextChanged(Salesforce_Application_Scope.Desing_LISTOBJECT);
        }

        public SelectWizardForm(String _AuthToken, String _ServiceURL, String SearchValue,String Sobject)
        {
            isWizard = false;
            InitializeComponent();
            AuthToken = _AuthToken;
            ServiceURL = _ServiceURL;
            searchValue = SearchValue;
            sobject = Sobject;
            lbExample.Visible = false;
            cbCommand.Visible = false;
            CbObjectType.Visible = false;
            lbObjectValue.Visible = true;
            lbObjectValue.Text = Sobject;           
            //tbSelectCMD.Location = new Point(15, 60);
            tbSelectCMD.Text = "";
            this.Text = "Get " + sobject + " Wizard";

            searchValue = searchValue.Replace(",", ",~");
            searchValue = searchValue.Replace("=", "~=~");
            searchValue = searchValue.Replace(">", "~>~");
            searchValue = searchValue.Replace("<", "~<~");
            splitContainer8.Panel2Collapsed = true;
            splitContainer1.SplitterDistance = 45;
        }

        private void cbID_CheckedChanged(object sender, EventArgs e)
        {
            if (cbID.Checked) tbID.Enabled = true;
            else tbID.Enabled = false;
        }

        private async void btGetData_Click(object sender, EventArgs e)
        {
            List<string> tmpList = new List<string>();
            String Account_ID = "";
            String restCallURL = "";
            HttpRequestMessage apirequest;
            HttpResponseMessage apiCallResponse;
            String requestresponse;
            String Error = "";

            if (!isWizard)
                valueForAttributs = lbObjectValue.Text;
            else
            {
                try
                {
                    valueForAttributs = CbObjectType.Text;
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                }
            }

            btGetData.Enabled = false;
            tabControl1.Enabled = false;

            var dataTable = new System.Data.DataTable();
            dataTable.TableName = "TableName";
            dataTable.Columns.Add(new DataColumn("DisplayName", typeof(string)) { MaxLength = 2000 });
            dataTable.Columns.Add(new DataColumn("Name", typeof(string)) { MaxLength = 200 });
            dataTable.Columns.Add(new DataColumn("Value", typeof(string)) { MaxLength = 60000 });

            DataRow row;
            try
            {
                tbDisplay.Clear();
                if (Salesforce_Application_Scope.Desing_HASHOBJECT[valueForAttributs] == null)
                {
                    var task = await (new CmdRestAPI(AuthToken, ServiceURL, Type_of_Command.GetCustomMandatory, valueForAttributs)).ExecuteAsync();
                }
                HttpClient apiCallClient = new HttpClient();

                if (cbID.Checked && tbID.Text.Trim().Length > 0)
                {
                    Account_ID = tbID.Text.Trim();
                }
                else
                {
                    restCallURL = ServiceURL + "/services/data/v32.0/query?q=SELECT Id FROM " + valueForAttributs + " LIMIT 1";

                    apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                    apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    apirequest.Headers.Add("Authorization", "Bearer " + AuthToken);
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
                                    Account_ID = item2.Id;
                                    break;
                                }
                            }
                        }
                    }
                }


                restCallURL = ServiceURL + "/services/data/v20.0/sobjects/" + valueForAttributs + "/" + Account_ID;

                apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                apirequest.Headers.Add("Authorization", "Bearer " + AuthToken);
                apiCallResponse = await apiCallClient.SendAsync(apirequest);

                requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                if (apiCallResponse.IsSuccessStatusCode)
                {
                    dynamic list = JsonConvert.DeserializeObject(requestresponse);
                    foreach (var item in list)
                    {
                        if (!item.Name.ToString().Contains("attributes"))
                        {
                            String Name = item.Name;
                            row = dataTable.NewRow();
                            try
                            {
                                row["DisplayName"] = ((Hashtable)Salesforce_Application_Scope.Desing_HASHOBJECT[valueForAttributs])[Name];
                            }
                            catch (Exception ex)
                            {
                                Error = ex.ToString();
                                row["DisplayName"] = "";
                            }
                            row["Name"] = item.Name;
                            row["Value"] = item.Value;
                            if (row["Value"].ToString().Trim().Length > 0) dataTable.Rows.Add(row);
                            else tmpList.Add(item.Name);
                        }
                    }
                    if (tmpList.Count > 0)
                    {
                        foreach (String str in tmpList)
                        {
                            row = dataTable.NewRow();
                            try
                            {
                                row["DisplayName"] = ((Hashtable)Salesforce_Application_Scope.Desing_HASHOBJECT[valueForAttributs])[str];
                            }
                            catch (Exception ex)
                            {
                                Error = ex.ToString();
                                row["DisplayName"] = "";
                            }
                            row["Name"] = str;
                            row["Value"] = "";
                            dataTable.Rows.Add(row);
                        }
                    }
                }
                else
                {
                    row = dataTable.NewRow();
                    row["DisplayName"] = restCallURL;
                    row["Name"] = "ERROR";
                    row["Value"] = requestresponse;
                    dataTable.Rows.Add(row);
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

            dataSet2 = new DataSet();
            dataSet2.EnforceConstraints = false;
            dataSet2.Tables.Add(dataTable);
            dataGridView2.DataSource = dataSet2.Tables[0];

            btGetData.Enabled = true;
            tabControl1.Enabled = true;
        }

        private void populateSelectCMD(String StrInput)
        {
            String Error = "";
            try
            {
                String str = StrInput;
                tbSelectCMD.Clear();
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
                        tbSelectCMD.SelectionColor = Color.Green;
                        tbSelectCMD.SelectedText = Item + Environment.NewLine;
                    }
                    else
                    {
                        tbSelectCMD.SelectionColor = Color.Blue;
                        tbSelectCMD.SelectedText = "  " + Item.Replace("~", " ") + Environment.NewLine;
                    }
                }

            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String str = (cbCommand.SelectedItem as ComboboxItem).Value;
            populateSelectCMD(str);
        }

        private String Convert_ColorText_To_CMD(String str)
        {
            String final = "";
            Boolean cmdBefore = false;
            foreach (String Item in str.Split('\n'))
            {
                if (Item.Trim().ToUpper().Equals("SELECT") || Item.Trim().ToUpper().Equals("FROM") || Item.Trim().ToUpper().Equals("WHERE") || Item.Trim().ToUpper().Equals("LIMIT") || Item.ToUpper().Equals("AND"))
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
            return final;
        }

        private async void btRunServer_Click(object sender, EventArgs e)
        {
            String final = Convert_ColorText_To_CMD(tbSelectCMD.Text);
            tabControl1.SelectedIndex = 1;
            tbCommand.Text = "Waiting for the Salesforce server to respond...";
            tabControl1.Enabled = false;
            var task = await (new CmdRestAPI(AuthToken, ServiceURL, final, false)).ExecuteAsync();

            if (task.ValidConnection)
            {
                tabControl1.SelectedIndex = 1;
                tbCommand.Text = final;
                dataSet1 = new DataSet();
                dataSet1.EnforceConstraints = false;
                dataSet1.Tables.Add(task.DataTableResp);
                dataGridView1.DataSource = dataSet1.Tables[0];
            }
            else
            {
                CommandResult tmpwindow = new CommandResult(false, task.Response, task.ID, AuthToken, ServiceURL, "", Type_of_Command.SOQLcommand);
                tmpwindow.Show();
            }
            tabControl1.Enabled = true;
        }

        private void SelectWizardForm_Load(object sender, EventArgs e)
        {
            if (isWizard)
            {
                cbCommand.Items.Clear();

                if (searchValue.Length > 1)
                {
                    ComboboxItem item0 = new ComboboxItem();
                    item0.Text = "Actual value that is save in Properties --> Input --> Search";
                    item0.Value = searchValue;
                    cbCommand.Items.Add(item0);
                }

                ComboboxItem item = new ComboboxItem();
                item.Text = "Select the first 100 Accounts from a country";
                item.Value = "SELECT Name,Owner.Name,Type,Vertical__c,LastActivityDate,LastModifiedDate FROM Account WHERE BillingCountry='Canada' LIMIT 100";

                cbCommand.Items.Add(item);

                ComboboxItem item2 = new ComboboxItem();
                item2.Text = "Select the first 100 Leads from a country";
                item2.Value = "SELECT Country,FirstName,LastName,Title,Owner.Name,Email,Phone,LeadSource,Lead_Domain__c,CreatedDate FROM Lead WHERE Country='Japan' LIMIT 100";

                cbCommand.Items.Add(item2);

                ComboboxItem item3 = new ComboboxItem();
                item3.Text = "Select Opportunity lost in 2018";
                item3.Value = "SELECT Name,Type,CloseDate,Amount,Owner.Name FROM Opportunity WHERE CloseDate>2018-01-01 AND StageName='Closed~Lost'";

                cbCommand.Items.Add(item3);

                ComboboxItem item4 = new ComboboxItem();
                item4.Text = "Select Opportunity won in Q1 2018";
                item4.Value = "SELECT Name,Type,CloseDate,Amount,CurrencyIsoCode,Owner.Name FROM Opportunity WHERE CloseDate>=2018-01-01 AND CloseDate<2018-04-01 AND IsWon=True";

                cbCommand.Items.Add(item4);

                cbCommand.SelectedIndex = 0;                
            }
            else
            {
                populateSelectCMD(searchValue);
            }
        }

        private void CbObjectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            String Error = "";
            try
            {
                _needUpdate = false;
                valueForAttributs = CbObjectType.Text;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }

        }

        private void btSave_Click(object sender, EventArgs e)
        {
            SaveSearch = true;
            SelectCMD = Convert_ColorText_To_CMD(tbSelectCMD.Text);            
            Close();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((Salesforce_Application_Scope.Design_VALIDCONN) &&(!isWizard)&&(tabControl1.SelectedIndex==2)&&(firstLoad))
            {
                firstLoad = false;
                btGetData.PerformClick();
            }
        }
      
        private void tbDisplay_TextChanged(object sender, EventArgs e)
        {
            String Error = "";
            try
            {
                if (dataSet2.Tables[0] != null)
                    dataSet2.Tables[0].DefaultView.RowFilter = string.Format("Name Like '%{0}%' OR Value Like '%{0}%' OR DisplayName Like '%{0}%'", tbDisplay.Text);
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String Error = "";
            try
            {
                String str = tbSelectCMD.Text;
                String addpart = "";
                Int32 nr = str.IndexOf("FROM");
                //if is an other table logic
                Int32 Endfrom = str.IndexOf("WHERE");
                if (Endfrom < 1)
                {
                    Endfrom = str.IndexOf("LIMIT");
                    if (Endfrom < 1)
                    {
                        Endfrom = str.Length-1;
                    }
                }
                String tablename = str.Substring(nr + 4, Endfrom - nr - 4);
                tablename = tablename.Trim(Environment.NewLine.ToCharArray()).Trim('\n').Trim();
                if (tablename.ToUpper().Equals(CbObjectType.Text.Trim().ToUpper())) addpart = "";
                else addpart = CbObjectType.Text+".";
                //end of logic                
                tbSelectCMD.SelectionStart = nr-1;
                tbSelectCMD.SelectionLength = 1;
                tbSelectCMD.SelectedText = ", "+addpart+dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString() + Environment.NewLine;
                tabControl1.SelectedTab = tabPage1;
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }
        }

        private bool _canUpdate = true;

        private bool _needUpdate = false;

        //If text has been changed then start timer
        //If the user doesn't change text while the timer runs then start search
        private void CbObjectType_TextChanged(object sender, EventArgs e)
        {
            if (_needUpdate)
            {
                if (_canUpdate)
                {
                    _canUpdate = false;
                    UpdateData();
                }
                else
                {
                    RestartTimer();
                }
            }
        }

        private void UpdateData()
        {
            if (CbObjectType.Text.Length > 0)
            {
                //List<string> searchData = Salesforce_Application_Scope.Desing_LISTOBJECT.
                List<string> searchData = new List<string>();
                foreach (String item in Salesforce_Application_Scope.Desing_LISTOBJECT)
                    if (item.ToUpper().StartsWith(CbObjectType.Text.ToUpper()))
                        searchData.Add(item);
                HandleTextChanged(searchData);
            }
            else
            {
                HandleTextChanged(Salesforce_Application_Scope.Desing_LISTOBJECT);
            }
        }


        //Update data only when the user (not program) change something
        private void CbObjectType_TextUpdate(object sender, EventArgs e)
        {
            _needUpdate = true;
        }

        private void RestartTimer()
        {
            timer1.Stop();
            _canUpdate = false;
            timer1.Start();
        }

        //Update data when timer stops
        private void timer1_Tick(object sender, EventArgs e)
        {
            _canUpdate = true;
            timer1.Stop();
            UpdateData();
        }

        //Update combobox with new data
        private void HandleTextChanged(List<string> dataSource)
        {
            var text = CbObjectType.Text;

            if (dataSource.Count > 0)
            {
                CbObjectType.DataSource = dataSource;

                var sText = CbObjectType.Items[0].ToString();
                CbObjectType.SelectionStart = text.Length;
                CbObjectType.SelectionLength = sText.Length - text.Length;
                CbObjectType.DroppedDown = true;
                return;
            }
            else
            {
                CbObjectType.DroppedDown = false;
                CbObjectType.SelectionStart = text.Length;
            }
        }
    }
}
