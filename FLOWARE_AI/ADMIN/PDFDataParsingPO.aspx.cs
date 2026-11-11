using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using FLOWARE_AI.App_Code;


namespace FLOWARE_AI.ADMIN
{
    public partial class PDFDataParsingPO : ClsPageEvents
    {
        private static readonly HttpClient httpClient = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {

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
                var response = await client.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                );

                string result = await response.Content.ReadAsStringAsync();
                var parsed = JObject.Parse(result);
                return parsed["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            catch(Exception ex)
            {                
                return ex.Message;
            }
        }



        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected async void btnConvertJson_Click(object sender, EventArgs e)
        {

            string inputData = txtChatData.Text.Trim();
            //string inputData = "Extract the Description,Size,Pack,Order Respond in JSON.";

            if (string.IsNullOrWhiteSpace(inputData))
            {
                // txtJsonResult.Text = "⚠ Please enter some tabular data first.";
                return;
            }


            string jsonResult = await GetJsonFromChatGPT(inputData);

            if (jsonResult.StartsWith("```json")) jsonResult = jsonResult.Substring(7).Trim();
            if (jsonResult.StartsWith("```")) jsonResult = jsonResult.Substring(3).Trim();
            if (jsonResult.EndsWith("```")) jsonResult = jsonResult.Substring(0, jsonResult.Length - 3).Trim();
            // txtJsonResult.Text = jsonResult;

            // Optional: Bind JSON to GridView
            try
            {
                JObject root = JObject.Parse(jsonResult);
                // Extract "items" array
                JArray items = (JArray)root["items"];
                // Convert to DataTable
                DataTable dt = items.ToObject<DataTable>();
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            catch
            {
                LblResult.Text = "JSON generated but could not bind to GridView.";
            }

        }



        private async Task<string> GetJsonFromChatGPT(string inputText)
        {
            try
            {

                string apiKey = "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";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "gpt-4.1-mini", // or gpt-4o-mini
                    messages = new[]
                    {
                new { role = "system", content = "You are a JSON converter. Convert tabular/plain text into structured JSON." },
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
            catch(Exception ex)
            {
                return ex.Message;
            }
        }



        protected async void btnUploadPdf_Click(object sender, EventArgs e)
        {

            if (!fileUploadPdf.HasFile)
            {
                LblResult.Text = "Please select a PDF file to upload.";
                LblResult.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // Save uploaded PDF locally
                string fileName = System.IO.Path.GetFileName(fileUploadPdf.FileName);
                string savePath = Server.MapPath("~/Firebasefile/");
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                string pdfFile = System.IO.Path.Combine(savePath, fileName);
                fileUploadPdf.SaveAs(pdfFile);
                // Process PDF using iTextSharp
                await ProcessPdfAndExtract(pdfFile);
            }
            catch (Exception ex)
            {
                LblResult.Text = "Error: " + ex.Message;
                LblResult.ForeColor = System.Drawing.Color.Red;
            }

        }


        private async Task ProcessPdfAndExtract(string pdfFile)
        {
            try
            {

                string extractedText = "";

                // Use iTextSharp to extract text from PDF
                using (PdfReader reader = new PdfReader(pdfFile))
                {
                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        extractedText += PdfTextExtractor.GetTextFromPage(reader, page);
                    }
                }

                // Clean up extracted text
                extractedText = extractedText.Replace("\r", " ").Replace("\n", " ").Trim();

                // --- Extract Header Info (PO No, Dates) using Regex ---
                string purchaseOrderNo = "";
                string createdDate = "";
                string deliveryDate = "";


                var poMatch = Regex.Match(extractedText, @"PURCHASE\s*ORDER\s*[:\-]?\s*([A-Za-z0-9\-]+)", RegexOptions.IgnoreCase);
                purchaseOrderNo = poMatch.Success ? poMatch.Groups[1].Value.Trim() : "";


                var normalizedText = Normalize(extractedText);


                var createdDate1 = ExtractDateAfterLabel(normalizedText, "Created");
                var deliveryDate1 = ExtractDateAfterLabel(normalizedText, "Delivery");

                createdDate = Regex.Replace(createdDate1, @"\s+(Created|Delivery|Created\s+By|Delivery\s+Date).*$", "", RegexOptions.IgnoreCase).Trim();
                deliveryDate = Regex.Replace(deliveryDate1, @"\s+(Created|Delivery|Created\s+By|Delivery\s+Date).*$", "", RegexOptions.IgnoreCase).Trim();

                txtPONumber.Text = !string.IsNullOrEmpty(purchaseOrderNo) ? purchaseOrderNo : "Not found";
                txtCreatedDate.Text = !string.IsNullOrEmpty(createdDate) ? createdDate : "Not found";
                txtDeliveryDate.Text = !string.IsNullOrEmpty(deliveryDate) ? deliveryDate : "Not found";

                // --- Extract Items using ChatGPT ---
                string openAiKey = "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";
                string userPrompt = txtPrompt.Text;
                string promptToSend = string.IsNullOrEmpty(userPrompt)
                    ? "Extract the Code, Item Name, UOM, Quantity, and Price from the below text. Respond in pure JSON array format."
                    : userPrompt;

                string result = await AskChatGPT(openAiKey, extractedText, promptToSend);

                // Clean ChatGPT’s response if wrapped in code fences
                if (result.StartsWith("```json")) result = result.Substring(7).Trim();
                if (result.StartsWith("```")) result = result.Substring(3).Trim();
                if (result.EndsWith("```")) result = result.Substring(0, result.Length - 3).Trim();

                // Parse JSON result to GridView
                JArray jsonArray = JArray.Parse(result);
                DataTable dt = jsonArray.ToObject<DataTable>();
                GridView1.DataSource = dt;
                GridView1.DataBind();

                ViewState["GridData"] = dt;
            }
            catch(Exception ex)
            {
                _strError = ex.Message;
            }
        }


        string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            // replace non-breaking space, newlines etc with single space and collapse multiple spaces
            s = s.Replace('\u00A0', ' ');
            s = Regex.Replace(s, @"\r\n|\r|\n", " ");
            s = Regex.Replace(s, @"\s{2,}", " ");
            return s.Trim();
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
               return ex.Message;
            }
        }


        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {

                GridView1.EditIndex = e.NewEditIndex;
                BindGrid(); // Rebind with data to enter edit mode
            }
            catch(Exception ex)
            {
                    _strError = ex.Message;
            }
        }

