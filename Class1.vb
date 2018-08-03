Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI


Public Class Class1
    Private Const AntiXsrfTokenKey As String = "__AntiXsrfTokenXYZ"
    Private Const AntiXsrfUserNameKey As String = "__AntiXsrfUserName"
    Private _antiXsrfTokenValue As String

    Public Sub SetViewStateUserKeyAndResponseCookie(ByVal page As Page)
        Dim requestCookie = page.Request.Cookies(AntiXsrfTokenKey)
        Dim requestCookieGuidValue As Guid

        If requestCookie IsNot Nothing AndAlso Guid.TryParse(requestCookie.Value, requestCookieGuidValue) Then
            _antiXsrfTokenValue = requestCookie.Value
            page.ViewStateUserKey = _antiXsrfTokenValue
        Else
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N")
            page.ViewStateUserKey = _antiXsrfTokenValue
            Dim responseCookie = New HttpCookie(AntiXsrfTokenKey) With {
                    .HttpOnly = True,
                    .Value = _antiXsrfTokenValue
                    }
            If FormsAuthentication.RequireSSL AndAlso page.Request.IsSecureConnection Then responseCookie.Secure = True
            page.Response.Cookies.[Set](responseCookie)
        End If
    End Sub

    Public Sub PageOnPreLoad(ByVal page As Page, ByVal viewState As StateBag, ByVal context As HttpContext)
        If Not page.IsPostBack Then
            viewState(AntiXsrfTokenKey) = page.ViewStateUserKey
            viewState(AntiXsrfUserNameKey) = If(context.User.Identity.Name, String.Empty)
        Else

            If CStr(viewState(AntiXsrfTokenKey)) <> _antiXsrfTokenValue OrElse CStr(viewState(AntiXsrfUserNameKey)) <> (If(context.User.Identity.Name, String.Empty)) Then
                Throw New InvalidOperationException("Validation of Anti - XSRF token failed.")
            End If
        End If
    End Sub
End Class