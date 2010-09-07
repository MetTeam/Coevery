﻿<%@ Control Language="C#" Inherits="Orchard.Mvc.ViewUserControl<LogOnViewModel>" %>
<%@ Import Namespace="Orchard.Users.ViewModels"%>

<h1 class="page-title"><%: Html.TitleForPage(Model.Title)%></h1>
<p><%: T("Please enter your username and password.")%> <%: Html.ActionLink(T("Register").ToString(), "Register")%><%: T(" if you don't have an account.")%></p>
<%: Html.ValidationSummary(T("Login was unsuccessful. Please correct the errors and try again.").ToString())%>
<%
using (Html.BeginFormAntiForgeryPost(Url.Action("LogOn", new {ReturnUrl = Request.QueryString["ReturnUrl"]}))) { %>
<fieldset class="login-form">
    <legend><%: T("Account Information")%></legend>
    <div class="group">
        <label for="userNameOrEmail"><%: T("Username:")%></label>
        <%: Html.TextBox("userNameOrEmail", "", new { autofocus = "autofocus" })%>
        <%: Html.ValidationMessage("userNameOrEmail")%>
    </div>
    <div class="group">
        <label for="password"><%: T("Password:")%></label>
        <%: Html.Password("password")%>
        <%: Html.ValidationMessage("password")%>
    </div>
    <div class="group">
        <%: Html.CheckBox("rememberMe")%><label class="forcheckbox" for="rememberMe"><%: T("Remember me?")%></label>
    </div>
    <input type="submit" value="<%: T("Log On") %>" />
</fieldset><%
} %>