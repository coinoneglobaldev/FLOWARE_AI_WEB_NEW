using FLOWARE_AI.App_Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FLOWARE_AI.ADMIN
{
    public partial class Prompt : ClsPageEvents
    {
        DataTable DT;
        ClsPrompt ObjCls = new ClsPrompt();
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    FnGetPromptData();
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public void FnGetPromptData()
        {
            DT = ObjCls.FnGetPromptDetails();
            if(DT.Rows.Count>0)
            {

              DataRow[] textRows = DT.Select("cPromptType = 'TEXT'");
                if (textRows.Length > 0)
                {
                    TxtTextInput.Text = textRows[0]["cPromptData"].ToString();
                    HdnText.Value= textRows[0]["nId"].ToString();
                }                 
                DataRow[] pdfRows = DT.Select("cPromptType = 'PDF'");
                if (pdfRows.Length > 0)
                {
                    TxtPDFInput.Text = pdfRows[0]["cPromptData"].ToString();
                    HdnPDF.Value= pdfRows[0]["nId"].ToString();
                }
                DataRow[] linkRows = DT.Select("cPromptType = 'LINK'");
                if (linkRows.Length > 0)
                {
                    TxtLink.Text = linkRows[0]["cPromptData"].ToString();
                    HdnLink.Value= linkRows[0]["nId"].ToString();
                }
                DataRow[] MbodyRows = DT.Select("cPromptType = 'MBODY'");
                if (MbodyRows.Length > 0)
                {
                    TxtMailBody.Text = MbodyRows[0]["cPromptData"].ToString();
                    HdnMBody.Value = MbodyRows[0]["nId"].ToString();
                }
                TabContainer1.ActiveTabIndex = 0;

            }

        }
        protected void BtnTxtSubmit_Click(object sender, EventArgs e)
        {
            ObjCls.ID =ObjCls.FnIsNumeric(HdnText.Value);
            ObjCls.cPromptData = TxtTextInput.Text.Trim();
            ObjCls.cPromptType = "TEXT";
            ObjCls.FnSavePromptData();
        }
        protected void BtnPDFSubmit_Click(object sender, EventArgs e)
        {
            ObjCls.ID = ObjCls.FnIsNumeric(HdnPDF.Value);
            ObjCls.cPromptData = TxtPDFInput.Text.Trim();
            ObjCls.cPromptType = "PDF";
            ObjCls.FnSavePromptData();
            TabContainer1.ActiveTabIndex = 1;
        }

        protected void BtnLinkSubmit_Click(object sender, EventArgs e)
        {
            ObjCls.ID = ObjCls.FnIsNumeric(HdnLink.Value);
            ObjCls.cPromptData = TxtLink.Text.Trim();
            ObjCls.cPromptType = "LINK";
            ObjCls.FnSavePromptData();
            TabContainer1.ActiveTabIndex = 2;
        }

        protected void LnkBtnMBody_Click(object sender, EventArgs e)
        {
            ObjCls.ID = ObjCls.FnIsNumeric(HdnMBody.Value);
            ObjCls.cPromptData = TxtMailBody.Text.Trim();
            ObjCls.cPromptType = "MBODY";
            ObjCls.FnSavePromptData();
            TabContainer1.ActiveTabIndex = 3;
        }
    }
}