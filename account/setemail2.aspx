<%@ Page Language="C#" AutoEventWireup="true" CodeFile="setemail2.aspx.cs" Inherits="account_setemail2" EnableViewState="false" Title="Set account email" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <div style="padding-left: 10px; margin-bottom: 5px">
        <h3>my account settings</h3>
    </div>  
    <h1 style="text-align: center;">set email</h1>

    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 650px;">
                <asp:Label ID="L_Action" style="font-size: 1.2em; color: #3C6C80" runat="server" ></asp:Label>

                <table class="wide_table" style="margin-top: 20px;"><tr>
                    <td class="wide_table" style="width: 50%"></td>
                    <td class="wide_table">    
                        <asp:Panel ID="P_ApplyButton" runat="server" class="SubmitButton Blue">
                            <asp:Button ID="B_Apply" runat="server" Text="apply" onclick="Apply_Click" />
                        </asp:Panel>
                    </td>
                    <td class="wide_table" style="width: 50%"></td>
                </tr></table>
            </div>
            <div style="text-align: center;">
                <asp:Label ID="L_Message" Visible="false" Text="The email has been successfully set!" style="font-size: 1.2em;" runat="server"/>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>