using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web.UI;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Xml;
using System.Collections;
using FLOWARE_AI.App_Code;
using System.Web.Services;

namespace FLOWARE_AI.ADMIN
{
    public partial class OrderParsing : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();
        ClsCustomerItemMapping ObjCls = new ClsCustomerItemMapping();
        ClsAIMailDetails objMail = new ClsAIMailDetails();
        ClsOrderParsing ObjParse = new ClsOrderParsing();

        public class MailSummary
        {
            public string UniqueId { get; set; }
            public string FromEmail { get; set; }
            public string EmailSubject { get; set; }
            public DateTime DateReceived { get; set; }
            public string EmailBody { get; set; }
            public List<string> AttachmentPaths { get; set; } = new List<string>();
            public List<string> Links { get; set; } = new List<string>();
            public Dictionary<string, string> LinkContents { get; set; } = new Dictionary<string, string>();
        }
        private static List<MailSummary> _staticCache = new List<MailSummary>();
        private List<MailSummary> CachedMails
        {
            get => _staticCache;
            set => _staticCache = value ?? new List<MailSummary>();
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.Page_Load(sender, e);
                if (!IsPostBack)
                {
                    FillDropdownList();
                    FnInitializeForm();
                }
               // RegisterGridControlMasterScript();
                CtrlGrdItemCode.DestinationControls = CtrlGrdItem.IdControl + "," + CtrlGrdItem.TextControl;
                CtrlGrdItem.DestinationControls = CtrlGrdItemCode.IdControl + "," + CtrlGrdItemCode.TextControl;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public override void FnInitializeForm()
        {
            try
            {
                int iCmpId = FnGetRights().COMPANYID, iBrId = FnGetRights().BRANCHID, iFaId = FnGetRights().FAYEARID;
                ObjCls = new ClsCustomerItemMapping(iCmpId, iBrId, iFaId);
                FnAssignPrompt();
                ViewState["DT"] = objMail.FnGetMailMasterData();
                ViewState["DT_CHILD"] = FnGetGeneralXml(ObjCls);
                FnGridViewBinding("MAIL");
                TabContainer1.ActiveTabIndex = 0;
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public void FnGridViewBinding(string PrmFlag)
        {
            if (PrmFlag.Equals("MAIL"))
            {
                GrdVwRecords.DataSource = ViewState["DT"] as DataTable;
                //GrdVwRecords.DataKeyNames = new String[] { ObjCls.KeyName };
                GrdVwRecords.DataBind();
                GrdVwRecords.SelectedIndex = -1;
            }
            else
            {
                GrdVwOrderDtls.DataSource = ViewState["DT_CHILD"] as DataTable;
                //GrdVwOrderDtls.DataKeyNames = new String[] { objMail.KeyName };
                GrdVwOrderDtls.DataBind();
                GrdVwOrderDtls.SelectedIndex = -1;
                TabContainer1.ActiveTabIndex = 1;
            }
        }
        public void FnClear()
        {
            TxtPONo.Text = "";
            CtrlOrderDate.DateText = "";
            CtrlDelDate.DateText = "";
            FnInitializeForm();
        }
        public void FnClearItems()
        {
            CtrlGrdItemCode.FnClearControls();
            CtrlGrdItem.FnClearControls();
            DdlUomAdd.SelectedIndex = 0;
            TxtOrderQty.Text = "0";
        }
        protected void FillDropdownList()
        {
            try
            {
                DataTable DtU = objMail.FnGetItemUOMData();
                DdlUomAdd.DataSource = DtU;
                DdlUomAdd.DataTextField = "UomName";
                DdlUomAdd.DataValueField = "UomId";
                DdlUomAdd.DataBind();
                DdlUomAdd.Items.Insert(0, new ListItem("--Select--", ""));
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        public void ManiPulateDataEvent_Clicked(object sender, EventArgs e)
        {
        }
        public void FnAssignPrompt()
        {
            try
            {
                DT_TEMP = objMail.FnGetPromptData();
                if (DT_TEMP.Rows.Count > 0)
                {
                    foreach (DataRow row in DT_TEMP.Rows)
                    {
                        string promptType = Convert.ToString(row["cPromptType"]).Trim().ToUpper();
                        string promptData = Convert.ToString(row["cPromptData"]).Trim();

                        switch (promptType)
                        {
                            case "PDF":
                                ViewState["PromptPDF"] = promptData;
                                break;
                            case "LINK":
                                ViewState["PromptLINK"] = promptData;
                                break;
                            case "TEXT":
                                ViewState["PromptTEXT"] = promptData;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        string FnNormalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            // replace non-breaking space, newlines etc with single space and collapse multiple spaces
            s = s.Replace('\u00A0', ' ');
            s = Regex.Replace(s, @"\r\n|\r|\n", " ");
            s = Regex.Replace(s, @"\s{2,}", " ");
            return s.Trim();
        }
        static async Task<string> AskChatGPT(string apiKey, string inputText, string instruction)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                var payload = new
                {
                    model = "gpt-4.1-mini", // cheaper + fast
                    messages = new object[]
                    {
                new { role = "system", content = instruction },
                new { role = "user", content = inputText }
                    }
                };
                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                );
                string result = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(result);
                return parsed["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private async Task<string> GetJsonFromUrlAsync(string url)
        {
            try
            {
                // Step 1: Fetch HTML from URL
                string htmlContent = await httpClient.GetStringAsync(url);
                // Step 2: OpenAI API key (replace with your valid one)
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                string prompt = "";
                if (ViewState["PromptLINK"] != null)
                {
                    prompt = ViewState["PromptLINK"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert HTML order details into structured JSON (e.g., SupplierName, OrderDate, Items, Quantity, Price, etc.). Always return valid JSON only.";
                }
                var requestBody = new
                {
                    model = "gpt-4.1-mini",
                    messages = new[]
                    {
                    new { role = "system", content =prompt },
                    new { role = "user", content = htmlContent }
                }
                };
                var response = await httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                );
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result?.choices?[0]?.message?.content ?? "No JSON output received.";
                // Step 5: Clean output — remove ```json, ``` etc.
                jsonOutput = Regex.Replace(jsonOutput, @"^```json\s*|\s*```$", "", RegexOptions.Multiline).Trim();
                jsonOutput = Regex.Replace(jsonOutput, @"^```|```$", "", RegexOptions.Multiline).Trim();
                return jsonOutput.Trim();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Server.MapPath("~/ErrorLog.txt"),
                    $"{DateTime.Now}: {ex.Message}\n");
                return "Error: " + ex.Message;
            }
        }
        private DataTable ConvertJsonToDataTable(string json)
        {
            try
            {
                // Clean unwanted formatting (if GPT adds text around JSON)
                json = json.Trim();
                // Try to parse JSON directly
                JToken token = JToken.Parse(json);
                // If JSON contains array (like "Items"), use it
                if (token.Type == JTokenType.Array)
                {
                    return JsonConvert.DeserializeObject<DataTable>(json);
                }
                else if (token.Type == JTokenType.Object)
                {
                    // If object contains an array property (like "Items")
                    var obj = (JObject)token;
                    foreach (var property in obj.Properties())
                    {
                        if (property.Value.Type == JTokenType.Array)
                        {
                            return JsonConvert.DeserializeObject<DataTable>(property.Value.ToString());
                        }
                    }
                    // Otherwise, single object → create one-row DataTable
                    DataTable dt = new DataTable();
                    foreach (var prop in obj.Properties())
                    {
                        dt.Columns.Add(prop.Name);
                    }
                    DataRow row = dt.NewRow();
                    foreach (var prop in obj.Properties())
                    {
                        row[prop.Name] = prop.Value.ToString();
                    }
                    dt.Rows.Add(row);
                    return dt;
                }
                return null;
            }
            catch (Exception ex)
            {
                //lblStatus.Text = "⚠️ JSON Parse Error: " + ex.Message;
                return null;
            }
        }
        private async Task<string> GetJsonFromChatGPT(string inputText)
        {
            try
            {
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                string prompt = "";
                if (ViewState["PromptTEXT"] != null)
                {
                    prompt = ViewState["PromptTEXT"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert tabular/plain text into structured JSON.";
                }
                var requestBody = new
                {
                    model = "gpt-4.1-mini", // or gpt-4o-mini
                    messages = new[]
                    {
                new { role = "system", content =prompt },
                new { role = "user", content = inputText }
                    }
                };
                var response = await httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                );
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result.choices[0].message.content;
                return jsonOutput.Trim();
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private async Task ProcessManualPdfAndExtract(string pdfFile)
        {
            try
            {
                string extractedText = "";
                using (PdfReader reader = new PdfReader(pdfFile))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                        extractedText += PdfTextExtractor.GetTextFromPage(reader, page);
                }
                extractedText = extractedText.Replace("\r", " ").Replace("\n", " ").Trim();
                string openAiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                string promptToSend = "";
                if (ViewState["PromptPDF"] != null)
                {
                    promptToSend = ViewState["PromptPDF"].ToString();
                }
                else
                {
                    promptToSend = "Extract the Code, Item Name, UOM, Quantity, and Price from the below text. Respond in pure JSON array format.";
                }

                string result = await AskChatGPT(openAiKey, extractedText, promptToSend);
                // Clean markdown fences if present
                result = result.Replace("```json", "").Replace("```", "").Trim();
                JArray jsonArray = JArray.Parse(result);
                foreach (var obj in jsonArray)
                {
                    var cust = obj["CustomerName"]?.ToString();
                    if (!string.IsNullOrEmpty(cust))
                    {
                        // Remove prefix 'Williams of London' and extra spaces
                        cust = Regex.Replace(cust, @"^.*?\bKitchen\b", "Kitchen", RegexOptions.IgnoreCase).Trim();
                        obj["CustomerName"] = cust;
                    }
                }
                DataTable dt = new DataTable();
                if (jsonArray.Count > 0)
                {
                    JObject first = (JObject)jsonArray[0];
                    TxtPONo.Text = (string)first["OrderNumber"] ?? "";
                    CtrlOrderDate.DateText = (string)first["OrderDate"] ?? "";
                    CtrlDelDate.DateText = (string)first["DeliveryDate"] ?? "";
                    LblCustName.Text = (string)first["CustomerName"] ?? "";
                    if (first["Items"] != null && first["Items"].Type == JTokenType.Array)
                    {
                        JArray itemsArray = (JArray)first["Items"];
                        dt = itemsArray.ToObject<DataTable>();
                    }
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!dt.Columns.Contains("Id"))
                    {
                        dt.Columns.Add("Id", typeof(string));
                        dt.Columns.Add("SlNo", typeof(int));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Id"] = "";
                            row["SlNo"] = nSlNo++;
                        }
                    }
                    if (dt.Columns.Contains("ProductID"))
                        dt.Columns["ProductID"].ColumnName = "AICode";
                    if (dt.Columns.Contains("ItemCode"))
                        dt.Columns["ItemCode"].ColumnName = "AICode";
                    if (!dt.Columns.Contains("Code"))
                    {
                        dt.Columns.Add("Code", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Code"] = "";
                        }
                    }
                    if (dt.Columns.Contains("code"))
                        dt.Columns["code"].ColumnName = "AICode";
                    if (dt.Columns.Contains("Name"))
                        dt.Columns["Name"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("Product"))
                        dt.Columns["Product"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("Item Name"))
                        dt.Columns["Item Name"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("description"))
                        dt.Columns["description"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("ItemName"))
                        dt.Columns["ItemName"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("Unit"))
                        dt.Columns["Unit"].ColumnName = "UOM";
                    if (dt.Columns.Contains("Count"))
                        dt.Columns["Count"].ColumnName = "Quantity";
                    if (dt.Columns.Contains("DefaultQuantity"))
                        dt.Columns["DefaultQuantity"].ColumnName = "Quantity";
                    if (dt.Columns.Contains("ItemId"))
                        dt.Columns["ItemId"].ColumnName = "ItemId";
                    if (dt.Columns.Contains("UomId"))
                        dt.Columns["UomId"].ColumnName = "UomId";
                    if (dt.Columns.Contains("UomName"))
                        dt.Columns["UomName"].ColumnName = "UomName";
                    if (!dt.Columns.Contains("ItemName"))
                    {
                        dt.Columns.Add("ItemName", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["ItemName"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("UOM"))
                    {
                        dt.Columns.Add("UOM", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["UOM"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("Quantity"))
                    {
                        dt.Columns.Add("Quantity", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Quantity"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("Price"))
                    {
                        dt.Columns.Add("Price", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Price"] = "";
                        }
                    }
                    if (!dt.Columns.Contains("ItemCode"))
                        dt.Columns.Add("ItemCode", typeof(string));
                    if (!dt.Columns.Contains("ItemId"))
                        dt.Columns.Add("ItemId", typeof(string));
                    if (!dt.Columns.Contains("UomId"))
                        dt.Columns.Add("UomId", typeof(string));
                    if (!dt.Columns.Contains("UomName"))
                        dt.Columns.Add("UomName", typeof(string));
                    GrdVwOrderDtls.DataSource = dt;
                    HdnType.Value = "PDF";
                    ViewState["DTGEN"] = dt;
                    GrdVwOrderDtls.DataBind();
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        string ExtractDateAfterLabel(string text, string label)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return "";
                // locate label (handles "Created", "Created On", "Created Date", etc.)
                var labelPattern = $@"\b{Regex.Escape(label)}\b\s*(?:On|Date)?\s*[:\-]?\s*";
                var labelMatch = Regex.Match(text, labelPattern, RegexOptions.IgnoreCase);
                if (!labelMatch.Success) return "";
                // take a small snippet right after the label (adjust length if needed)
                int start = labelMatch.Index + labelMatch.Length;
                int maxLen = Math.Min(120, text.Length - start);
                string snippet = text.Substring(start, maxLen);
                // date patterns to try (order: most specific -> more permissive)
                string[] datePatterns = new[]
                {
              // 22/Sep/25 or 22-Sep-25 with optional time and AM/PM
              @"\d{1,2}[\/\-][A-Za-z]{3}[\/\-]\d{2,4}(?:\s+\d{1,2}:\d{2}(?::\d{2})?\s*(?:AM|PM)?)?",
             // 22/09/2025 or 22-09-2025 with optional time
              @"\d{1,2}[\/\-]\d{1,2}[\/\-]\d{2,4}(?:\s+\d{1,2}:\d{2}(?::\d{2})?\s*(?:AM|PM)?)?",
            // 22 Sep 2025 or 22 September 2025
              @"\d{1,2}\s+[A-Za-z]{3,9}\s+\d{2,4}(?:\s+\d{1,2}:\d{2}(?::\d{2})?\s*(?:AM|PM)?)?",
            // September 22, 2025  (month first)
             @"[A-Za-z]{3,9}\s+\d{1,2},?\s+\d{2,4}"
            };
                foreach (var p in datePatterns)
                {
                    var m = Regex.Match(snippet, p, RegexOptions.IgnoreCase);
                    if (m.Success) return m.Value.Trim();
                }
                // final fallback: grab the first short token that looks like a date (e.g. "22/Sep/25" or "22/Sep/25 5:26:45 PM")
                var fallback = Regex.Match(snippet, @"[A-Za-z0-9\/\-\s:,]{6,40}");
                return fallback.Success ? fallback.Value.Trim() : "";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
        private async Task<string> GetJsonFromUrlLinkAsync(string url)
        {
            try
            {
                // Step 1: Fetch HTML from URL
                string htmlContent = await httpClient.GetStringAsync(url);
                // Step 2: OpenAI API key (replace with your valid one)
                string apiKey = ConfigurationManager.AppSettings["OpenAIKey"];
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                // Step 3: Prepare API Request
                string prompt = "";
                if (ViewState["PromptLINK"] != null)
                {
                    prompt = ViewState["PromptLINK"].ToString();
                }
                else
                {
                    prompt = "You are a JSON converter. Convert HTML order details into structured JSON (e.g., SupplierName, OrderDate, Items, Quantity, Price, etc.). Always return valid JSON only.";
                }
                var requestBody = new
                {
                    model = "gpt-4.1-mini",
                    messages = new[]
                    {
                    new { role = "system", content = prompt },
                    new { role = "user", content = htmlContent }
                }
                };
                var response = await httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
                );
                string responseContent = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseContent);
                string jsonOutput = result?.choices?[0]?.message?.content ?? "No JSON output received.";
                jsonOutput = Regex.Replace(jsonOutput, @"^```json\s*|\s*```$", "", RegexOptions.Multiline).Trim();
                jsonOutput = Regex.Replace(jsonOutput, @"^```|```$", "", RegexOptions.Multiline).Trim();
                return jsonOutput.Trim();
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(Server.MapPath("~/ErrorLog.txt"),
                    $"{DateTime.Now}: {ex.Message}\n");
                return "Error: " + ex.Message;
            }
        }
        protected async void FileUploadDoc_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                if (FileUploadDoc.HasFile)
                {
                    // Get file name and save path
                    string fileName = System.IO.Path.GetFileName(FileUploadDoc.FileName);
                    string savePath = Server.MapPath("~/Firebasefile/");
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    string pdfFile = System.IO.Path.Combine(savePath, fileName);
                    FileUploadDoc.SaveAs(pdfFile);
                    HdnFilePath.Value = pdfFile;
                    Session["UploadedFilePath"] = pdfFile;
                    LblResult.Text = "File uploaded successfully!";
                    LblResult.ForeColor = System.Drawing.Color.Green;
                    ScriptManager.GetCurrent(this).RegisterPostBackControl(BtnPDFSubmit); // Ensure the button is a full postback control if necessary, but forcing an update is usually sufficient.
                    ScriptManager.GetCurrent(this).SetFocus(BtnPDFSubmit); // Optional: Set focus to the next action button
                }
                else
                {
                    LblResult.Text = "No file selected.";
                    LblResult.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                LblResult.Text = "Error: " + ex.Message;
                LblResult.ForeColor = System.Drawing.Color.Red;
            }
        }
        protected void TabContainer1_ActiveTabChanged(object sender, EventArgs e)
        {
            FnClear();
        }
        protected async void BtnLinkSubmit_Click1(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showLoader", "showLoader();", true);
                string url = TxtLink.Text.Trim();
                if (string.IsNullOrEmpty(url))
                {
                    return;
                }
                string jsonOutput = await GetJsonFromUrlLinkAsync(url);

                JObject jsonObject = JObject.Parse(jsonOutput);
                TxtPONo.Text = jsonObject["OrderNumber"]?.ToString();
                CtrlOrderDate.DateText = jsonObject["OrderDate"]?.ToString();
                CtrlDelDate.DateText = jsonObject["DeliveryDate"]?.ToString();
                LblCustName.Text = jsonObject["CustomerName"]?.ToString();

                DataTable dt = ConvertJsonToDataTable(jsonOutput);
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (!dt.Columns.Contains("Id"))
                    {
                        dt.Columns.Add("Id", typeof(string));
                        dt.Columns.Add("SlNo", typeof(int));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Id"] = "";
                            row["SlNo"] = nSlNo++;
                        }
                    }
                    if (dt.Columns.Contains("Product"))
                        dt.Columns["Product"].ColumnName = "ItemName";
                    if (dt.Columns.Contains("code"))
                        dt.Columns["code"].ColumnName = "AICode";
                    if (dt.Columns.Contains("Name"))
                        dt.Columns["Name"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("Product"))
                        dt.Columns["Product"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("Item Name"))
                        dt.Columns["Item Name"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("description"))
                        dt.Columns["description"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("ItemName"))
                        dt.Columns["ItemName"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("ItemId"))
                        dt.Columns["ItemId"].ColumnName = "ItemId";
                    if (dt.Columns.Contains("ItemId"))
                        dt.Columns["ItemId"].ColumnName = "ItemId";
                    if (dt.Columns.Contains("UomId"))
                        dt.Columns["UomId"].ColumnName = "UomId";
                    if (dt.Columns.Contains("UomId"))
                        dt.Columns["UomName"].ColumnName = "UomName";
                    if (!dt.Columns.Contains("UomName"))
                    {
                        dt.Columns.Add("ItemName", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["ItemName"] = "";
                        }
                    }

                    if (!dt.Columns.Contains("UOM"))
                    {
                        dt.Columns.Add("UOM", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["UOM"] = ""; // default empty value
                        }
                    }
                    if (!dt.Columns.Contains("Price"))
                    {
                        dt.Columns.Add("Price", typeof(string));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Price"] = ""; // default empty value
                        }
                    }
                    if (!dt.Columns.Contains("ItemCode"))
                        dt.Columns.Add("ItemCode", typeof(string));
                    if (!dt.Columns.Contains("ItemId"))
                        dt.Columns.Add("ItemId", typeof(string));
                    if (!dt.Columns.Contains("UomId"))
                        dt.Columns.Add("UomId", typeof(string));
                    if (!dt.Columns.Contains("UomName"))
                        dt.Columns.Add("UomName", typeof(string));
                    GrdVwOrderDtls.DataSource = dt;
                    HdnType.Value = "LINK";
                    ViewState["DTGEN"] = dt;
                    GrdVwOrderDtls.DataBind();
                }
                else
                {
                    GrdVwOrderDtls.DataSource = null;
                    GrdVwOrderDtls.DataBind();
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "alertScript", "hideLoader();", true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected async void BtnPDFSubmit_Click1(object sender, EventArgs e)
        {
            try
            {
                string filePath = "";
                // Priority 1: Session
                if (Session["UploadedFilePath"] != null)
                    filePath = Session["UploadedFilePath"].ToString();
                // Priority 2: Hidden field
                if (string.IsNullOrEmpty(filePath))
                    filePath = HdnFilePath.Value;

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    await ProcessManualPdfAndExtract(filePath);
                    LblResult.Text = "File uploaded successfully!";
                    LblResult.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    LblResult.Text = "No file selected.";
                    LblResult.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                LblResult.Text = "Error: " + ex.Message;
                LblResult.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected async void BtnTxtSubmit_Click1(object sender, EventArgs e)
        {
            string inputData = TxtAreainput.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputData))
            {
                return;
            }
            string jsonResult = await GetJsonFromChatGPT(inputData);
            if (jsonResult.StartsWith("```json")) jsonResult = jsonResult.Substring(7).Trim();
            if (jsonResult.StartsWith("```")) jsonResult = jsonResult.Substring(3).Trim();
            if (jsonResult.EndsWith("```")) jsonResult = jsonResult.Substring(0, jsonResult.Length - 3).Trim();
            try
            {
                JArray jsonArray = JArray.Parse(jsonResult);
                foreach (var obj in jsonArray)
                {
                    var cust = obj["CustomerName"]?.ToString();
                    if (!string.IsNullOrEmpty(cust))
                    {
                        // Remove prefix 'Williams of London' and extra spaces
                        cust = Regex.Replace(cust, @"^.*?\bKitchen\b", "Kitchen", RegexOptions.IgnoreCase).Trim();
                        obj["CustomerName"] = cust;
                    }
                }
                if (jsonArray.Count > 0)
                {
                    JObject first = (JObject)jsonArray[0];
                    TxtPONo.Text = (string)first["OrderNumber"] ?? "";
                    CtrlOrderDate.DateText = (string)first["OrderDate"] ?? "";
                    CtrlDelDate.DateText = (string)first["DeliveryDate"] ?? "";
                    LblCustName.Text = (string)first["CustomerName"] ?? "";
                }
                // Convert to DataTable
                JArray itemsArray = (JArray)jsonArray[0]["Items"];
                DataTable dt = itemsArray.ToObject<DataTable>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Rename columns if needed
                    if (!dt.Columns.Contains("Id"))
                    {
                        dt.Columns.Add("Id", typeof(string));
                        dt.Columns.Add("SlNo", typeof(int));
                        foreach (DataRow row in dt.Rows)
                        {
                            row["Id"] = "";
                            row["SlNo"] = nSlNo++;
                        }
                    }
                    if (dt.Columns.Contains("code"))
                        dt.Columns["code"].ColumnName = "AICode";
                    if (dt.Columns.Contains("Item Name"))
                        dt.Columns["Item Name"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("ItemName"))
                        dt.Columns["ItemName"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("description"))
                        dt.Columns["description"].ColumnName = "AIItemName";
                    if (dt.Columns.Contains("unit"))
                        dt.Columns["unit"].ColumnName = "UOM";
                    if (dt.Columns.Contains("quantity"))
                        dt.Columns["quantity"].ColumnName = "Quantity";
                    if (dt.Columns.Contains("unit_price"))
                        dt.Columns["unit_price"].ColumnName = "Price";
                    if (dt.Columns.Contains("ItemId"))
                        dt.Columns["ItemId"].ColumnName = "ItemId";
                    if (dt.Columns.Contains("UomId"))
                        dt.Columns["UomId"].ColumnName = "UomId";
                    if (dt.Columns.Contains("UomName"))
                        dt.Columns["UomName"].ColumnName = "UomName";

                    if (!dt.Columns.Contains("Code"))
                        dt.Columns.Add("Code", typeof(string));
                    if (!dt.Columns.Contains("ItemName"))
                        dt.Columns.Add("ItemName", typeof(string));
                    if (!dt.Columns.Contains("UOM"))
                        dt.Columns.Add("UOM", typeof(string));
                    if (!dt.Columns.Contains("Quantity"))
                        dt.Columns.Add("Quantity", typeof(string));
                    if (!dt.Columns.Contains("Price"))
                        dt.Columns.Add("Price", typeof(string));
                    if (!dt.Columns.Contains("ItemCode"))
                        dt.Columns.Add("ItemCode", typeof(string));
                    if (!dt.Columns.Contains("ItemId"))
                        dt.Columns.Add("ItemId", typeof(string));
                    if (!dt.Columns.Contains("UomId"))
                        dt.Columns.Add("UomId", typeof(string));
                    if (!dt.Columns.Contains("UomName"))
                        dt.Columns.Add("UomName", typeof(string));
                    GrdVwOrderDtls.DataSource = dt;
                    HdnType.Value = "TEXT";
                    ViewState["DTGEN"] = dt;
                    GrdVwOrderDtls.DataBind();
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void BtnExportData_Click1(object sender, EventArgs e)
        {
            try
            {
                if (CtrlGrdCustDtl.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
                    return;
                }
                ObjCls.CustomerId = ObjCls.FnIsNumeric(CtrlGrdCustDtl.SelectedValue);
                ObjCls.MailId = ObjCls.FnIsNumeric(HdnmailId.Value);
                ObjCls.FromMail = HdnMailFrom.Value;

                HiddenField HdnId, hdnItemId;
                Label LblCode, LblName, LblUom;
                CtrlGridViewList CtrlGrdViewItemDtl, CtrlGrdViewUom;

                foreach (GridViewRow row in GrdVwOrderDtls.Rows)
                {
                    HdnId = (HiddenField)row.FindControl("HdnId");
                    LblCode = (Label)row.FindControl("LblCode");
                    LblName = (Label)row.FindControl("LblName");
                    LblUom = (Label)row.FindControl("LblUom");
                    CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                    CtrlGrdViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");
                    hdnItemId = (HiddenField)row.FindControl("HdnItemId");

                    ObjCls.ItmCode = LblCode.Text.ToString();
                    ObjCls.ItemDesc = LblName.Text.ToString();
                    ObjCls.ItmUom = LblUom.Text.ToString();

                    ObjCls.ItemId = ObjCls.FnIsNumeric(CtrlGrdViewItemDtl.SelectedValue);
                    ObjCls.UomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.SplitUomId = ObjCls.FnIsNumeric(CtrlGrdViewUom.SelectedValue);
                    ObjCls.FnUpdateAIOrderDetails();
                }
                ObjCls.FnUpdateCustomerDetails();
                ObjCls.FnUpdateCheckStatus();
                ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Record Inserted Successfully '); }, 100);", true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void BtnAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                objMail.nFItemId = ObjCls.FnIsNumeric(CtrlGrdItem.SelectedValue);
                objMail.cMailUId = HdnmailId.Value;
                objMail.nFUomId = ObjCls.FnIsNumeric(DdlUomAdd.SelectedValue);
                objMail.Qty = ObjCls.FnIsDouble(TxtOrderQty.Text.Trim());
                //DataTable DTAdd = objMail.FnAddNewItemToOrder();
                DataTable DTM = objMail.FnGetOrderData();
                if (DTM.Rows.Count > 0)
                {
                    GrdVwOrderDtls.DataSource = DTM;
                    GrdVwOrderDtls.DataBind();
                }
                FnClearItems();

            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }

        }
        private void RegisterGridControlMasterScript()
        {
            try
            {
                string cMasterScript = FnRegisterGridControlMasterScript();
                ScriptManager.RegisterStartupScript(this, GetType(), "grid_master_" + DateTime.Now.Ticks, cMasterScript, true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void GrdVwOrderDtls_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    Button btnDelete = e.Row.FindControl("BtnDelete") as Button;
                    if (btnDelete != null)
                    {
                        string subject = e.Row.Cells[1].Text; // Subject column index
                        btnDelete.OnClientClick = $"return confirm('Delete mail');";
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void GrdVwOrderDtls_PreRender(object sender, EventArgs e)
        {
            try
            {
                return;
                //FnUom();
                base.OnPreRender(e);
                SetupGridChildControls();
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        private void SetupGridChildControls()
        {
            try
            {
                CtrlGridViewList CtrlGrdViewItemCode, CtrlGrdViewItemDtl, CtrlGridViewUom;
                foreach (GridViewRow row in GrdVwOrderDtls.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CtrlGrdViewItemCode = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemCode");
                        CtrlGrdViewItemDtl = (CtrlGridViewList)row.FindControl("CtrlGrdViewItemDtl");
                        CtrlGridViewUom = (CtrlGridViewList)row.FindControl("CtrlGrdViewUom");

                        if (CtrlGrdViewItemCode != null)
                        {
                            CtrlGrdViewItemCode.DestinationControls = CtrlGrdViewItemDtl.IdControl + "," + CtrlGrdViewItemDtl.TextControl + "," + CtrlGridViewUom.IdControl + "," + CtrlGridViewUom.TextControl;
                        }
                        if (CtrlGrdViewItemDtl != null)
                        {
                            CtrlGrdViewItemDtl.DestinationControls = CtrlGrdViewItemCode.IdControl + "," + CtrlGrdViewItemCode.TextControl + "," + CtrlGridViewUom.IdControl + "," + CtrlGridViewUom.TextControl;
                        }
                        CtrlGridViewUom.ParentControl = CtrlGrdViewItemDtl.IdControl;
                    }
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void BtnAssignMap_Click(object sender, EventArgs e)
        {
            try
            {
                if (CtrlGrdCustDtl.SelectedValue == "0")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "custAlert", "setTimeout(function(){ alert('Please select a valid customer'); }, 100);", true);
                    return;
                }
                objMail.cMailUId = HdnmailId.Value;
                objMail.nCustomerId = objMail.FnIsNumeric(CtrlGrdCustDtl.SelectedValue.ToString());
                DataTable DTExtract = ViewState["DTGEN"] as DataTable;
                if (DTExtract != null && DTExtract.Rows.Count > 0)
                {
                    foreach (DataRow dRow in DTExtract.Rows)
                    {
                        ObjParse.cOrderNo = TxtAreainput.Text.Trim();
                        ObjParse.dOrderDate = ObjCls.FnDateTime(CtrlOrderDate.DateText);
                        ObjParse.dDeliveryDate = ObjCls.FnDateTime(CtrlDelDate.DateText);
                        ObjParse.nCustomerId = ObjCls.FnIsNumeric(CtrlGrdCustDtl.SelectedValue);
                        ObjParse.cAIItemCode = dRow["AICode"].ToString().Trim();
                        ObjParse.cAIItemName = dRow["AIItemName"].ToString().Trim();
                        ObjParse.Qty = ObjCls.FnIsDouble(dRow["Quantity"].ToString().Trim());
                        ObjParse.cUomName = dRow["UOM"].ToString().Trim();
                        ObjParse.nPrice = ObjCls.FnIsDecimal(dRow["Price"].ToString().Trim());
                        ObjParse.TType = HdnType.Value;
                        ObjParse.FnAddExtractItemAsNewOrder();
                    }
                    DataTable DTAE = ObjParse.FnGetExtractItemAsNewOrder();
                    GrdVwOrderDtls.DataSource = DTAE;
                    GrdVwOrderDtls.DataBind();
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void GrdVwRecords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            return;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(GrdVwRecords, "Select$" + e.Row.RowIndex);
                e.Row.Attributes["onclick"] += $";setScrollPosition({e.Row.RowIndex});";
                e.Row.Style["cursor"] = "pointer";

                if (e.Row.RowIndex == GrdVwRecords.SelectedIndex)
                {
                    e.Row.CssClass += " selected-mail-row";
                }
                if (DataBinder.Eval(e.Row.DataItem, "nIsChk").ToString() == "1")
                {
                    e.Row.CssClass += "export-row1";
                }
            }
        }
        protected void GrdVwRecords_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, GetType(), "SetFocusRow", $"setFocusRow({GrdVwRecords.SelectedIndex - 1});", true);
        }
        protected void GrdVwRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow selectedRow = GrdVwRecords.SelectedRow;
                HiddenField HdnFromMail = (HiddenField)selectedRow.FindControl("HdnFrMail");
                HdnMailFrom.Value = HdnFromMail != null ? HdnFromMail.Value : "";

                DataTable Dt = null;
                FnClear();
                if (GrdVwRecords.SelectedDataKey == null) return;
                ViewState["SelectedIndex"] = GrdVwRecords.SelectedIndex;
                HdnmailId.Value = GrdVwRecords.SelectedDataKey.Value.ToString();
                objMail.cMailUId = HdnmailId.Value;
                objMail.cFromMail = HdnMailFrom.Value;
                Dt = objMail.FnGetOrderData();

                if (Dt != null && Dt.Rows.Count > 0)
                {
                    DataRow firstRow = Dt.Rows[0];
                    TxtPONo.Text = Convert.ToString(firstRow["OrderNo"]);
                    CtrlOrderDate.DateText = firstRow["OrderDate"] != DBNull.Value ? Convert.ToDateTime(firstRow["OrderDate"]).ToString("dd/MMM/yyyy") : "";
                    CtrlDelDate.DateText = firstRow["DeliveryDate"] != DBNull.Value ? Convert.ToDateTime(firstRow["DeliveryDate"]).ToString("dd/MMM/yyyy") : "";
                    LblCustName.Text = Convert.ToString(firstRow["AICustomerName"]);
                    CtrlGrdCustDtl.SelectedValue = Convert.ToString(firstRow["CustomerId"]);
                    CtrlGrdCustDtl.SelectedText = Convert.ToString(firstRow["CustomerName"]);
                }
                else
                {
                    TxtPONo.Text = "";
                    CtrlOrderDate.DateText = "";
                    CtrlDelDate.DateText = "";
                    CtrlGrdCustDtl.SelectedText = "";
                    LblCustName.Text = "";
                }
                ViewState["MailUId"] = HdnmailId.Value;
                GrdVwOrderDtls.DataSource = Dt;
                GrdVwOrderDtls.DataBind();
                int selectedIndex = GrdVwRecords.SelectedIndex;
                string script = $"setFocusRow({selectedIndex});";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "OpenViewEmail", "openViewEmailsWinBox();", true);
                ClientScript.RegisterStartupScript(this.GetType(), "SetFocus", script, true);
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
        protected void GrdVwOrderDtls_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "DeleteMail")
                {
                    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    Label LblCode = (Label)row.FindControl("LblCode");
                    Label LblName = (Label)row.FindControl("LblName");

                    string mailId = HdnmailId.Value;
                    ObjParse.ID = Convert.ToInt32(e.CommandArgument);
                    ObjParse.cAIItemCode = LblCode.Text;
                    ObjParse.cAIItemName = LblName.Text;
                    ObjParse.FnDeleteMailDataItems();

                    HiddenField HdnFromMail = (HiddenField)row.FindControl("HdnFrMail");
                    HdnMailFrom.Value = HdnFromMail != null ? HdnFromMail.Value : "";
                    objMail.cMailUId = HdnmailId.Value;
                    objMail.cFromMail = HdnMailFrom.Value;
                    DT_TEMP = objMail.FnGetOrderData();
                    GrdVwOrderDtls.DataSource = DT_TEMP;
                    GrdVwOrderDtls.DataBind();

                    ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Deleted!');", true);
                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void GrdVwRecords_SelectedIndexChanging1(object sender, GridViewSelectEventArgs e)
        {
            try
            {
                GridViewRow selectedRow = GrdVwRecords.SelectedRow;
                HiddenField HdnFromMail = (HiddenField)selectedRow.FindControl("HdnFrMail");
                HdnMailFrom.Value = HdnFromMail != null ? HdnFromMail.Value : "";
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }
    }
}
