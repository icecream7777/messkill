# MES PDA 移动端接口与通信参考

本文档基于 `03-PDA` 与 `04-服务器端程序/LonSon.Mobile.PrinxChengShan.App.Web` 实际代码提炼。

---

## 1. 前端 Ajax 请求格式

PDA 前端统一通过 `mui.ajax` 向后端的通用处理程序（`.ashx`）发送 POST 请求：

```javascript
mui.ajax(requestPath + '/ashx/{MODULE}.ashx', {
    data: {
        action: "by",                   // 常用 action: "by"(查条码), "up"(提交更新), "sea"(条件检索)
        Token: storage["Token"],         // 认证 Token
        FAC: storage["FAC"],             // 工厂代码 (如 "02")
        LOGINNAM: storage["LOGINNAME"],  // 登录工号
        ENAM: storage["NAME"],           // 登录人姓名
        BARCODE: barcode,                // 扫描条码
        lang: storage["Language"]        // 语言
    },
    dataType: 'json',
    type: 'post',
    timeout: 100000,
    success: function(data) {
        // data.ErrCode 为 "0" 表示业务成功
        // data.Info 包含业务实体数据
        // data.Error 包含提示或错误文本
    },
    error: function(xhr, type) {
        mui.alert("网络请求异常: " + type);
    }
});
```

---

## 2. 后端 Handler 与 Bll 标准实现

### 1. ASHX 路由入口（如 `LTA01.ashx`）
```csharp
<%@ WebHandler Language="C#" Class="LTA01" %>

using System;
using System.Web;
using Mobile.PrinxChengShan.Bll;
using System.Web.SessionState;

public class LTA01 : IHttpHandler, IReadOnlySessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Write(new LTA0001Bll().ProcessRequest(context));
    }

    public bool IsReusable => false;
}
```

### 2. BLL 业务处理（如 `LTA0001Bll.cs`）
```csharp
public class LTA0001Bll
{
    private LTA0001Dal dal = new LTA0001Dal();

    public string ProcessRequest(HttpContext context)
    {
        string action = context.Request["action"] ?? string.Empty;
        switch (action)
        {
            case "by":
                return GetByBarcode(context);
            case "up":
                return UpdateData(context);
            default:
                return JsonHelper<Messaging<string>>.EntityToJson(
                    new Messaging<string>("404", "未知操作指令"));
        }
    }
}
```

### 3. 数据应答包装（`Messaging<T>`）
- 成功：`new Messaging<T>("0", "", dataModel)`
- 业务拦截/失败：`new Messaging<string>("1", "未找到扫描的条码信息")`
- 系统异常：`new Messaging<string>("500", ex.Message)`
