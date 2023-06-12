<%@ Page Language="C#" AutoEventWireup="true" CodeFile="settings.aspx.cs" Inherits="account_settings" Title="My account settings" MasterPageFile="~/Site.master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1 style="text-align: center;">my account settings</h1>

    <table class="wide_table" style="margin-top: 25px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">            
            <div style="width: 530px;">
                <asp:Table ID="Tb_settings" CssClass="wide_table" runat="server">            
                    <asp:TableRow runat="server">
                        <asp:TableCell CssClass="wide_table" Style="color: #3C6C80; padding: 5px;" runat="server">Name</asp:TableCell>
                        <asp:TableCell CssClass="wide_table" Style="width: 360px; border: 1px solid gray;" ID="TD_OwnerName" runat="server"/>
                        <asp:TableCell CssClass="wide_table" Style="padding-left: 20px; width: 60px" runat="server">
                            <asp:Panel ID="P_OwnerNameButton" runat="server" class="SubmitButtonMini Blue">
                                <asp:Button ID="B_OwnerName" runat="server" Text="edit" onclick="OwnerName_Click" />
                            </asp:Panel>
                        </asp:TableCell>                            
                    </asp:TableRow>    
                    <asp:TableRow Style="height: 15px;" runat="server"/>
                    <asp:TableRow runat="server">
                        <asp:TableCell CssClass="wide_table" Style="color: #3C6C80; padding: 5px;" runat="server">Password</asp:TableCell>
                        <asp:TableCell CssClass="wide_table" Style="width: 360px; border: 1px solid gray;" ID="TD_Password" runat="server"/>
                        <asp:TableCell CssClass="wide_table" Style="padding-left: 20px; width: 60px" runat="server">
                            <asp:Panel ID="P_PasswordButton" runat="server" class="SubmitButtonMini Blue">
                                <asp:Button ID="B_Password" runat="server" Text="edit" onclick="Password_Click" />
                            </asp:Panel>
                        </asp:TableCell>                            
                    </asp:TableRow>    
                    <asp:TableRow Style="height: 15px;" runat="server"/>
                    <asp:TableRow runat="server">
                        <asp:TableCell CssClass="wide_table" Style="color: #3C6C80; padding: 5px;" runat="server">Email</asp:TableCell>
                        <asp:TableCell CssClass="wide_table" Style="width: 360px; border: 1px solid gray; background-color: #E9E9E9;" ID="TD_Email" runat="server"/>
                        <asp:TableCell CssClass="wide_table" Style="padding-left: 20px; width: 60px" runat="server">
                            <asp:Panel ID="P_EmailButton" runat="server" class="SubmitButtonMini Blue">
                                <asp:Button ID="B_Email" runat="server" Text="edit" onclick="Email_Click" />
                            </asp:Panel>
                        </asp:TableCell>                            
                    </asp:TableRow>    
                </asp:Table>

                <table class="wide_table" style="margin-top: 30px;"><tr>
                    <td class="wide_table" style="width: 83%"></td>
                    <td class="wide_table">
                        <asp:PlaceHolder ID="PH_ViewButton" runat="server"></asp:PlaceHolder>
                    </td>
                    <td class="wide_table" style="width: 17%"></td>
                </tr></table>

                <div style="padding-top: 40px; text-align: left; padding-left: 90px;">
                    <asp:Label ID="L_ErrorMessage" Visible="false" CssClass="failureNotification" runat="server"/>
                </div>
            </div>  
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>           
</asp:Content>