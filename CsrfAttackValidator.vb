Imports System
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Runtime.InteropServices

Namespace WebApplication1
    Public Class CsrfAttackValidator
        Private Const AntiXsrfTokenKey As String = "__AntiXsrfToken"
        Private Const AntiXsrfUserNameKey As String = "__AntiXsrfUserName"
        Private _antiXsrfTokenValue As String

        Public Function GetAntiXsrfTokenKey() As String
            Return AntiXsrfTokenKey
        End Function

        Public Function GetAntiXsrfUserNameKey() As String
            Return AntiXsrfUserNameKey
        End Function

        Public Function GetViewStateUserKey(ByVal request As HttpRequest) As String
            Dim requestCookieGuidValue As Guid
            Dim requestCookie = request.Cookies(AntiXsrfTokenKey)

            If requestCookie IsNot Nothing AndAlso Guid.TryParse(requestCookie.Value, requestCookieGuidValue) Then
                _antiXsrfTokenValue = requestCookie.Value
            End If

            Return _antiXsrfTokenValue
        End Function

        Public Function GetNewAntiXsrfToken(ByVal request As HttpRequest, <Out> ByRef httpResponseCookie As HttpCookie) As String
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N")
            Dim responseCookie = New HttpCookie(AntiXsrfTokenKey) With {
                .HttpOnly = True,
                .Value = _antiXsrfTokenValue
            }
            If FormsAuthentication.RequireSSL AndAlso request.IsSecureConnection Then responseCookie.Secure = True
            httpResponseCookie = responseCookie
            Return _antiXsrfTokenValue
        End Function

        Public Sub ValidateAntiXsrfTokenAndUserNameOnPageLoad(ByVal context As HttpContext, ByVal viewState As StateBag)
            If CStr(viewState(AntiXsrfTokenKey)) <> _antiXsrfTokenValue OrElse CStr(viewState(AntiXsrfUserNameKey)) <> (If(context.User.Identity.Name, String.Empty)) Then
                Throw New InvalidOperationException("Validation of Anti - XSRF token failed.")
            End If
        End Sub
    End Class
End Namespace
