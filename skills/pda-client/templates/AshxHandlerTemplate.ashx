<%@ WebHandler Language="C#" Class="{MODULE_NAME}" %>
//  功能描述(Description)：{MODULE_TITLE} Web 请求入口
//  参考代码：04-服务器端程序/LonSon.Mobile.PrinxChengShan.App.Web/Web/Ashx/LTA01.ashx

using System;
using System.Web;
using Mobile.PrinxChengShan.Bll;
using System.Web.SessionState;

public class {MODULE_NAME} : IHttpHandler, IReadOnlySessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Write(new {MODULE_NAME}Bll().ProcessRequest(context));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
