---
name: mes-management-client
description: >-
  Create, modify, and maintain Prinx Chengshan MES Management Client (05-MES管理端) WinForms modules.
  Based on real VIEW and BUS projects, using STC0005 as the worked example.
---

# 在 MES 管理端创建页面/功能 (基于 STC0005 案例)

「05-MES管理端」是多工程 WinForms（.NET 4.5 / VS 老式 csproj）。**界面 (VIEW) 与业务 (BUS) 分离成两个 DLL 工程**，由宿主 `RunEvn\Main.exe` 在运行期按字符串约定装配。

---

## 1. 弄清业务与表结构（动手前必做）

对业务逻辑或某张表的字段含义不确定时，**先查数据字典**（`D:\git\mes-major\06-数据字典\`），不要瞎猜表结构：
- 按模块分目录（如 `ST-库存管理`、`LT-LOT追溯管理`、`QM-品质管理` 等）；
- 命名格式为 `<表名>-<表中文名>.xlsx`（例 `STC0005` 对应的胎盘库存表、`WIP0002-胎胚库存.xlsx`）。

---

## 2. 目录与命名约定

- **VIEW 工程**：`D:\git\mes-major\05-MES管理端\VIEW\<域>_VIEW\<模块>_VIEW\`
- **BUS 工程**：`D:\git\mes-major\05-MES管理端\BUS\<域>_BUS\<模块>_BUS\`
  - 示例：STC0005 $\to$ `VIEW\ST_VIEW\STC_VIEW\` 与 `BUS\ST_BUS\STC_BUS\`。
- **文件成对命名**：
  - 主页面：`STC0005W.cs`、`STC0005W.Designer.cs`、`STC0005W.resx` 对应 BUS 端的 `STC0005B.cs`。
  - 弹窗子页：后缀数字递增，成对对应（如 `STC0005W1` + `STC0005B1`）。
- 同域下的工程分别被 `ST_VIEW.sln`、`ST_BUS.sln` 汇总，输出统一汇总至 `RunEvn\`。

---

## 3. 创建主页面步骤 (Worked Example: STC0005)

### 步骤 1：确定模块编码与落点
浏览对应模块目录寻找下一可用空号，确定模块代码（如 `STC0005`）。

### 步骤 2：登记工程文件 (.csproj)
`VIEW` 与 `BUS` 均为老式 csproj，新增文件需在工程文件中手动添加项：
- **VIEW 侧**：
  ```xml
  <Compile Include="STC0005W.cs">
    <SubType>Form</SubType>
  </Compile>
  <Compile Include="STC0005W.Designer.cs">
    <DependentUpon>STC0005W.cs</DependentUpon>
  </Compile>
  <EmbeddedResource Include="STC0005W.resx">
    <DependentUpon>STC0005W.cs</DependentUpon>
  </EmbeddedResource>
  ```
- **BUS 侧**：
  ```xml
  <Compile Include="STC0005B.cs" />
  ```

### 步骤 3：VIEW 侧实现 (W.cs)
参照模板 [`templates/FormViewTemplate.cs`](./templates/FormViewTemplate.cs)：
- 构造函数注入 `IConfig`，调用 `GetPic()` 循环 `panel1.Controls` 为所有 `LSToolButton` 取图；
- 暴露单例方法 `public static STC0005W Instance(IConfig Config)`；
- 窗体关闭时注销单例（`instance = null`）。

### 步骤 4：BUS 侧实现 (B.cs)
参照模板 [`templates/BusClassTemplate.cs`](./templates/BusClassTemplate.cs)：
- 继承 `BusniessClassBase`，构造函数调用 `base(Config)`；
- 控件属性通过 `GetControlByName("控件名") as 类型` 动态获取：
  ```csharp
  LSToolButton BtnFrozen => GetControlByName("BtnFrozen") as LSToolButton;
  LSDataGrid GrdMain => GetControlByName("GrdMain") as LSDataGrid;
  GetCmncode CmnCode => this.GetService(typeof(ICMNCODE)) as GetCmncode;
  ```
- `Form_Load` 执行初始查询，`GetData()` 组装 SQL 查询并绑定到 `GrdMain.DataSource`；
- **事件绑定**：工具栏按钮由框架按 `控件Name_Click` 自动反射绑定，Designer 内部不需要手工挂接 `.Click +=`。

### 步骤 5：弹窗子页实现 (W1 + B1)
需要弹窗编辑时，参照模板 [`templates/DialogViewTemplate.cs`](./templates/DialogViewTemplate.cs) 与 [`templates/DialogBusTemplate.cs`](./templates/DialogBusTemplate.cs)：
- 在 BUS 侧调用：
  ```csharp
  if (Config.FormService.ShowDialog(this.ViewForm.Text, "STC_VIEW|STC0005W1",
      "STC_BUS|STC0005B1", new object[] { 标题, arr }) == DialogResult.OK)
  {
      // 处理回传数据
  }
  ```

---

## 4. 通用写库/读库套路 (后端)

| 需求 | 标准写法 |
| :--- | :--- |
| **主库查询** | `DataTable dt = Config.DataBase.GetTable(sql);` |
| **主库单条执行** | `Config.DataBase.ExecuteNonQuery(sql);` |
| **批量写/事务** | `Config.DataBase.AddSqlString(sql);` ... `Config.DataBase.ExceSqlList(); Config.DataBase.ClearSqlString();` |
| **外部接口库** | `Config.DataBaseList["PLMESINTERFACE"]` |
| **码表下拉** | `CmnCode.GetCboCode("SHT")`（返回 DataTable，列为 `DCOD` 与 `DNAM`） |
| **系统时间** | `TimeService.GetFrameDateTime()`（使用数据库/服务器时间，严禁本地 `DateTime.Now`） |
| **弹窗提示** | `MessageService.ShowMessage("...")` / `MessageService.ShowAsk("...") == DialogResult.OK` / `MessageService.ShowError("...")` |
| **登录上下文** | `Config.DataList["mFac"]`（工厂）、`Config.DataList["mEmployeeName"]`（姓名） |

---

## 5. 给已有页面加按钮（轻量修改套路）

1. **VIEW 侧**：在 `xxxW.Designer.cs` 中的 `panel1.Controls.Add(this.BtnXxx)`，并在底部声明 `private UserControls.LSToolButton BtnXxx;`，设置 `Title`。
2. **BUS 侧**：
   - 声明属性：`LSToolButton BtnXxx => GetControlByName("BtnXxx") as LSToolButton;`
   - 增加处理方法：
     ```csharp
     public void BtnXxx_Click(object sender, EventArgs e)
     {
         DataRow[] drs = (GrdMain.DataSource as DataTable)?.Select("XZ='Y'");
         if (drs == null || drs.Length == 0)
         {
             MessageService.ShowError("请先勾选需要操作的数据行！");
             return;
         }
         if (MessageService.ShowAsk("确定执行该操作吗？") == DialogResult.OK)
         {
             // 组织 SQL 执行
             GetData();
         }
     }
     ```
