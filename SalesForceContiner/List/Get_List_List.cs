using System;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

namespace UiPathTeam.Salesforce.Marketing_Cloud
{
    [Description("Get a list of Leads from the Salesforce instance.")]
    public class Get_List_List : AsyncCodeActivity<CmdRestAPI>
    {
        public Get_List_List()
        {
            Constraints.Add(CheckParentConstraint.GetCheckParentConstraint<Get_List_List>(typeof(Salesforce_Marketing_Cloud_Scope).Name));
        }

        [Browsable(false)]
        public new CmdRestAPI Result { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> Search { get; set; }


        [Category("Output")]
        [Description("Datatable result.")]
        public OutArgument<DataTable> DataResult { get; set; }

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

            String _Search = Search.Get(context);

            var task = (new CmdRestAPI(salesForceProperty.AuthToken, salesForceProperty.ServiceURL, _Search)).ExecuteAsync();
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
            DataResult.Set(context, temp.DataTableResp);
            Response.Set(context, temp.Response);
            ValidConnection.Set(context, temp.ValidConnection);
            return temp;
        }

    }
}
