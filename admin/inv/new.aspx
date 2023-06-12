<%@ Page Language="C#" AutoEventWireup="true" CodeFile="new.aspx.cs" Inherits="admin_inv_new" Title="New Invitation <- Admin" MasterPageFile="~/Site.master"  MaintainScrollPositionOnPostback="true" EnableViewState="false"%>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <table class="wide_table" style="width:798px"><tr>
        <td class="wide_table" style="width:199px; vertical-align:top;">
            <table class="wide_table"><tr>
                <td class="wide_table" style="padding-left:5px;">
                    <div class="SubmitButton Gray">
                        <asp:Button ID="B_Back" runat="server" Text="back" onclick="Back_Click" />
                    </div>
                </td>                 
                <td class="wide_table" style="width:100%"></td>                
            </tr></table>        
        </td>
        <td class="wide_table" style="width: 400px; padding-top: 0px; vertical-align:top;">
            <h1 style="text-align: center;">new invitation</h1> 
        </td>
        <td class="wide_table" style="width:199px;"></td>
    </tr></table>  

    <table class="wide_table" style="margin-top: 20px;"><tr>
        <td class="wide_table" style="width: 50%"></td>
        <td class="wide_table">            
            <div style="width: 500px; padding: 10px; background-color: #A2C5D3">
                <table class="wide_table">
                <tr>
                    <td class="wide_table">
                        <span class="white_caption">e-mail</span>
                        <asp:TextBox ID="TB_EMail" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 12px">
                        <span class="white_caption">description</span>
                        <asp:TextBox ID="TB_Description" AutoComplete="off" runat="server" Style="border-width: 0px; outline:none; width:494px; margin: 0px; padding: 3px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 12px">
                        <asp:CheckBox ID="CB_AssignProviderRole" Checked="false" style="color: White; font-size: 1.1em;" Text="Assign 'Provider' role on registration" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="wide_table" style="padding-top: 25px"> 
                        <table class="wide_table"><tr>
                            <td class="wide_table" style="width:30%"></td>
                            <td class="wide_table">
                                <div class="SubmitButtonEmbodied LightBlue">
                                    <asp:Button ID="B_Cancel" runat="server" Text="cancel" onclick="Cancel_Click" />
                                </div>
                            </td>
                            <td class="wide_table" style="width:40%"></td>
                            <td class="wide_table">
                                <div class="SubmitButtonEmbodied LightBlue">
                                    <asp:Button ID="B_CreateInvitation" runat="server" Text="create" onclick="CreateInvitation_Click" />
                                </div>
                            </td>
                            <td class="wide_table" style="width:30%"></td>
                        </tr></table>
                    </td>
                </tr>
                </table>
            </div>            
        </td>
        <td class="wide_table" style="width: 50%"></td>
    </tr></table>             
</asp:Content>