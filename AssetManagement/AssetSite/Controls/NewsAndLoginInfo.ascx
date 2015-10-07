<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewsAndLoginInfo.ascx.cs" Inherits="AssetSite.Controls.NewsAndLoginInfo" %>
<div id='news-area'>  
        <div class="panel info-block">
        <div class="panelheader">
            <asp:Label runat="server" ID="Label1" meta:resourcekey="NewsLabel" />
        </div>
        <div class="panelcontent"> 
                asdasdasdasdasdasd
        </div>
    </div>
    <div class="panel news-block" style="margin-left: 20px">
        <div class="panelheader">
            <asp:Label runat="server" ID="NewsLabel" meta:resourcekey="NewsLabel" />
        </div>
        <div class="panelcontent"> 
                <asp:Repeater ID="NewsList" runat="server">
                <ItemTemplate>                            
                    <div class="news-item">
                        <p>
                            <span class="news-item-date">[<%# Eval("Date") %>]</span>
                            <a href="<%# Eval("Url") %>"><%# Eval("Title") %></a>
                            <br/>                                
                            <%# Eval("Text") %>
                        </p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div class="clear"></div>
</div>