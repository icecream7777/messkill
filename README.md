# Prinx Chengshan MES AI 智能化开发技能库 (messkill)

本项目是专为**浦林成山 MES 系统**打造的工程级 AI 技能仓库（AI Skill Repository）。通过采用**主路由与专用子技能分离（Router Orchestrator Pattern）**的架构，严格基于实际生产代码案例（如管理端 `STC0005`、PDA 端 `BarcodeUpdate` 与 `LTA01.ashx`）提炼开发规约与工程模板，不进行脱离实际的过度设计。

---

## 目录结构 (Repository Structure)

```text
messkill/
├── SKILL.md                          # [主路由技能] 智能分发与编排中枢 (mes-workflow-router)
├── README.md                         # 本文件
├── .gitignore                        # Git 忽略配置
├── skills/                           # 专用子技能目录
│   ├── data-dictionary/             # [子技能] 数据字典生成与校验 (mes-data-dictionary)
│   │   ├── SKILL.md                 # 数据字典技能入口（依循 LTA1024 标杆标准）
│   │   ├── templates/               # 标准模板 (template.xlsx)
│   │   ├── scripts/                 # 自动化脚本 (generate_dict.py, verify_dict.py)
│   │   └── references/              # 规范参考 (type-mapping.md, naming-convention.md)
│   ├── management-client/           # [子技能] MES 管理端代码生成 (mes-management-client)
│   │   ├── SKILL.md                 # 管理端技能入口（基于 STC0005 真实案例）
│   │   ├── templates/               # C# WinForms 三层架构模板 (xxxW, xxxB, xxxW1, xxxB1)
│   │   └── references/              # 编码规范与 API 字典 (coding-standards.md, api-reference.md)
│   └── pda-client/                  # [子技能] MES PDA 移动端代码生成 (mes-pda-client)
│       ├── SKILL.md                 # PDA 移动端技能入口（基于 BarcodeUpdate / LTA01 真实案例）
│       ├── templates/               # HTML5/MUI/JS 前端及 C# ASHX/BLL 后端模板
│       └── references/              # 扫码交互与接口参考 (pda-workflow.md, api-reference.md)
└── tests/                           # 技能完整性与功能回归测试集
    └── test_skills.py
```

---

## 核心技能清单 (Skills Catalog)

### 1. 路由中枢：[`SKILL.md`](./SKILL.md)
- **技能名称**：`mes-workflow-router`
- **功能**：作为统一入口，根据开发者的指令、SQL 语句或业务需求，路由到对应的数据字典、管理端或 PDA 子技能；支持跨技能的全链路端到端协同。

### 2. 数据字典技能：[`skills/data-dictionary`](./skills/data-dictionary/SKILL.md)
- **技能名称**：`mes-data-dictionary`
- **对标规范**：`06-数据字典/` 下的 `LTA1024-静音棉计件表.xlsx` 标杆格式。
- **能力特性**：
  - 支持从 JSON 表定义或直接从 SQL Server `CREATE TABLE` DDL 语句自动化生成 Excel。
  - 严格映射企业统一类型体系（`I`、`S`、`D`、`N`）。
  - 内置 `verify_dict.py` 自动化合规检查（黄色表头、宋体字体、B列细边框、类型有效性）。

### 3. 管理端代码生成技能：[`skills/management-client`](./skills/management-client/SKILL.md)
- **技能名称**：`mes-management-client`
- **对标规范**：`05-MES管理端` 生产模块（以 `STC0005` 为标准案例）。
- **能力特性**：
  - 界面与业务强解耦：`VIEW`（`xxxW.cs` / 单例 / `GetPic()`）与 `BUS`（`xxxB.cs` 继承 `BusniessClassBase`）。
  - 按钮事件由框架按 `控件Name_Click` 自动反射绑定（Designer 无 `.Click` 挂接）。
  - 弹窗采用 `xxxW1` + `xxxB1`（继承 `FrmDialog` / `BusniessDialogClassBase`）。
  - 旧式 `.csproj` 手工登记规约（`<Compile Include>`、`<DependentUpon>`）。

### 4. PDA 移动端代码生成技能：[`skills/pda-client`](./skills/pda-client/SKILL.md)
- **技能名称**：`mes-pda-client`
- **对标规范**：`03-PDA`（HTML5 + MUI）与 `04-服务器端程序`（ASHX + BLL）（以 `BarcodeUpdate.html` / `LTA01.ashx` 为标准案例）。
- **能力特性**：
  - 前端扫码框监听 `keyup`（拦截回车键码 `13` 与 `0`），保持光标聚焦（`_selBARCODE.focus()`）。
  - 会话从 `localStorage` 读取 `FAC`、`LOGINNAME`、`Token`、`NAME`。
  - 前后端统一采用 `mui.ajax` 向 `.ashx` 发起 POST 请求，后端由对应 `Bll` 类处理业务并返回 `Messaging<T>` JSON 结果。
  - 轻量交互提示：`mui.toast(...)` 与 `mui.alert(...)`。

---

## 自动化测试 (Automated Testing)

运行测试套件，验证所有子技能的 frontmatter、关键模板、数据字典生成与校验器功能：

```bash
python tests/test_skills.py
```
