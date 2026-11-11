<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/MasterPageEmpty.Master" AutoEventWireup="true" CodeBehind="ViewEmail.aspx.cs" Inherits="FLOWARE_AI.ADMIN.ViewEmail" Async="true" StylesheetTheme="SkinFile" Theme="SkinFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .view-email-container {
            width: 100%;
            height: 100%;
            overflow-y: auto;
            overflow-x: hidden;
        }

        .mail-body {
            width: 100%;
            padding: 5px;
            box-sizing: border-box;
        }

        .pdf-container {
            width: 100%;
            margin-top: 15px;
            text-align: center;
        }

        .pdf-page {
            display: block;
            margin: 10px auto;
            max-width: 100%;
            height: auto;
        }

    </style>
    <div class="view-email-container">
        <div class="mail-body">
            <asp:Literal ID="LitMailBody" runat="server"></asp:Literal>
            
        </div>        
        <div id="pdfContainer" class="pdf-container">
        </div>
    </div>
    <script type="module">

        import * as pdfjsLib
            from "../JS/pdf.min.js";

        pdfjsLib.GlobalWorkerOptions.workerSrc = "../JS/pdf.worker.min.js";
        window.loadPDF = async function (pdfUrl) {

            const container =document.getElementById("pdfContainer");
            container.innerHTML = "";
            try
            {
                const response = await fetch(pdfUrl);
                if (!response.ok) {
                    throw new Error("PDF HTTP error: " + response.status);
                }
                const pdfData =await response.arrayBuffer();
                const pdf =await pdfjsLib.getDocument({data: pdfData}).promise;
                for (let pageNumber = 1; pageNumber <= pdf.numPages; pageNumber++)
                {
                    const page =await pdf.getPage(pageNumber);
                    const viewport =page.getViewport({scale: 1.5});
                    const canvas =document.createElement("canvas");
                    canvas.className ="pdf-page";
                    const context =canvas.getContext("2d");
                    canvas.width =viewport.width;
                    canvas.height =viewport.height;
                    container.appendChild(canvas);
                    await page.render({canvasContext: context,viewport: viewport}).promise;
                }
            }
            catch (error)
            {
                container.innerHTML = "<b>Unable to load PDF.</b><br/>" +error.message;
                console.error("PDF loading error:",error);
            }
        };

    </script>

</asp:Content>
