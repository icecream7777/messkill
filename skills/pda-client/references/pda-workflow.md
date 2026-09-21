# MES PDA 移动端工作流参考

本规范基于 `03-PDA` 与 `04-服务器端程序/LonSon.Mobile.PrinxChengShan.App.Web` 实际代码提炼。

---

## 1. 业务交互流程

以 `Forming/BarcodeUpdate.html` 与 `js/BarCodeUpdate.js` 为标准案例：

```mermaid
sequenceDiagram
    autonumber
    actor Worker as 操作工人 / 扫码枪
    participant UI as PDA 界面 (HTML5/MUI)
    participant JS as 控制器脚本 (.js)
    participant Ashx as 后端接口 (.ashx)
    participant BLL as 业务逻辑层 (.cs)

    Worker->>UI: 扫入条码 (触发 keyup / keyCode 13 或 0)
    UI->>JS: 获取输入框内容
    JS->>Ashx: mui.ajax (action="by", Token, FAC, LOGINNAM, BARCODE)
    Ashx->>BLL: ProcessRequest -> GetByBarcode
    BLL-->>Ashx: 返回 JSON (Messaging 对象)
    Ashx-->>JS: 返回数据包
    alt 查询成功
        JS->>UI: 回填条码、规格、数量等字段展示
    else 未查到或异常
        JS->>UI: mui.toast("未找到扫描的条码信息!")
    end
    JS->>UI: 清空扫码框并 _selBARCODE.focus()

    Worker->>UI: 点击“确定”按钮 (tap 事件)
    UI->>JS: OnCheckText() 表单必填校验
    JS->>Ashx: mask.show() -> mui.ajax (action="up", 数据参数)
    Ashx->>BLL: ProcessRequest -> UpdateData
    BLL-->>Ashx: 返回处理结果
    Ashx-->>JS: 返回响应
    JS->>UI: mui.toast("操作成功!") -> OnCleanText() -> mask.close()
```

---

## 2. 核心代码规范

### 1. 扫码框按键监听
扫码输入框绑定 `keyup` 事件，拦截 `13`（回车）与 `0`（部分扫码头）：
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

### 2. 本地会话变量
PDA 登录后将用户信息存储于 `window.localStorage`：
- `storage["Token"]`：接口身份验证令牌
- `storage["FAC"]`：所属工厂代码（如 "02"）
- `storage["FACNM"]`：工厂名称
- `storage["NAME"]`：员工姓名
- `storage["LOGINNAME"]`：员工工号
- `storage["Language"]`：多语言环境代码（如 "CHN", "ENG", "THAI"）

### 3. 数据提交与提示
- 请求前使用 `mask.show()` 开启遮罩防止重复提交，响应后调用 `mask.close()`。
- 轻量提示使用 `mui.toast(msg, { duration: tim, type: 'div' })`。
- 警告/错误弹窗使用 `mui.alert(msg, "Message")`。
