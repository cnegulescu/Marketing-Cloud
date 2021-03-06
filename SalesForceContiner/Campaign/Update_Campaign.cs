﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;

using System.Text;
using System.Threading.Tasks;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Browsable(false)]
    [Description("Update an campaign from the Salesforce instance. You must provide Campaign ID.")]
    public class Update_Campaign : AsyncCodeActivity<CmdRestAPI>
    {
        [Browsable(false)]
        public new CmdRestAPI Result { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("String Campaign ID to be updated.")]
        public InArgument<String> CampaignID { get; set; }

        [Browsable(false)]
        public List<ParametersArgument> Parameters { get; set; }

        [Browsable(false)]
        public Type_of_Command cmdTYPE { get; set; }

        [Category("Output")]
        [Description("String response from the server.")]
        public OutArgument<string> Response { get; set; }

        [Category("Output")]
        [Description("Boolean result for connection.")]
        public OutArgument<Boolean> ValidConnection { get; set; }

        public Update_Campaign()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Update_Campaign>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
            Parameters = new List<ParametersArgument>();
            cmdTYPE = Type_of_Command.UpdateCampaign;
        }
        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var property = context.DataContext.GetProperties()[Salesforce_Marketing_Cloud_Scope.SalesForcePropertyTag];
            var salesForceProperty = property.GetValue(context.DataContext) as SalesForceProperty;

            String id = CampaignID.Get(context);

            var newDataTable = new System.Data.DataTable();
            newDataTable.TableName = "TableName";
            newDataTable.Columns.Add(new DataColumn("Parameter", typeof(string)) { MaxLength = 200 });
            newDataTable.Columns.Add(new DataColumn("Value", typeof(string)) { MaxLength = 60000 });
            newDataTable.Columns.Add(new DataColumn("isEnabled", typeof(Boolean)) { DefaultValue = true });
            foreach (ParametersArgument item in Parameters)
            {
                var row = newDataTable.NewRow();
                row["Parameter"] = item.Parameter.Get(context);
                row["Value"] = item.ValueData.Get(context);
                newDataTable.Rows.Add(row);
            }

            var task = (new CmdRestAPI(salesForceProperty.AuthToken, salesForceProperty.ServiceURL, salesForceProperty.SoapClient, newDataTable, "campaigns", id,"",cmdTYPE)).ExecuteAsync();
            var tcs = new TaskCompletionSource<CmdRestAPI>(state);

            task.ContinueWith(t =>
            {
                if (t.IsFaulted) tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(t.Result);
                if (callback != null) callback(tcs.Task);
            });

            return tcs.Task;           
        }

        protected override CmdRestAPI EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            CmdRestAPI temp = ((Task<CmdRestAPI>)result).Result;
            Response.Set(context, temp.Response);
            ValidConnection.Set(context, temp.ValidConnection);
            return temp;
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {            
            base.CacheMetadata(metadata);
            ValidateParameters(metadata);
        }

        private void ValidateParameters(CodeActivityMetadata metadata)
        {
            if (Parameters == null)
            {
                return;
            }

            var counter = 0;
            foreach (var paramItem in Parameters)
            {
                var toType1 = typeof(object);
                var toType2 = typeof(object);


                toType1 = paramItem.Parameter.ArgumentType;
                toType2 = paramItem.ValueData.ArgumentType;

                var toArgument = new RuntimeArgument("Arg" + counter, toType1, ArgumentDirection.In);
                metadata.Bind(paramItem.Parameter, toArgument);
                metadata.AddArgument(toArgument);
                counter++;

                var toArgument1 = new RuntimeArgument("Arg" + counter, toType2, ArgumentDirection.In);
                metadata.Bind(paramItem.ValueData, toArgument1);
                metadata.AddArgument(toArgument1);
                counter++;
            }
        }
    }
}
