# MES 管理端核心类与常用框架 API 参考手册

本文档列出浦林成山 MES 管理端二次开发中最常用的框架服务、接口与控件方法。

---

## 1. 核心上下文接口 `IConfig`

每个业务类 `BusniessClassBase` 与窗体均注入 `IConfig Config` 实例，提供以下基础设施：

### `Config.DataBase` (主数据库操作引擎)
- `DataTable GetTable(string sql)`：执行 SQL 查询并返回 `DataTable`。
- `int ExecuteNonQuery(string sql)`：执行增删改 SQL 语句，返回受影响行数。
- `bool InsertRow(string tableName, DataRow row)`：自动根据 DataRow 列映射执行单行插入。
- `bool DeleteRow(string tableName, DataRow row)`：根据主键删除数据行。
- `bool UpdateRow(string tableName, DataRow row)`：根据主键更新数据行。

### `Config.DataBaseList` (多数据库连接池)
MES 拥有多个分布式或接口数据库：
- `Config.DataBaseList["MESDB"]`：主 MES 业务数据库。
- `Config.DataBaseList["PLMESINTERFACE"]`：外部接口中间数据库（HR、SAP、SRM 交互）。
- `Config.DataBaseList["LONSONLOG"]`：系统日志数据库。

### `Config.FormService` (窗体工厂与反射调用)
- `DialogResult ShowDialog(string title, string viewAssemblyAndForm, string busAssemblyAndClass, object[] args)`：
  以反射解耦方式弹出对话框。
  示例：
  ```csharp
  Config.FormService.ShowDialog("新增数据", "CKA_VIEW|CKA0032W1", "CKA_BUS|CKA0032B1", new object[] { e.Row });
  ```

### `Config.SqlExec` (SQL 模板解析器)
- `string GetSqlText(string key)`：获取系统预定义的命名 SQL 模板。
  示例：
  ```csharp
  string sql = string.Format(Config.SqlExec.GetSqlText("Shift_GetData"));
  ```

### `Config.DataList` (当前登录会话全局变量)
- `Config.DataList["mEmployeeName"]`：当前登录人姓名。
- `Config.DataList["mUserNo"]` / `mUserId`：当前登录人工号/账号。
- `Config.DataList["FAC"]`：当前选中的工厂代码（如 "02"）。

---

## 2. 常用静态服务 (Static Services)

### `MESService.MessageService` (交互对话框)
- `void ShowMessage(string message)`：弹出标准信息提示框。
- `void ShowError(string errorMessage)`：弹出错误或警告提示框。
- `DialogResult ShowAsk(string question)`：弹出询问确认框（返回 `DialogResult.OK` 或 `DialogResult.Cancel`）。

### `MESService.TimeService` (服务器授时时钟)
- `DateTime GetFrameDateTime()`：获取 MES 应用服务器当前标准化时间（避免客户端本地时间不一致）。
- `DateTime GetDbDateTime()`：直接从数据库服务器获取 `GETDATE()`。

---

## 3. 专用控件方法 (UserControls)

### `LSDataGrid` (MES 数据表格控件)
- `object DataSource`：绑定 `DataTable` 数据源。
- `DataRow CurrentRow`：获取当前鼠标选中的数据行。
- `void NewRow()`：触发新增行事件。
- `void EditRow()`：触发编辑行事件。
- `void DeleteRow()`：触发删除当前选中行事件。
- 事件：`OnDataChange(object sender, LSDataRowEventArgs e)`
  - `e.ChangeType`：枚举类型，取值为 `ChangeType.New`、`ChangeType.Edit`、`ChangeType.Delete`。
  - `e.Row`：发生变更的目标 `DataRow`。
  - `e.Cancel`：布尔值，设置为 `true` 时阻止/回滚变更。

### `LSToolButton` (工具栏按钮)
- `string Title`：按钮标题（如 `"新增"`, `"查询"`, `"修改"`, `"删除"`, `"导出"`）。
- `bool ShowText`：是否显示文字文本。
- `Image SelectedImage` / `Image Image`：由 `config.ImageFactory.GetImage(btn.Title, isSelected)` 自动绑定系统图标。
