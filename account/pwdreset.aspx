<%@ Page Language="C#" AutoEventWireup="true" CodeFile="pwdreset.aspx.cs" Inherits="account_pwdreset" EnableViewState="false" Title="Password Reset" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">    
    <h1 style="text-align: center;">password reset</h1>

    <table class="wide_table" style="margin-top: 15px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">    
            <div style="width: 400px;">
                <span style="font-size: 1.2em; color: #3C6C80">                    
                    Type your account name or email in the text box and click <span style="font-weight: bold;">send recovery mail</span>.                    
                </span>

                <div style="margin-top: 20px;">
                    <asp:TextBox ID="TB_TargetName" AutoComplete="off" runat="server" Style="font-size:1.2em; border: 1px solid #B0B0B0; outline:none; width:390px; margin: 0px; padding: 4px;"></asp:TextBox>
                
                    <table class="wide_table" style="margin-top: 20px;"><tr>
                        <td class="wide_table" style="width: 50%"></td>
                        <td class="wide_table">  
                            <asp:Panel ID="P_SendMailButton" class="SubmitButton Blue" runat="server">
                                <asp:Button ID="B_SendMail" runat="server" Text="send recovery mail" onclick="SendMail_Click" />                                    
                            </asp:Panel>
                        </td>
                        <td class="wide_table" style="width: 50%"></td>
                    </tr></table>
                </div>

                <asp:Panel ID="P_Success" Visible="false" runat="server">
                    <div style="margin-top: 15px; margin-bottom: 15px; text-align: center">
                        <asp:Label ID="L_Success" runat="server" style="font-size: 1.2em;"></asp:Label>
                    </div>
                    <table class="wide_table"><tr>
                        <td class="wide_table" style="width:50%"></td>
                        <td class="wide_table">
                            <div class="SubmitButton Blue">
                                <asp:Button ID="B_OK" runat="server" Text="ok"/>
                            </div>
                        </td>
                        <td class="wide_table" style="width:50%"></td>
                    </tr></table>
                </asp:Panel>

                <asp:Panel ID="P_Error" Visible="false" runat="server">
                    <div style="margin-top: 15px; margin-bottom: 15px; text-align: center">
                        <asp:Label ID="L_Error" style="font-size: 1.2em; color: #9F0000" runat="server"/>
                    </div>
                    <table class="wide_table"><tr>
                        <td class="wide_table" style="width:50%"></td>
                        <td class="wide_table">
                            <div class="SubmitButton Gray">
                                <asp:Button ID="B_Error" runat="server" Text="ok"/>
                            </div>
                        </td>
                        <td class="wide_table" style="width:50%"></td>
                    </tr></table>
                </asp:Panel>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>
