<%@ Page Language="C#" AutoEventWireup="true" CodeFile="newdomain.aspx.cs" Inherits="newdomain" Title="New Domain" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <table class="wide_table"><tr>
        <td class="wide_table" style="width:150px; vertical-align:top;">
            <table class="auto_table"><tr>
                <td class="auto_table" style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                 
            </tr></table>        
        </td>
        <td class="wide_table" style="padding-top: 0px; vertical-align:top;">
            <h1 style="text-align: center">new domain</h1>      
        </td>
        <td class="wide_table" style="width:150px;"></td>
    </tr></table>  

    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">            
            <div style="width: 505px; border: 1px solid #A2C5D3; padding: 5px; background-color: #A2C5D3">
                <table class="wide_table"><tr>
                    <td class="wide_table" style="padding-bottom: 5px">
                        <span style="color: white">&nbsp;domain name</span> <span style="color: #3C6C80">(<span style="color: white">optional</span>)</span>
                        <asp:TextBox ID="TB_DomainName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:400px; margin: 0px; margin-top: 3px; padding: 3px;"></asp:TextBox>
                    </td>
                    <td class="wide_table" style="width:100%">
                    </td>
                    <td class="wide_table" style="padding-right: 10px; padding-top: 15px; padding-bottom: 5px">
                        <div class="SubmitButtonEmbodied LightBlue">
                            <asp:Button ID="B_CreateDomain" runat="server" Text="Create" onclick="CreateDomain_Click" />
                        </div>
                    </td>
                </tr></table>
            </div>  
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table> 
</asp:Content>