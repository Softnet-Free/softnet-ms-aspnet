<%@ Page Language="C#" AutoEventWireup="true" CodeFile="deletesite.aspx.cs" Inherits="deletesite" MasterPageFile="~/Site.master" Title="Delete site" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">   
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="padding-left: 10px; padding-bottom: 10px">
        <h3>
            <span>domain </span>
            <asp:HyperLink CssClass="h3_name domain_color" ID="HL_Domain" runat="server"/>
        </h3>
    </div>

    <h1 style="margin-bottom: 20px; text-align: center">
        <span>delete site </span><asp:Label CssClass="h1_name black_text" ID="L_Description" runat="server"/>
    </h1>

    <table class="wide_table"><tr>
        <td class="wide_table width_50"></td>
        <td class="wide_table">
            <div style="text-align: center; width:400px;">            
                <div style="text-align: left;">
                    <asp:Label CssClass="warning" ID="L_Warning" runat="server"></asp:Label>
                </div>
                <br/>
                <table class="wide_table"><tr>
                    <td class="wide_table" style="width:35%;"></td>
                    <td>
                        <div class="SubmitButton Gray">
                            <asp:Button ID="B_Cancel" runat="server" Text="Cancel" onclick="Cancel_Click" />
                        </div>
                    </td>
                    <td class="wide_table" style="width:30%;"></td>
                    <td class="wide_table">
                        <div class="SubmitButton RedOrange">
                            <asp:Button ID="B_Delete" runat="server" Text="Delete" onclick="Delete_Click" />
                        </div>
                    </td>
                    <td style="width:35%;"></td>
                </tr></table>
            </div>
        </td>
        <td class="wide_table width_50"></td>
    </tr></table>
</asp:Content>