        private void BindGrid()
        {
            try
            {

                if (ViewState["GridData"] != null)
                {
                    GridView1.DataSource = ViewState["GridData"];
                    GridView1.DataBind();

                }
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }


        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            try
            {
                GridView1.EditIndex = -1;
                BindGrid(); // Rebind with original data
            }
            catch (Exception ex)
            {
                _strError = ex.Message;
            }
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {

                // Get DataTable from ViewState
                DataTable dt = ViewState["GridData"] as DataTable;
                if (dt == null) return;

                int rowIndex = e.RowIndex;

                // Loop through all cells and update DataTable
                for (int i = 1; i < GridView1.Columns.Count; i++) // skip CommandField (index 0)
                {
                    string columnName = dt.Columns[i - 1].ColumnName;
                    TextBox txt = GridView1.Rows[rowIndex].Cells[i].Controls[0] as TextBox;
                    if (txt != null)
                    {
                        dt.Rows[rowIndex][columnName] = txt.Text.Trim();
                    }
                }

                GridView1.EditIndex = -1;
                GridView1.DataSource = dt;
                GridView1.DataBind();
                ViewState["GridData"] = dt; // keep the changes
            }
            catch(Exception ex)
            {
                _strError = ex.Message;
            }

        }

        protected async void BtnConvertUrl_Click(object sender, EventArgs e)
        {
            try
            {
                string url = txtUrl.Text.Trim();
                if (string.IsNullOrEmpty(url))
                {
                    lblStatus.Text = "⚠️ Please enter a valid URL.";
                    return;
                }

                lblStatus.Text = "⏳ Fetching and converting data...";
                string jsonOutput = await GetJsonFromUrlAsync(url);

                //txtJsonOutput.Text = jsonOutput;

                // Try to parse JSON into DataTable
                DataTable dt = ConvertJsonToDataTable(jsonOutput);

                if (dt != null && dt.Rows.Count > 0)
                {
                    GridView1.DataSource = dt;
                    GridView1.DataBind();
                    lblStatus.Text = $"✅ Conversion complete. {dt.Rows.Count} record(s) found.";
                }
                else
                {
                    GridView1.DataSource = null;
                    GridView1.DataBind();
                    lblStatus.Text = "⚠️ No data found in JSON.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Error: " + ex.Message;
            }
        }


        private async Task<string> GetJsonFromUrlAsync(string url)
        {
            try
            {
                // Step 1: Fetch HTML from URL
                string htmlContent = await httpClient.GetStringAsync(url);

                // Step 2: OpenAI API key (replace with your valid one)
                string apiKey = "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";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Step 3: Prepare API Request
                var requestBody = new
                {
                    model = "gpt-4.1-mini",
                    messages = new[]
                    {
                    new { role = "system", content = "You are a JSON converter. Convert HTML order details into structured JSON (e.g., SupplierName, OrderDate, Items, Quantity, Price, etc.). Always return valid JSON only." },
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
                lblStatus.Text = "⚠️ JSON Parse Error: " + ex.Message;
                return null;
            }
        }

    }
}



