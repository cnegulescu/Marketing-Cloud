using System;
using System.Activities;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Description("Delete an DataExtension from the Salesforce instance. You must provide DataExtension")]
    public class Delete_DataExtension : AsyncCodeActivity<CmdRestAPI>
    {
        public Delete_DataExtension()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Delete_DataExtension>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
        }

        [Browsable(false)]
        public new CmdRestAPI Result { get; set; }

        [Category("Input")]
        [RequiredArgument]
        [Description("DataExtension Name to delete.")]
        public InArgument<string> DataExtensionName { get; set; }

        [Category("Output")]
        [Description("String response from the server.")]
        public OutArgument<string> Response { get; set; }

        [Category("Output")]
        [Description("Boolean result for connection.")]
        public OutArgument<Boolean> ValidConnection { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            var property = context.DataContext.GetProperties()[Salesforce_Marketing_Cloud_Scope.SalesForcePropertyTag];
            var salesForceProperty = property.GetValue(context.DataContext) as SalesForceProperty;

            String id = DataExtensionName.Get(context);

            var task = (new CmdRestAPI(salesForceProperty.AuthToken, salesForceProperty.ServiceURL, salesForceProperty.SoapClient, null, "DataExtension", id,"",Type_of_Command.DeleteDataExtension)).ExecuteAsync();
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
        
    }
}
