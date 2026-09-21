---
name: mes-pda-client
description: >-
  Create, modify, and maintain Prinx Chengshan MES Mobile PDA scanning modules and backend handlers.
  Based on real 03-PDA (HTML5 + MUI) and 04-服务器端程序 (ASHX + BLL) architecture.
---

# 在 MES PDA 移动端创建扫码功能

MES 移动端（手持扫码终端）运行于 `03-PDA` 前端工程与 `04-服务器端程序` 后端工程。本技能完全基于系统现有生产代码（以 `Forming/BarcodeUpdate.html`、`js/BarCodeUpdate.js` 与 `Ashx/LTA01.ashx`、`LTA0001Bll.cs` 为样例）提供直接可用的开发规约与模板。

---

## 1. 弄清业务与表结构（动手前必做）

在开发扫码功能前，**先查数据字典**（`D:\git\mes-major\06-数据字典\`），明确：
1. 扫描的条码类型（硫化条码、胎胚条码、台车条码等）；
2. 扫描后需要联查出哪些字段（如规格代码 `BUITNBR`、规格名称 `BUITDSC`、机台 `BUMCH` 等）；
3. 提交后需要更新哪张表（如 `WIP0002`、`LTA0001` 等）。

---

## 2. 目录与命名约定

- **前端页面**：`D:\git\mes-major\03-PDA\LonSon.Mobile.PrinxChengShan.App\<业务模块>\<功能名>.html`
  - 示例：`Forming\BarcodeUpdate.html`
- **前端 JS**：`D:\git\mes-major\03-PDA\LonSon.Mobile.PrinxChengShan.App\js\<功能名>.js`
  - 示例：`js\BarCodeUpdate.js`
- **后端 Handler**：`D:\git\mes-major\04-服务器端程序\LonSon.Mobile.PrinxChengShan.App.Web\Web\Ashx\<模块名>.ashx`
  - 示例：`Ashx\LTA01.ashx`
- **后端 BLL**：`D:\git\mes-major\04-服务器端程序\LonSon.Mobile.PrinxChengShan.App.Web\Mobile.PrinxChengShan.Bll\<模块名>Bll.cs`
  - 示例：`Mobile.PrinxChengShan.Bll\LTA0001Bll.cs`

---

## 3. 完整实现流程（基于现有代码例子）

### 步骤 1：前端 HTML 页面（参照 `BarcodeUpdate.html`）
在对应业务目录下创建 `.html`（模板位于 [`templates/PdaPageTemplate.html`](./templates/PdaPageTemplate.html)）：
- 顶部导航：`<header class="mui-bar mui-bar-nav">` 带有回退按钮与标题。
- 表单区域：`<div class="mui-input-group">` 内放置扫码框与只读展示框：
  ```html
  <div class="mui-input-row">
      <input type="text" id="selBARCODE" placeholder="扫描条码... " />
  </div>
  <div class="mui-input-row">
      <label id="lblBARCODE">条码:</label>
      <input type="text" id="txtBARCODE" class="mui-input" readonly="readonly" style="font-weight:bold;"/>
  </div>
  ```
- 按钮栏：包含返回按钮与确认按钮 `<button id="btnAdd" ...>确定</button>`。
- 底部状态：`<nav class="mui-bar mui-bar-tab">` 显示操作用户和工厂。
- 底部依次引入 `jquery-3.3.1.min.js`、`mui.min.js`、`utilComm.js` 及对应业务 JS。

### 步骤 2：前端 JS 脚本（参照 `BarCodeUpdate.js`）
在 `js/` 目录下创建对应 `.js`（模板位于 [`templates/PdaScriptTemplate.js`](./templates/PdaScriptTemplate.js)）：
1. **读取会话**：从 `window.localStorage` 获取 `NAME`、`FAC`、`LOGINNAME`、`Token`、`Language`。
2. **光标聚焦与扫码监听**：
   ```javascript
   var _selBARCODE = mui('#selBARCODE')[0];
   _selBARCODE.focus();
   _selBARCODE.addEventListener('keyup', function() {
       if (13 == event.keyCode || 0 == event.keyCode) {
           var barcodeVal = _selBARCODE.value.trim();
           if (barcodeVal.length >= 6) {
               queryBarcodeInfo(barcodeVal);
           }
       }
       _selBARCODE.focus();
   }, false);
   ```
3. **Ajax 交互**：通过 `mui.ajax(requestPath + '/ashx/模块.ashx', ...)` 发起 POST 请求。
4. **提交与提示**：
   - 提交前调用 `OnCheckText()` 校验必填；
   - 提交时调用 `mask.show()`，成功后 `mui.toast(...)` 提示并 `OnCleanText()` 清空；
   - 异常时调用 `mui.alert(...)`。

### 步骤 3：后端 ASHX 入口（参照 `LTA01.ashx`）
在 `Ashx/` 目录下新增处理程序（模板位于 [`templates/AshxHandlerTemplate.ashx`](./templates/AshxHandlerTemplate.ashx)）：
```csharp
public class LTA01 : IHttpHandler, IReadOnlySessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Write(new LTA0001Bll().ProcessRequest(context));
    }
    public bool IsReusable => false;
}
```

### 步骤 4：后端 BLL 逻辑（参照 `BarcodeQueryBll.cs` / `LTA0001Bll.cs`）
在 `Mobile.PrinxChengShan.Bll/` 下创建业务类（模板位于 [`templates/BllClassTemplate.cs`](./templates/BllClassTemplate.cs)）：
- **严禁建立实体类！** 后端直接通过 SQL 查出 `DataTable`，或直接处理字符串（`string`）。
- `ProcessRequest` 中获取 `context.Request["action"]`，使用 `switch-case` 分发；
- **查询数据（返回 DataTable）**：
  ```csharp
  DataTable dt = dal.GetBarcodeQueryData(barcode, fac);
  // 直接传入 dt，序列化后前端通过 data.TL 读取
  return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("0", "查询成功", dt));
  ```
- **提交/保存（返回字符串消息）**：
  ```csharp
  bool success = dal.UpdateData(barcode, fac, loginName, qty);
  return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>(success ? "0" : "1", success ? "操作成功！" : "保存失败！"));
  ```
- **异常捕获**：返回 `"500"` 及错误字符串：
  ```csharp
  return JsonHelper<Messaging<string>>.EntityToJson(new Messaging<string>("500", ex.Message));
  ```

---

## 4. 约束

- **严禁建立 Model/实体类**：所有后端查询直接返回 `DataTable`，由 `Messaging<string>` 封装到 `TL`，前端通过 `data.TL[0]` 取值；状态/提示直接用 `string`。
- 严格依循现有代码约定，不要引入不存在的第三方库或额外特性。
- 扫码输入框在任何交互后（回显成功或报错后）均应保持光标聚焦（`_selBARCODE.focus()`）。
- 提交前后使用 `mask.show()` 与 `mask.close()` 避免连击。
