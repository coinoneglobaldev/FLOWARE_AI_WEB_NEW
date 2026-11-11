
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.ADMIN
{
    public partial class CustomersPendAprvList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStaticGrid();
            }
        }


        private void BindStaticGrid()
        {
            // Create a new DataTable
            DataTable dt = new DataTable();

            // Define columns (must match DataField/Eval names in GridView)
            dt.Columns.Add("SlNo", typeof(int));
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Parent", typeof(string));
            dt.Columns.Add("Remarks", typeof(string));
            dt.Columns.Add("Status", typeof(string));

            // Add sample rows (static data)
            dt.Rows.Add(1, 101, "John Doe", "EMP001", "HR", "Active employee", "Active");
            dt.Rows.Add(2, 102, "Jane Smith", "EMP002", "Finance", "On leave", "Inactive");
            dt.Rows.Add(3, 103, "Robert Brown", "EMP003", "IT", "Newly joined", "Active");
            dt.Rows.Add(4, 104, "Emily Davis", "EMP004", "Sales", "Transferred", "Active");
            dt.Rows.Add(5, 105, "Michael Johnson", "EMP005", "HR", "Promoted recently", "Active");
            dt.Rows.Add(6, 106, "Sarah Wilson", "EMP006", "Finance", "Maternity leave", "Inactive");
            dt.Rows.Add(7, 107, "David Miller", "EMP007", "IT", "Contractor", "Active");
            dt.Rows.Add(8, 108, "Laura Taylor", "EMP008", "Marketing", "Resigned notice period", "Inactive");
            dt.Rows.Add(9, 109, "Daniel Anderson", "EMP009", "HR", "Transferred from branch", "Active");
            dt.Rows.Add(10, 110, "Olivia Thomas", "EMP010", "Finance", "Part-time", "Active");
            dt.Rows.Add(11, 111, "James Jackson", "EMP011", "IT", "Remote employee", "Active");
            dt.Rows.Add(12, 112, "Sophia White", "EMP012", "Sales", "Top performer", "Active");
            dt.Rows.Add(13, 113, "Liam Harris", "EMP013", "Marketing", "New recruit", "Active");
            dt.Rows.Add(14, 114, "Isabella Martin", "EMP014", "Finance", "Pending retirement", "Inactive");
            dt.Rows.Add(15, 115, "Ethan Thompson", "EMP015", "HR", "On-site consultant", "Active");
            dt.Rows.Add(16, 116, "Ava Garcia", "EMP016", "Sales", "Field agent", "Active");
            dt.Rows.Add(17, 117, "William Martinez", "EMP017", "IT", "Probation period", "Active");
            dt.Rows.Add(18, 118, "Mia Robinson", "EMP018", "Marketing", "Team leader", "Active");
            dt.Rows.Add(19, 119, "Alexander Clark", "EMP019", "Finance", "Transferred", "Active");
            dt.Rows.Add(20, 120, "Charlotte Rodriguez", "EMP020", "HR", "Leave approved", "Inactive");
            dt.Rows.Add(21, 121, "Benjamin Lewis", "EMP021", "IT", "Promoted", "Active");
            dt.Rows.Add(22, 122, "Amelia Lee", "EMP022", "Sales", "New target achiever", "Active");
            dt.Rows.Add(23, 123, "Henry Walker", "EMP023", "Marketing", "Resigned", "Inactive");
            dt.Rows.Add(24, 124, "Ella Hall", "EMP024", "Finance", "Training assigned", "Active");
            dt.Rows.Add(25, 125, "Lucas Allen", "EMP025", "HR", "Shift change", "Active");
            dt.Rows.Add(26, 126, "Grace Young", "EMP026", "IT", "Intern", "Active");
            dt.Rows.Add(27, 127, "Jack Hernandez", "EMP027", "Sales", "Transferred branch", "Active");
            dt.Rows.Add(28, 128, "Chloe King", "EMP028", "Finance", "New hire", "Active");
            dt.Rows.Add(29, 129, "Sebastian Wright", "EMP029", "IT", "Long-term employee", "Active");
            dt.Rows.Add(30, 130, "Harper Scott", "EMP030", "Marketing", "Temporary leave", "Inactive");


            // Bind to GridView
            GrdVwRecords.DataSource = dt;
            GrdVwRecords.DataBind();
            GrdVwRecords1.DataSource = dt;
            GrdVwRecords1.DataBind();
            TabContainer1.ActiveTabIndex = 0;

        }
    }
}