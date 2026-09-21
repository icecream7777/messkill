# MES PDA 端 API 与 WebService 协议参考手册

本文档说明浦林成山 MES 移动 PDA 端的前后端通信规范与常用接口定义。

---

## 1. 前端通信框架：MUI Ajax 规范

PDA 前端统一使用 MUI 的 `mui.ajax` 与后端 WebService 通信：

```javascript
var arr = new Array();
arr.push(storage["FAC"]);        // 参数 0: 工厂代码
arr.push(storage["LOGINNAME"]);  // 参数 1: 操作员工号
arr.push(barcode);               // 参数 2: 扫描条码
arr.push(extraParam);            // 参数 3+: 其它业务参数

mui.ajax(webUrl, {
    traditional: true,
    data: JSON.stringify({
        MethodName: "PdaService_SaveData",
        Params: arr
    }),
    dataType: 'json',
    type: 'post',
    timeout: 5000,
    headers: { 'Content-Type': 'application/json; charset=utf-8' },
    async: false,
    success: function(response) {
        var res = JSON.parse(response.d);
        if (res.Success || (Array.isArray(res) && res.length > 0)) {
            // 处理业务成功
            mui.toast("操作成功！");
        } else {
            mui.alert(res.Message || "操作失败！", "系统提示");
        }
    },
    error: function(xhr, type, errorThrown) {
        mui.alert("网络请求异常: " + type, "网络错误");
    }
});
```

---

## 2. 后端协议：C# WebService (ASMX)

后端统一采用 ASP.NET WebService 或 WCF 提供移动端服务：

```csharp
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class PdaWebService : System.Web.Services.WebService
{
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string ExecuteMethod(string MethodName, string[] Params)
    {
        try
        {
            switch (MethodName)
            {
                case "Pda_GetMaterialInfo":
                    return GetMaterialInfo(Params);
                case "Pda_SubmitTransaction":
                    return SubmitTransaction(Params);
                default:
                    return JsonConvert.SerializeObject(new { Success = false, Message = "未找到请求的处理方法" });
            }
        }
        catch (Exception ex)
        {
            return JsonConvert.SerializeObject(new { Success = false, Message = ex.Message });
        }
    }
}
```
