<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pwdreset2.aspx.cs" Inherits="account_pwdreset2" EnableViewState="false" Title="Password reset" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <h1 style="text-align: center;">password reset</h1>
    
    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 600px; text-align: center;">
                <asp:Label ID="L_Action" style="font-size: 1.2em; color: #3C6C80" runat="server" ></asp:Label>

                <div style="margin-top: 20px; text-align:center ">
                    <asp:TextBox ID="TB_Password" AutoComplete="off" runat="server" Style="font-size:1.2em; border: 1px solid #B0B0B0; outline:none; width:390px; margin: 0px; padding: 4px;"></asp:TextBox>
                </div>
                
                <table class="wide_table" style="margin-top: 20px;"><tr>
                    <td class="wide_table" style="width: 50%"></td>
                    <td class="wide_table">    
                        <asp:Panel ID="P_ApplyButton" runat="server" class="SubmitButton Blue">
                            <asp:Button ID="B_Apply" runat="server" Text="save" onclick="Save_Click" />
                        </asp:Panel>
                    </td>
                    <td class="wide_table" style="width: 50%"></td>
                </tr></table>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>

</asp:Content>