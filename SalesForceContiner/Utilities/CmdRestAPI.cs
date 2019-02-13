using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    public class CmdRestAPI
    {
        private String authToken;
        private String serviceURL;
        private DataTable parameters;
        private String objectName;
        private String idDecision;
        private MCser.SoapClient soapClient;
        private Type_of_Search typeOfSearch;
        private String nameDecision;
        private Type_of_Command cmd;
        private Dictionary<String, String> dataIN;
        private String search;
        public String Response { get; set; }
        public String ID { get; set; }
        public Boolean ValidConnection { get; set; }
        public String RespAuthToken { get; set; }
        public String RespServiceURL { get; set; }
        public MCser.SoapClient RespSoapClient { get; set; }
        public DataTable DataTableResp { get; set; }
        public CmdRestAPI(String AuthToken, String ServiceURL, MCser.SoapClient SoapClient, DataTable Parameters, String ObjectName, String IdDecision, String NameDecision, Type_of_Command Cmd)
        {
            authToken = AuthToken;
            serviceURL = ServiceURL;
            parameters = Parameters;
            objectName = ObjectName;
            idDecision = IdDecision;
            soapClient = SoapClient;
            nameDecision = NameDecision;
            cmd = Cmd;
        }

        public CmdRestAPI(Dictionary<String, String> DataIN)
        {
            dataIN = DataIN;
            cmd = Type_of_Command.Connection;
        }

        public CmdRestAPI(String AuthToken, String ServiceURL, String Search)
        {
            authToken = AuthToken;
            serviceURL = ServiceURL;
            search = " " + Search;
            soapClient = Salesforce_Marketing_Cloud_Scope.Design_SOAP;
            cmd = Type_of_Command.SOQLcommand;
        }

        public CmdRestAPI(String AuthToken, String ServiceURL, Type_of_Command Cmd,String ServiceName)
        {
            authToken = AuthToken;
            serviceURL = ServiceURL;
            soapClient = Salesforce_Marketing_Cloud_Scope.Design_SOAP;
            cmd = Cmd;
            objectName = ServiceName;
        }

        public CmdRestAPI(String AuthToken, String ServiceURL)
        {
            authToken = AuthToken;
            serviceURL = ServiceURL;
            soapClient = Salesforce_Marketing_Cloud_Scope.Design_SOAP;
            cmd = Type_of_Command.ObjectList;
        }

        public CmdRestAPI(String AuthToken, String ServiceURL,String Search,String ServiceName, Type_of_Search TypeOfSearch)
        {
            authToken = AuthToken;
            serviceURL = ServiceURL;
            search = " " + Search;
            objectName = ServiceName;
            soapClient = Salesforce_Marketing_Cloud_Scope.Design_SOAP;
            cmd = Type_of_Command.GenericSearch;
            typeOfSearch = TypeOfSearch;
        }


        public async Task<CmdRestAPI> ExecuteAsync()
        {
            HttpClient apiCallClient = new HttpClient();
            String restCallURL = "";
            String Error = "";
            HttpRequestMessage apirequest;
            HttpResponseMessage apiCallResponse;
            String requestresponse;
            String insertPacket="";
            String strtmp = "";
            StringContent insertString;
            Response = "";
            
            switch (cmd)
            {
                case Type_of_Command.AddList:
                case Type_of_Command.AddDataExtension:
                case Type_of_Command.AddSubscriber:
                case Type_of_Command.AddCampaign:
                case Type_of_Command.AddDataExtensionObject:
                    try
                    {
                        string cRequestID = String.Empty;
                        string cStatus = String.Empty;
                        String IDName = "";
                        MCser.CreateResult[] cResults = null;
                        string strGUID = System.Guid.NewGuid().ToString();
                        switch (cmd)
                        {
                            case Type_of_Command.AddSubscriber:
                                MCser.Subscriber sub = new MCser.Subscriber();
                                sub.SubscriberKey = strGUID;
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "EMAILADDRESS":
                                            sub.EmailAddress = rowitem[1].ToString().Trim();
                                            sub.EmailTypePreference = MCser.EmailType.Text;
                                            sub.EmailTypePreferenceSpecified = true;
                                            break;
                                        case "NROFLISTS":
                                            sub.Lists = new MCser.SubscriberList[Convert.ToInt32(rowitem[1].ToString().Trim())];//If a list is not specified the Subscriber will be added to the "All Subscribers" List                                            
                                            break;
                                        case "NROFATTRIBUTES":
                                            sub.Attributes = new MCser.Attribute[Convert.ToInt32(rowitem[1].ToString().Trim())];
                                            break;
                                        default:
                                            if (rowitem[0].ToString().Trim().ToUpper().StartsWith("NAME"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("NAME", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                sub.Attributes[idatr-1] = new MCser.Attribute();
                                                sub.Attributes[idatr-1].Name = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("VALUE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("VALUE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                sub.Attributes[idatr-1].Value = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("LISTID"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("LISTID", "").Trim();
                                                Int32 idlist = Convert.ToInt32(nr);
                                                sub.Lists[idlist-1] = new MCser.SubscriberList();
                                                sub.Lists[idlist-1].ID = Convert.ToInt32(rowitem[1].ToString().Trim());//Available in the UI via List Properties
                                                sub.Lists[idlist-1].IDSpecified = true;
                                            }
                                            else Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }
                                //Create the CreateOptions object for the Create method
                                MCser.CreateOptions co = new MCser.CreateOptions();
                                co.SaveOptions = new MCser.SaveOption[1];
                                co.SaveOptions[0] = new MCser.SaveOption();
                                co.SaveOptions[0].SaveAction = MCser.SaveAction.UpdateAdd;//This set this call to act as an UpSert, meaning if the Subscriber doesn't exist it will Create if it does it will Update
                                co.SaveOptions[0].PropertyName = "*";

                                cResults = soapClient.Create(new MCser.CreateOptions(), new MCser.APIObject[] { sub }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.AddList:                                
                                MCser.List listTMP = new MCser.List();
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "LISTNAME":
                                            listTMP.ListName = rowitem[1].ToString().Trim();
                                            break;
                                        case "DESCRIPTION":
                                            listTMP.Description = rowitem[1].ToString().Trim();
                                            break;
                                        default:
                                            Response += rowitem[0].ToString().Trim()+" --> Invalid Parameter"+ Environment.NewLine;
                                            break;
                                    }
                                }
                                cResults = soapClient.Create(new MCser.CreateOptions(), new MCser.APIObject[] { listTMP }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.AddDataExtension:
                                MCser.DataExtension dataExt = new MCser.DataExtension();                      
                                dataExt.PartnerKey = strGUID;
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "NAME":
                                            dataExt.Name = rowitem[1].ToString().Trim();
                                            IDName = rowitem[1].ToString().Trim();
                                            break;
                                        case "DESCRIPTION":
                                            dataExt.Description = rowitem[1].ToString().Trim();
                                            break;
                                        case "NROFFIELDS":
                                            dataExt.Fields = new MCser.DataExtensionField[Convert.ToInt32(rowitem[1].ToString().Trim())];//If a list is not specified the Subscriber will be added to the "All Subscribers" List                                            
                                            break;
                                        default:
                                            if (rowitem[0].ToString().Trim().ToUpper().StartsWith("NAME"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("NAME", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1] = new MCser.DataExtensionField();
                                                dataExt.Fields[idatr-1].Name = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("DATATYPE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("DATATYPE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1].DataType = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("MAXLENGTH"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("MAXLENGTH", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1].MaxLength = Convert.ToInt32(rowitem[1].ToString().Trim());
                                                dataExt.Fields[idatr-1].MaxLengthSpecified = true;
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("DEFAULTVALUE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("DEFAULTVALUE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1].DefaultValue = rowitem[1].ToString().Trim();
                                            }
                                            else Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }
                                cResults = soapClient.Create(new MCser.CreateOptions(), new MCser.APIObject[] { dataExt }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.AddDataExtensionObject:
                                MCser.DataExtensionObject dataTMP = new MCser.DataExtensionObject();
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "CUSTOMERKEY":
                                            dataTMP.CustomerKey = rowitem[1].ToString().Trim();
                                            break;                                        
                                        case "NROFPROPERTIES":
                                            dataTMP.Properties = new MCser.APIProperty[Convert.ToInt32(rowitem[1].ToString().Trim())];//If a list is not specified the Subscriber will be added to the "All Subscribers" List                                            
                                            break;
                                        default:
                                            if (rowitem[0].ToString().Trim().ToUpper().StartsWith("NAME"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("NAME", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataTMP.Properties[idatr-1] = new MCser.APIProperty();
                                                dataTMP.Properties[idatr-1].Name = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("VALUE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("VALUE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataTMP.Properties[idatr-1].Value = rowitem[1].ToString().Trim();
                                            }                                            
                                            else Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }
                                cResults = soapClient.Create(new MCser.CreateOptions(), new MCser.APIObject[] { dataTMP }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.AddCampaign:
                                insertPacket = "{";
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    strtmp = "" + rowitem[1].ToString();
                                    if (strtmp.Trim().ToUpper().Equals("TRUE")) strtmp = "true";
                                    else if (strtmp.Trim().ToUpper().Equals("FALSE")) strtmp = "false";
                                    insertPacket += "\"" + rowitem[0].ToString() + "\":\"" + strtmp + "\",";
                                }                  
                                
                                insertPacket = insertPacket.Remove(insertPacket.Length - 1);
                                insertPacket += "}";
                                insertString = new StringContent(insertPacket, Encoding.UTF8, "application/json");

                                    restCallURL = serviceURL + "/hub/v1/campaigns";
                                    apirequest = new HttpRequestMessage(HttpMethod.Post, restCallURL);
                                    apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                                    apirequest.Content = insertString;
                                    apiCallResponse = await apiCallClient.SendAsync(apirequest);
                                    requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                                    Response = requestresponse;
                                    ValidConnection = true;
                                    if (apiCallResponse.IsSuccessStatusCode)
                                    {
                                         dynamic item = JsonConvert.DeserializeObject(requestresponse);
                                         ID = item.id;                                        
                                    }
                                    else
                                    {
                                        ValidConnection = false;
                                        ID = insertPacket;
                                    }                                
                                break;
                        }
                        if (cmd != Type_of_Command.AddCampaign)
                        {
                            Response += "Overall Status: " + cStatus + Environment.NewLine + "Number of Results: " + cResults.Length + Environment.NewLine;
                            foreach (MCser.CreateResult cr in cResults)
                            {
                                Response += "Status Message: " + cr.StatusMessage + Environment.NewLine;
                                ID = cr.NewID.ToString();
                                Int32 IDnr = 0;
                                if (Int32.TryParse(ID, out IDnr))
                                {
                                    if (IDnr == 0)
                                    {
                                        if (cmd == Type_of_Command.AddDataExtension)
                                        {
                                            ID = IDName;
                                        }
                                        else ID = strGUID;
                                    }
                                    else
                                    {
                                        if (cmd == Type_of_Command.AddSubscriber)
                                        {
                                            ID = strGUID;
                                        }
                                    }
                                }
                            }
                            if (cStatus.Equals("OK")) ValidConnection = true;
                            else ValidConnection = false;
                        }
                    }
                    catch (Exception exc)
                    {
                        Response = exc.ToString();
                        ValidConnection = false;
                    }
                    break;                    

                case Type_of_Command.UpdateList:
                case Type_of_Command.UpdateSubscriber:
                case Type_of_Command.UpdateCampaign:
                case Type_of_Command.UpdateDataExtension:
                case Type_of_Command.UpdateDataExtensionObject:
                    try
                    {
                        string cRequestID = String.Empty;
                        string cStatus = String.Empty;
                        MCser.UpdateResult[] cResults = null;
                        switch (cmd)
                        {
                            case Type_of_Command.UpdateSubscriber:
                                string strGUID = System.Guid.NewGuid().ToString();
                                MCser.Subscriber sub = new MCser.Subscriber();
                                sub.SubscriberKey = strGUID;
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "SUBSCRIBERKEY":
                                            sub.SubscriberKey = rowitem[1].ToString().Trim();
                                            sub.EmailTypePreference = MCser.EmailType.Text;
                                            sub.EmailTypePreferenceSpecified = true;
                                            break;
                                        case "NROFLISTS":
                                            sub.Lists = new MCser.SubscriberList[Convert.ToInt32(rowitem[1].ToString().Trim())];//If a list is not specified the Subscriber will be added to the "All Subscribers" List                                            
                                            break;
                                        case "NROFATTRIBUTES":
                                            sub.Attributes = new MCser.Attribute[Convert.ToInt32(rowitem[1].ToString().Trim())];
                                            break;
                                        default:
                                            if (rowitem[0].ToString().Trim().ToUpper().StartsWith("NAME"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("NAME", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                sub.Attributes[idatr-1] = new MCser.Attribute();
                                                sub.Attributes[idatr-1].Name = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("VALUE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("VALUE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                sub.Attributes[idatr-1].Value = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("LISTID"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("LISTID", "").Trim();
                                                Int32 idlist = Convert.ToInt32(nr);
                                                sub.Lists[idlist-1] = new MCser.SubscriberList();
                                                sub.Lists[idlist-1].ID = Convert.ToInt32(rowitem[1].ToString().Trim());//Available in the UI via List Properties
                                                sub.Lists[idlist-1].IDSpecified = true;
                                            }
                                            else Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }

                                cResults = soapClient.Update(new MCser.UpdateOptions(), new MCser.APIObject[] { sub }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.UpdateList:
                                MCser.List listTMP = new MCser.List();
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "ID":
                                            listTMP.ID = Convert.ToInt32(rowitem[1].ToString().Trim());
                                            listTMP.IDSpecified = true;
                                            break;
                                        case "LISTNAME":
                                            listTMP.ListName = rowitem[1].ToString().Trim();
                                            break;
                                        case "DESCRIPTION":
                                            listTMP.Description = rowitem[1].ToString().Trim();
                                            break;
                                        default:
                                            Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }
                                cResults = soapClient.Update(new MCser.UpdateOptions(), new MCser.APIObject[] { listTMP }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.UpdateDataExtension:
                                MCser.DataExtension dataExt = new MCser.DataExtension();
                                string strGUID2 = System.Guid.NewGuid().ToString();
                                dataExt.PartnerKey = strGUID2;
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "NAME":
                                            dataExt.Name = rowitem[1].ToString().Trim();
                                            break;
                                        case "DESCRIPTION":
                                            dataExt.Description = rowitem[1].ToString().Trim();
                                            break;
                                        case "NROFFIELDS":
                                            dataExt.Fields = new MCser.DataExtensionField[Convert.ToInt32(rowitem[1].ToString().Trim())];//If a list is not specified the Subscriber will be added to the "All Subscribers" List                                            
                                            break;
                                        default:
                                            if (rowitem[0].ToString().Trim().ToUpper().StartsWith("NAME"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("NAME", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1] = new MCser.DataExtensionField();
                                                dataExt.Fields[idatr-1].Name = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("DATATYPE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("DATATYPE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1].DataType = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("MAXLENGTH"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("MAXLENGTH", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1].MaxLength = Convert.ToInt32(rowitem[1].ToString().Trim());
                                                dataExt.Fields[idatr-1].MaxLengthSpecified = true;
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("DEFAULTVALUE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("DEFAULTVALUE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataExt.Fields[idatr-1].DefaultValue = rowitem[1].ToString().Trim();
                                            }
                                            else Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }
                                cResults = soapClient.Update(new MCser.UpdateOptions(), new MCser.APIObject[] { dataExt }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.UpdateDataExtensionObject:
                                MCser.DataExtensionObject dataTMP = new MCser.DataExtensionObject();
                                foreach (DataRow rowitem in parameters.Rows)
                                {
                                    switch (rowitem[0].ToString().Trim().ToUpper())
                                    {
                                        case "CUSTOMERKEY":
                                            dataTMP.CustomerKey = rowitem[1].ToString().Trim();
                                            break;
                                        case "NROFPROPERTIES":
                                            dataTMP.Properties = new MCser.APIProperty[Convert.ToInt32(rowitem[1].ToString().Trim())];//If a list is not specified the Subscriber will be added to the "All Subscribers" List                                            
                                            break;
                                        default:
                                            if (rowitem[0].ToString().Trim().ToUpper().StartsWith("NAME"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("NAME", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataTMP.Properties[idatr-1] = new MCser.APIProperty();
                                                dataTMP.Properties[idatr-1].Name = rowitem[1].ToString().Trim();
                                            }
                                            else if (rowitem[0].ToString().Trim().ToUpper().StartsWith("VALUE"))
                                            {
                                                String nr = rowitem[0].ToString().Trim().ToUpper().Replace("VALUE", "").Trim();
                                                Int32 idatr = Convert.ToInt32(nr);
                                                dataTMP.Properties[idatr-1].Value = rowitem[1].ToString().Trim();
                                            }
                                            else Response += rowitem[0].ToString().Trim() + " --> Invalid Parameter" + Environment.NewLine;
                                            break;
                                    }
                                }
                                MCser.UpdateOptions uo = new MCser.UpdateOptions();
                                uo.SaveOptions = new MCser.SaveOption[1];
                                uo.SaveOptions[0] = new MCser.SaveOption();
                                uo.SaveOptions[0].SaveAction = MCser.SaveAction.UpdateAdd;//This set this call to act as an UpSert, meaning if the Subscriber doesn't exist it will Create if it does it will Update
                                uo.SaveOptions[0].PropertyName = "*";
                                cResults = soapClient.Update(uo, new MCser.APIObject[] { dataTMP }, out cRequestID, out cStatus);
                                //rename as UPSERT
                                break;
                        }
                        Response += "Overall Status: " + cStatus + Environment.NewLine + "Number of Results: " + cResults.Length + Environment.NewLine;
                        foreach (MCser.UpdateResult cr in cResults)
                        {
                            Response += "Status Message: " + cr.StatusMessage + Environment.NewLine;
                        }
                        if (cStatus.Equals("OK")) ValidConnection = true;
                        else ValidConnection = false;
                    }
                    catch (Exception exc)
                    {
                        Response = exc.ToString();
                        ValidConnection = false;
                    }
                    break;
                case Type_of_Command.DeleteList:
                case Type_of_Command.DeleteSubscriber:                    
                case Type_of_Command.DeleteCampaign:
                case Type_of_Command.DeleteDataExtension:
                case Type_of_Command.DeleteDataExtensionObject:
                    try
                    {
                        string cRequestID = String.Empty;
                        string cStatus = String.Empty;
                        MCser.DeleteResult[] cResults = null;
                        switch (cmd)
                        {
                            case Type_of_Command.DeleteSubscriber:
                                MCser.Subscriber sub = new MCser.Subscriber();
                                sub.SubscriberKey = idDecision.Trim();
                                cResults = soapClient.Delete(new MCser.DeleteOptions(), new MCser.APIObject[] { sub }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.DeleteList:
                                MCser.List listTMP = new MCser.List();
                                listTMP.ID = Convert.ToInt32(idDecision.Trim());
                                listTMP.IDSpecified = true;
                                cResults = soapClient.Delete(new MCser.DeleteOptions(), new MCser.APIObject[] { listTMP }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.DeleteDataExtension:                                
                                MCser.DataExtension dataTMP = new MCser.DataExtension();
                                dataTMP.Name = idDecision.Trim();                      
                                cResults = soapClient.Delete(new MCser.DeleteOptions(), new MCser.APIObject[] { dataTMP }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.DeleteDataExtensionObject:
                                //trebuie 2 parametri
                                MCser.DataExtensionObject dataobj = new MCser.DataExtensionObject();
                                //listTMP.ID = Convert.ToInt32(idDecision.Trim());
                                //listTMP.IDSpecified = true;
                                cResults = soapClient.Delete(new MCser.DeleteOptions(), new MCser.APIObject[] { dataobj }, out cRequestID, out cStatus);
                                break;
                            case Type_of_Command.DeleteCampaign:
                                restCallURL = serviceURL + "/hub/v1/campaigns/" + idDecision.Trim();
                                apirequest = new HttpRequestMessage(HttpMethod.Delete, restCallURL);
                                apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                                apiCallResponse = await apiCallClient.SendAsync(apirequest);
                                requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                                Response = requestresponse;
                                ValidConnection = true;
                                if (!apiCallResponse.IsSuccessStatusCode)
                                    ValidConnection = false;
                                break;

                        }
                        if (cmd != Type_of_Command.DeleteCampaign)
                        {
                            Response = "Overall Status: " + cStatus + Environment.NewLine + "Number of Results: " + cResults.Length + Environment.NewLine;
                            foreach (MCser.DeleteResult cr in cResults)
                            {
                                Response += "Status Message: " + cr.StatusMessage + Environment.NewLine;
                            }
                            if (cStatus.Equals("OK")) ValidConnection = true;
                            else ValidConnection = false;
                        }
                    }
                    catch (Exception exc)
                    {
                        Response = exc.ToString();
                        ValidConnection = false;
                    }
                    break;
                case Type_of_Command.Connection:
                    try
                    {
                        HttpContent httpContent = new FormUrlEncodedContent(dataIN);
                        apiCallResponse = await apiCallClient.PostAsync("https://auth.exacttargetapis.com/v1/requestToken", httpContent);
                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                        JObject jsonObj = JObject.Parse(requestresponse);
                        RespAuthToken = (String)jsonObj["accessToken"];
                        String ErrorType = "";
                        ErrorType = (String)jsonObj["error"];
                        String ErrorMsg = "";
                        ErrorMsg = (String)jsonObj["error_description"];

                        ValidConnection = false;

                        if ((RespAuthToken != null && RespAuthToken != "") && ErrorMsg == null)
                        {
                            RespServiceURL = Salesforce_Marketing_Cloud_Scope.Design_ServiceURL;
                            BasicHttpsBinding binding = new BasicHttpsBinding();
                            binding.Name = "MyServicesSoap";
                            binding.CloseTimeout = TimeSpan.FromMinutes(1);
                            binding.OpenTimeout = TimeSpan.FromMinutes(1);
                            binding.ReceiveTimeout = TimeSpan.FromMinutes(60);
                            binding.SendTimeout = TimeSpan.FromMinutes(1);
                            binding.AllowCookies = false;
                            binding.BypassProxyOnLocal = false;
                            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                            binding.MaxBufferSize = 20000000;
                            binding.MaxBufferPoolSize = 20000000;
                            binding.MaxReceivedMessageSize = 20000000;
                            binding.MessageEncoding = WSMessageEncoding.Text;
                            binding.TextEncoding = System.Text.Encoding.UTF8;
                            binding.TransferMode = TransferMode.Buffered;
                            binding.UseDefaultWebProxy = true;

                            binding.ReaderQuotas.MaxDepth = 32;
                            binding.ReaderQuotas.MaxStringContentLength = 8192;
                            binding.ReaderQuotas.MaxArrayLength = 16384;
                            binding.ReaderQuotas.MaxBytesPerRead = 4096;
                            binding.ReaderQuotas.MaxNameTableCharCount = 16384;

                            binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                            binding.Security.Transport.Realm = "";
                            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                            binding.Security.Message.AlgorithmSuite = System.ServiceModel.Security.SecurityAlgorithmSuite.Default;


                            String endpointStr = "https://webservice.s10.exacttarget.com/Service.asmx";
                            EndpointAddress endpoint = new EndpointAddress(endpointStr);
                            RespSoapClient = new MCser.SoapClient(binding, endpoint);
                            RespSoapClient.ClientCredentials.UserName.UserName = Salesforce_Marketing_Cloud_Scope.Design_USER;
                            RespSoapClient.ClientCredentials.UserName.Password = Salesforce_Marketing_Cloud_Scope.Design_PASSWORD;
                            RespSoapClient.Endpoint.EndpointBehaviors.Add(new FuelOAuthHeaderBehavior(RespAuthToken));

                            ValidConnection = true;
                        }
                        else if (RespAuthToken == null && (ErrorMsg != "" && ErrorMsg != null))
                        {
                            RespAuthToken = "Error Type: " + ErrorType;
                            RespServiceURL = "Error: " + ErrorMsg;
                            RespSoapClient = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        RespServiceURL = ex.ToString();
                        RespSoapClient = null;
                        ValidConnection = false;
                    }
                    break;
                case Type_of_Command.ObjectList:
                    try
                    {                        
                        restCallURL = serviceURL + "/services/data/v33.0/sobjects";
                        apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                        apiCallResponse = await apiCallClient.SendAsync(apirequest);
                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                        List<String> sObjLst = new List<String>();
                        List<String> sObjLst2 = new List<String>();
                        ValidConnection = false;
                        DataRow row;
                        if (apiCallResponse.IsSuccessStatusCode)
                        {
                            var dataTable = new System.Data.DataTable();
                            dataTable.TableName = "TableName";
                            dataTable.Columns.Add(new DataColumn("sobject", typeof(string)) { MaxLength = 20000 });

                            JObject sObjJObj = JObject.Parse(requestresponse);
                            JToken tokens = sObjJObj["sobjects"];
                            try
                            {
                                foreach (JToken jt in tokens.Children())
                                {
                                    foreach (JProperty jp in jt)
                                    {
                                        if (jp.Name == "name")
                                        {
                                            if (jp.Value.ToString().Contains("__"))
                                                sObjLst2.Add(jp.Value.ToString());
                                            else sObjLst.Add(jp.Value.ToString());
                                        }
                                    }
                                }
                            
                                Salesforce_Marketing_Cloud_Scope.Desing_LISTOBJECT.Clear();
                                ValidConnection = true;

                                foreach (String s in sObjLst)
                                {
                                    row = dataTable.NewRow();
                                    row[0] = s;
                                    dataTable.Rows.Add(row);
                                }
                                foreach (String s in sObjLst2)
                                {
                                    row = dataTable.NewRow();
                                    row[0] = s;
                                    dataTable.Rows.Add(row);
                                }
                                DataTableResp = dataTable;
                            }
                            catch (Exception ex)
                            {
                                Error = ex.ToString();
                                DataTableResp = null;
                                ValidConnection = false;
                            }
                        }
                        else if (!(apiCallResponse.IsSuccessStatusCode))
                        {
                            DataTableResp = null;
                            ValidConnection = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        DataTableResp = null;
                        ValidConnection = false;
                    }
                    break;
                case Type_of_Command.CheckID:
                    try
                    {
                        restCallURL = serviceURL + "/services/data/v1/sobjects/" + objectName + "/" + idDecision;
                        apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                        apiCallResponse = await apiCallClient.SendAsync(apirequest);
                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                        ValidConnection = false;
                        DataRow row;
                        List<String> tmpList = new List<string>();
                        if (apiCallResponse.IsSuccessStatusCode)
                        {
                            var dataTable = new System.Data.DataTable();
                            dataTable.TableName = "TableName";
                            dataTable.Columns.Add(new DataColumn("Name", typeof(string)) { MaxLength = 200 });
                            dataTable.Columns.Add(new DataColumn("Value", typeof(string)) { MaxLength = 60000 });

                            dynamic list = JsonConvert.DeserializeObject(requestresponse);
                            foreach (var item in list)
                            {
                                if (!item.Name.ToString().Contains("attributes"))
                                {
                                    row = dataTable.NewRow();
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
                                    row["Name"] = str;
                                    row["Value"] = "";
                                    dataTable.Rows.Add(row);
                                }
                            }
                            ValidConnection = true;
                            DataTableResp = dataTable;
                        }
                        else if (!(apiCallResponse.IsSuccessStatusCode))
                        {
                            DataTableResp = null;
                            ValidConnection = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        DataTableResp = null;
                        ValidConnection = false;
                    }
                    break;             
                case Type_of_Command.GetMandatoryCampaign:                
                case Type_of_Command.GetMandatoryList:
                case Type_of_Command.GetMandatorySubscriber:
                case Type_of_Command.GetMandatoryDataExtension:
                case Type_of_Command.GetMandatoryDataExtensionObject:
                    try
                    {                       
                        Hashtable picklist = new Hashtable();                        
                        var dataTable = new System.Data.DataTable();
                        dataTable.TableName = "TableName";
                        dataTable.Columns.Add(new DataColumn("Mandatory", typeof(string)) { MaxLength = 500 });
                        dataTable.Columns.Add(new DataColumn("EditField", typeof(string)) { MaxLength = 100 });

                        DataRow row;
                        try
                        {
                            switch (cmd)
                            {
                                case Type_of_Command.GetMandatoryCampaign:
                                    row = dataTable.NewRow();row[0] = "name";row[1] = "1";dataTable.Rows.Add(row);

                                    row = dataTable.NewRow();row[0] = "description";row[1] = "1";dataTable.Rows.Add(row);

                                    row = dataTable.NewRow();row[0] = "campaignCode";row[1] = "1";dataTable.Rows.Add(row);

                                    row = dataTable.NewRow();row[0] = "color";row[1] = "1";dataTable.Rows.Add(row);

                                    row = dataTable.NewRow();row[0] = "favorite";row[1] = "1";dataTable.Rows.Add(row);

                                    break;
                                case Type_of_Command.GetMandatoryContact:
                                    row = dataTable.NewRow();row[0] = "contactKey"; row[1] = "1";dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Email Addresses"; row[1] = "0"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Email Address"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "HTML Enabled"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Email Demographics"; row[1] = "0"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Last Name"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "First Name"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Text Profile Attribute"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Number Profile Attribute"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "MobileConnect Demographics"; row[1] = "0"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Mobile Number"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Locale"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Status"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "MobilePush Demographics"; row[1] = "0"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Device ID"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Application"; row[1] = "1"; dataTable.Rows.Add(row);

                                    break;
                                case Type_of_Command.GetMandatoryList:
                                    row = dataTable.NewRow(); row[0] = "ListName"; row[1] = "1"; dataTable.Rows.Add(row);
                                    break;
                                case Type_of_Command.GetMandatorySubscriber:
                                    row = dataTable.NewRow(); row[0] = "EmailAddress"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "NrOfLists"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "ListID1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "ListID2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "NrOfAttributes"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Name1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Value1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Name2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Value2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    break;
                                case Type_of_Command.GetMandatoryDataExtension:
                                    row = dataTable.NewRow(); row[0] = "Name"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Description"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "NrOfFields"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Name1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "DataType1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Name2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "DataType2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    break;
                                case Type_of_Command.GetMandatoryDataExtensionObject:
                                    row = dataTable.NewRow(); row[0] = "CustomerKey"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "NrOfProperties"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Name1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Value1"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Name2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    row = dataTable.NewRow(); row[0] = "Value2"; row[1] = "1"; dataTable.Rows.Add(row);
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Error = ex.ToString();
                        }

                        ValidConnection = true;
                        DataTableResp = dataTable;
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        DataTableResp = null;
                        ValidConnection = false;
                    }
                    break;               
                case Type_of_Command.GenericSearch:
                    try
                    {
                        if (objectName.Trim().Length>1) restCallURL = serviceURL + "/services/data/v1/parameterizedSearch/?q=" + search.Trim().Replace(' ', '+')+"&sobject="+objectName;
                        else restCallURL = serviceURL + "/services/data/v1/parameterizedSearch/?q=" + search.Trim().Replace(' ', '+');
                        apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                        apiCallResponse = await apiCallClient.SendAsync(apirequest);

                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                        Response = requestresponse;
                        ValidConnection = true;
                        if (apiCallResponse.IsSuccessStatusCode)
                        {
                            dynamic list = JsonConvert.DeserializeObject(requestresponse);
                            var dataTable = new System.Data.DataTable();
                            dataTable.TableName = "TableName";
                            dataTable.Columns.Add(new DataColumn("Type", typeof(string)) { MaxLength = 200 });
                            dataTable.Columns.Add(new DataColumn("ID", typeof(string)) { MaxLength = 600 });
                            dataTable.Columns.Add(new DataColumn("Name", typeof(string)) { MaxLength = 60000 });
                        
                            DataRow row;
                            foreach (var item in list.searchRecords)
                            {
                                var item2 = item.attributes;
                                row = dataTable.NewRow();
                                row["Type"] = item2.type;
                                row["ID"] = item.Id;
                                if (typeOfSearch == Type_of_Search.Type_and_ID) row["Name"] = "";
                                else
                                {
                                    try
                                    {
                                        restCallURL = serviceURL + item2.url;
                                        apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                                        apiCallResponse = await apiCallClient.SendAsync(apirequest);
                                        String requestresponse2 = await apiCallResponse.Content.ReadAsStringAsync();

                                        ValidConnection = false;
                                        if (apiCallResponse.IsSuccessStatusCode)
                                        {
                                            dynamic list3 = JsonConvert.DeserializeObject(requestresponse2);
                                            foreach (var item3 in list3)
                                            {                                                
                                                if (!item3.Name.ToString().Contains("attributes"))
                                                {
                                                    if (item3.Name.ToString().Trim().ToUpper() == "NAME")
                                                    {
                                                        row["Name"] = item3.Value;
                                                        break;
                                                    }
                                                    else if (item3.Name.ToString().ToUpper() == "TITLE")
                                                    {
                                                        row["Name"] = item3.Value;
                                                        break;
                                                    }
                                                    else if (item3.Name.ToString().ToUpper() == "SUBJECT")
                                                    {
                                                        row["Name"] = item3.Value;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else if (!(apiCallResponse.IsSuccessStatusCode))
                                        {
                                            row["Name"] = "BAd conection";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Error = ex.ToString();
                                        row["Name"] = "EXCeptie";
                                    }

                                }
                                dataTable.Rows.Add(row);
                            }
                            ValidConnection = true;
                            DataTableResp = dataTable;
                        }
                        else
                        {
                            DataTableResp = null;
                            ValidConnection = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        ValidConnection = false;
                        DataTableResp = null;
                    }
                    break;
                case Type_of_Command.ExecuteReport:
                    try
                    {
                        var dataTable = new System.Data.DataTable();
                        dataTable.TableName = "TableName";
                        ValidConnection = true;
                        restCallURL = serviceURL + "/services/data/v35.0/analytics/reports/"+ idDecision.Trim() + "/instances";
                        apirequest = new HttpRequestMessage(HttpMethod.Post, restCallURL);
                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                        apiCallResponse = await apiCallClient.SendAsync(apirequest);

                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();
                        DataRow row;
                        Int32 cntRow = 0;

                        if (apiCallResponse.IsSuccessStatusCode)
                        {
                            System.Threading.Thread.Sleep(5000);
                            dynamic list = JsonConvert.DeserializeObject(requestresponse);
                            restCallURL = serviceURL + list.url;
                            apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                            apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                            apiCallResponse = await apiCallClient.SendAsync(apirequest);

                            requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                            if (apiCallResponse.IsSuccessStatusCode)
                            {
                                dynamic list2 = JsonConvert.DeserializeObject(requestresponse);
                                if (list2.reportExtendedMetadata != null)
                                {
                                    foreach (var item in list2.reportExtendedMetadata)
                                    {
                                        strtmp = item.Name;
                                        if (strtmp.Trim().ToUpper().Equals("DETAILCOLUMNINFO"))
                                        {
                                            strtmp = item.ToString();
                                            while (strtmp.Length > 5)
                                            {
                                                dataTable.Columns.Add(new DataColumn(strtmp.Substring(strtmp.IndexOf("\"label\": \"") + 10, strtmp.IndexOf("}") - strtmp.IndexOf("\"label\": \"") - 15), typeof(string)) { MaxLength = 2000 });
                                                strtmp = strtmp.Remove(0, strtmp.IndexOf("}") + 1);
                                            }
                                        }
                                    }
                                }
                                if (list2.factMap != null)
                                {
                                    foreach (var item in list2.factMap)
                                    {
                                        strtmp = item.Name;
                                        if (strtmp.Trim().ToUpper().Contains("!T"))
                                        {
                                            foreach (var item2 in item)
                                            {
                                                foreach (var item3 in item2.rows)
                                                {
                                                    try
                                                    {
                                                        row = dataTable.NewRow();
                                                        cntRow = 0;
                                                        foreach (var item4 in item3.dataCells)
                                                        {
                                                            row[cntRow] = (String)item4.label;
                                                            cntRow++;
                                                        }
                                                        dataTable.Rows.Add(row);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine(ex.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    row = dataTable.NewRow();
                                    cntRow = 0;
                                    foreach (DataColumn item4 in dataTable.Columns)
                                    {
                                        row[item4.ColumnName] = "NO_DATA";
                                    }
                                    dataTable.Rows.Add(row);
                                }
                                DataTableResp = dataTable;
                            }
                            else
                            {
                                dataTable.Columns.Add(new DataColumn("NO DATA", typeof(string)) { MaxLength = 2000 });
                                row = dataTable.NewRow();
                                row[0] = "NO VALUE";
                                dataTable.Rows.Add(row);
                                ValidConnection = false;
                                DataTableResp = dataTable;
                            }
                        }
                        else
                        {
                            dataTable.Columns.Add(new DataColumn("NO DATA", typeof(string)) { MaxLength = 2000 });
                            row = dataTable.NewRow();
                            row[0] = "NO VALUE";
                            dataTable.Rows.Add(row);
                            ValidConnection = false;
                            DataTableResp = dataTable;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        ValidConnection = false;
                        DataTableResp = null;
                    }
                    break;
                case Type_of_Command.SOQLcommand:
                    try
                    {
                        //Local variables
                        MCser.APIObject[] Results;
                        String requestID;
                        String status;
                        String objectName;

                        objectName = search.Trim().Substring(search.IndexOf(" FROM ") + 5, search.IndexOf(" WHERE ") - search.IndexOf(" FROM ") - 5);//required
                        // Instantiate the retrieve request
                        MCser.RetrieveRequest rr = new MCser.RetrieveRequest();
                        rr.ObjectType = objectName;

                        if (search.Trim().Contains(" AND "))
                        {

                            String[] strData = search.Trim().Split(new string[] { " AND " },StringSplitOptions.None);
                            // Setting up a simple filter
                            MCser.SimpleFilterPart sfl = new MCser.SimpleFilterPart();

                            String strTMP = "";
                            String[] strTMPList;
                            MCser.SimpleOperators TmpOPP;
                            ExtractValues tmp = new ExtractValues(strData[0].Trim(), out strTMP, out strTMPList, out TmpOPP);
                            sfl.Property = strTMP;
                            sfl.Value = strTMPList;
                            sfl.SimpleOperator = TmpOPP;

                            //Simple Filter Right
                            MCser.SimpleFilterPart sfr = new MCser.SimpleFilterPart();
                            tmp = new ExtractValues(strData[1].Trim(), out strTMP, out strTMPList, out TmpOPP);
                            sfr.Property = strTMP;
                            sfr.Value = strTMPList;
                            sfr.SimpleOperator = TmpOPP;

                            //Complex Filter made up of 2 SimpleFilters
                            MCser.ComplexFilterPart emCFP = new MCser.ComplexFilterPart();
                            emCFP.LeftOperand = sfl;
                            emCFP.LogicalOperator = MCser.LogicalOperators.AND;
                            emCFP.RightOperand = sfr;
                            rr.Filter = emCFP;
                        }
                        else
                        {
                            String str = search.Trim().Substring(search.IndexOf(" WHERE ") + 6,search.Trim().Length-1- search.IndexOf(" WHERE ") - 5).Trim();
                            if (!str.Trim().ToUpper().Equals("EMPTY"))
                            {
                                MCser.SimpleFilterPart sf = new MCser.SimpleFilterPart();
                                String strTMP = "";
                                String[] strTMPList;
                                MCser.SimpleOperators TmpOPP;
                                ExtractValues tmp = new ExtractValues(str, out strTMP, out strTMPList, out TmpOPP);
                                sf.Property = strTMP;
                                sf.Value = strTMPList;
                                sf.SimpleOperator = TmpOPP;
                                rr.Filter = sf;
                            }
                        }

                        String itemsSTR = search.Trim().Substring(6, search.IndexOf(" FROM ") - 6);//required
                        rr.Properties = itemsSTR.Trim().Split(',');

                        status = soapClient.Retrieve(rr, out requestID, out Results);


                        Response = status+Environment.NewLine;
                        Response += "Total Records: " + Results.Length;
                        // I NEED to RETUNR THE DATA TABLE
                        var dataTable = new System.Data.DataTable();
                        dataTable.TableName = "TableName";

                        if (Results.Length > 0)
                        {
                            ValidConnection = true;
                            foreach (String tmpcolumn in itemsSTR.Trim().Split(','))
                            {
                                dataTable.Columns.Add(new DataColumn(tmpcolumn, typeof(string)) { MaxLength = 20000 });
                            }
                            for (int i = 0; i < Results.Length; i++)
                            {
                                var row = dataTable.NewRow();
                                if (objectName.Contains("["))
                                    objectName = objectName.Remove(objectName.IndexOf("[")-1);
                                switch (objectName.Trim().ToUpper())
                                {
                                    case "LIST":
                                        MCser.List deo = Results[i] as MCser.List;
                                        foreach (String tmpcolumn in itemsSTR.Trim().Split(','))
                                        {
                                            switch (tmpcolumn)
                                            {
                                                case "ID":row["ID"] = deo.ID.ToString(); break;
                                                case "PartnerKey": row["PartnerKey"] = deo.PartnerKey.ToString(); break;
                                                case "CreatedDate": row["CreatedDate"] = deo.CreatedDate.ToString(); break;
                                                case "ModifiedDate":row["ModifiedDate"] = deo.ModifiedDate.ToString(); break;
                                                case "Client.ID": row["Client.ID"] = deo.Client.ID.ToString(); break;
                                                case "ListName": row["ListName"] = deo.ListName.ToString(); break;
                                                case "Description": row["Description"] = deo.Description.ToString(); break;
                                                case "Category":
                                                    row["Category"] = deo.Category.ToString(); break;
                                                case "Type":
                                                    row["Type"] = deo.Type.ToString(); break;
                                                case "ListClassification":
                                                    row["ListClassification"] = deo.ListClassification.ToString(); break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "SUBSCRIBER":
                                        MCser.Subscriber sub = Results[i] as MCser.Subscriber;
                                        foreach (String tmpcolumn in itemsSTR.Trim().Split(','))
                                        {
                                            switch (tmpcolumn)
                                            {
                                                case "ID": row["ID"] = sub.ID.ToString(); break;
                                                case "PartnerKey": row["PartnerKey"] = sub.PartnerKey.ToString(); break;
                                                case "CreatedDate": row["CreatedDate"] = sub.CreatedDate.ToString(); break;
                                                case "ModifiedDate": row["ModifiedDate"] = sub.ModifiedDate.ToString(); break;
                                                case "Client.ID": row["Client.ID"] = sub.Client.ID.ToString(); break;
                                                case "SubscriberKey": row["SubscriberKey"] = sub.SubscriberKey.ToString(); break;
                                                case "EmailAddress": row["EmailAddress"] = sub.EmailAddress.ToString(); break;
                                                case "PartnerType":
                                                    row["PartnerType"] = sub.PartnerType.ToString(); break;                                                
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "DATAEXTENSION":
                                        MCser.DataExtension dataobj = Results[i] as MCser.DataExtension;                                        
                                        foreach (String tmpcolumn in itemsSTR.Trim().Split(','))
                                        {
                                            switch (tmpcolumn)
                                            {
                                                case "ID": row["ID"] = dataobj.ID.ToString(); break;
                                                case "PartnerKey": row["PartnerKey"] = dataobj.PartnerKey.ToString(); break;
                                                case "CreatedDate": row["CreatedDate"] = dataobj.CreatedDate.ToString(); break;
                                                case "ModifiedDate": row["ModifiedDate"] = dataobj.ModifiedDate.ToString(); break;
                                                case "Client.ID": row["Client.ID"] = dataobj.Client.ID.ToString(); break;
                                                case "CustomerKey": row["CustomerKey"] = dataobj.CustomerKey.ToString(); break;
                                                case "ObjectID": row["ObjectID"] = dataobj.ObjectID.ToString(); break;
                                                case "ObjectState": row["ObjectState"] = dataobj.ObjectID.ToString(); break;                                                
                                                case "Name":
                                                    row["Name"] = dataobj.Name.ToString(); break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    case "DATAEXTENSIONOBJECT":
                                        MCser.DataExtensionObject dataobj2 = Results[i] as MCser.DataExtensionObject;
                                        /*foreach (String tmpcolumn in itemsSTR.Trim().Split(','))
                                        {
                                            switch (tmpcolumn)
                                            {
                                                case "ID": row["ID"] = dataobj2.ID.ToString(); break;
                                                case "PartnerKey": row["PartnerKey"] = dataobj2.PartnerKey.ToString(); break;
                                                case "CreatedDate": row["CreatedDate"] = dataobj2.CreatedDate.ToString(); break;
                                                case "ModifiedDate": row["ModifiedDate"] = dataobj2.ModifiedDate.ToString(); break;
                                                case "Client.ID": row["Client.ID"] = dataobj2.Client.ID.ToString(); break;
                                                case "CustomerKey": row["CustomerKey"] = dataobj2.CustomerKey.ToString(); break;
                                                case "ObjectID": row["ObjectID"] = dataobj2.ObjectID.ToString(); break;
                                                case "ObjectState": row["ObjectState"] = dataobj2.ObjectID.ToString(); break;
                                                case "Type": row["Type"] = dataobj2.Type.ToString(); break;
                                                case "Name": row["Name"] = dataobj2.Name.ToString(); break;
                                                default:
                                                    break;
                                            }
                                        }*/
                                        break;
                                    default:
                                        break;
                                }
                                dataTable.Rows.Add(row);
                            }
                        }
                        else
                        {
                            ValidConnection = true;                                                   
                            dataTable.Columns.Add(new DataColumn("NO DATA", typeof(string)) { MaxLength = 20000 });
                        }
                        DataTableResp = dataTable;
                    }
                    catch (Exception exc)
                    {
                        Response = exc.ToString();
                        ValidConnection = false;
                        DataTableResp = null;
                    }
                    break;
                case Type_of_Command.DownloadFile:
                    try
                    {
                        restCallURL = serviceURL + "/services/data/v1/query?q=SELECT FileExtension,Title,VersionData FROM ContentVersion WHERE ContentDocumentId='"+idDecision+"' AND IsLatest = true";
                        apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                        apiCallResponse = await apiCallClient.SendAsync(apirequest);

                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();
                        Response = requestresponse;
                        ValidConnection = true;
                        if (apiCallResponse.IsSuccessStatusCode)
                        {
                            dynamic list = JsonConvert.DeserializeObject(requestresponse);
                            String Title = "";
                            String ToDownload = "";
                            foreach (var item in list.records)
                            {
                                Title = item.Title + "." + item.FileExtension;
                                ToDownload = item.VersionData;
                                break;
                            }
                            restCallURL = serviceURL + ToDownload;
                            apirequest = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                            apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                            apiCallResponse = await apiCallClient.SendAsync(apirequest);

                            Stream s = await apiCallResponse.Content.ReadAsStreamAsync();

                            byte[] doc = null;
                            MemoryStream ms = new MemoryStream();
                            s.CopyTo(ms);
                            doc = ms.ToArray();

                            File.WriteAllBytes(nameDecision+"\\"+Title, doc);
                            if (apiCallResponse.IsSuccessStatusCode)
                            {
                                Response = nameDecision + "\\" + Title;
                            }
                            else
                            {
                                Response = "Not able to download the file";
                                ValidConnection = false;
                                DataTableResp = null;
                            }
                        }
                        else
                        {
                            ValidConnection = false;
                            DataTableResp = null;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        ValidConnection = false;
                        DataTableResp = null;
                    }
                    break;
                case Type_of_Command.SendDirectMessage:
                    try
                    {
                        restCallURL = serviceURL + "/services/data/v1/chatter/users/me/messages?recipients="+idDecision+"&text="+nameDecision.Trim().Replace(" ","+");
                          
                        apirequest = new HttpRequestMessage(HttpMethod.Post, restCallURL);
                        apirequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        apirequest.Headers.Add("Authorization", "Bearer " + authToken);
                        apiCallResponse = await apiCallClient.SendAsync(apirequest);
                        requestresponse = await apiCallResponse.Content.ReadAsStringAsync();

                        Response = " " + requestresponse;
                        ValidConnection = true;
                        if (!apiCallResponse.IsSuccessStatusCode)
                        {
                            ValidConnection = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Response = ex.ToString();
                    }
                    break;

                default:
                    break;
            }            

            return this;
        }
    }
}
