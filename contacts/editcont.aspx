<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editcont.aspx.cs" Inherits="contacts_editcont" Title="Contact Editor" MasterPageFile="~/Site.master" EnableViewState="false" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <table class="wide_table" style="width:798px;"><tr>
        <td style="width:199px; vertical-align:top;">
            <table class="wide_table"><tr>
                <td style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                   
                <td style="width:100%"></td>                
            </tr></table>        
        </td>
        <td style="width: 400px; vertical-align: top;">
            <h1 style="text-align: center;">contact editor</h1>
        </td>
        <td style="width:199px;"></td>        
    </tr></table>        

    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">
            <div style="width: 550px; padding: 15px; background-color: #A2C5D3">
                <div style="margin-top: 5px">
                    <asp:Label Style="color: White; font-size: 1.2em" ID="L_Partner" runat="server"/>
                </div>
                <div style="margin-top: 20px">
                    <span style="display: block; color: white; padding-left: 6px; padding-bottom: 2px">contact name</span>
                    <asp:TextBox ID="TB_ContactName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:400px; margin: 0px; padding: 3px;"></asp:TextBox>
                </div>
                <asp:Label ID="L_ContactNameError" Visible="false" runat="server" Text="" Style="display:block; margin-top:10px; color:#9F0000"></asp:Label>

                <div style="margin-top: 10px">
                    <span style="display: block; color: white; padding-left: 6px; padding-bottom: 2px">user default name</span>
                    <asp:TextBox ID="TB_UserDefaultName" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:400px; margin: 0px; padding: 3px;"></asp:TextBox>
                </div>
                <asp:Label ID="L_UserDefaultNameError" Visible="false" runat="server" Text="" Style="display:block; margin-top:10px; color:#9F0000"></asp:Label>
                
                <table class="wide_table" style="margin-top: 30px;"><tr>
                    <td class="wide_table" style="width: 50%"></td>
                    <td class="wide_table">
                        <div class="SubmitButtonEmbodied LightBlue">
                            <asp:Button ID="B_Save" runat="server" Text="Save" onclick="Save_Click" />
                        </div>
                    </td>
                    <td class="wide_table" style="width: 50%"></td>
                </tr></table>
            </div>
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>
</asp:Content>