using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.MasterPage
{
    public partial class MasterPageEmpty : System.Web.UI.MasterPage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ServiceReference ScrAjax = new ServiceReference();
                ScrAjax.Path = "../Web_GetResultService.asmx";
                ScriptManager1.Services.Add(ScrAjax);

                ServiceReference Scr = new ServiceReference();
                Scr.Path = "../Web_GridSelection.asmx";
                ScriptManager1.Services.Add(Scr);

                ServiceReference ScrGrid = new ServiceReference();
                ScrGrid.Path = "../WebGridSelection_Grid.asmx";
                ScriptManager1.Services.Add(ScrGrid);

            }
            catch (Exception ex)
            {

            }
        }
    }
}