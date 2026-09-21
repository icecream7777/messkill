---
name: mes-workflow-router
description: >-
  Primary router and dispatcher for Prinx Chengshan MES engineering tasks. Intelligently classifies
  and routes user requests to specialized skills: Data Dictionary Excel generation (mes-data-dictionary),
  WinForms Management Client code generation (mes-management-client), and Mobile PDA scanner apps
  (mes-pda-client), with extensible support for end-to-end multi-tier MES development.
---

# 浦林成山 MES 研发智能工作流路由 (MES Workflow Router)

本技能作为浦林成山 MES 智能化开发的核心路由中枢（Router Orchestrator），负责接收并分析用户的开发诉求、数据表结构或业务需求，并分发/调度到对应的专用子技能进行高保真代码和工程物料的生成。

---

## 1. 意图识别与路由决策表 (Routing Matrix)

当用户提出开发诉求时，AI 将提取意图特征并路由至对应的专业技能库：

```mermaid
flowchart TD
    UserReq["用户诉求 / 研发指令"] --> IntentCheck{"意图分类与特征匹配"}
    IntentCheck -->|"数据字典 / Excel / DDL / 字段设计"| SDD["skills/data-dictionary<br/>(数据字典生成)"]
    IntentCheck -->|"管理端 / WinForms / xxxW / xxxB / 窗体"| SMC["skills/management-client<br/>(管理端代码生成)"]
    IntentCheck -->|"PDA / 扫码 / 移动端 / MUI / 条码防错"| SPDA["skills/pda-client<br/>(PDA 移动端代码生成)"]
    IntentCheck -->|"端到端全流程开发 (数据表 + 管理端 + PDA)"| SE2E["全链路编排流水线 (Chain Execution)"]
```

| 意图分类 | 核心关键词 / 匹配特征 | 目标子技能 (Sub-Skill) | 输出物料 |
| :--- | :--- | :--- | :--- |
| **数据字典生成** | `数据字典`, `Excel`, `LTA1024`, `表结构`, `字段`, `DDL`, `建表`, `.xlsx`, `I/S/D/N` | [skills/data-dictionary](./skills/data-dictionary/SKILL.md) | 标准 Excel 数据字典文件、类型合规校验报告 |
| **管理端开发** | `管理端`, `WinForms`, `三层架构`, `xxxW.cs`, `xxxB.cs`, `LSDataGrid`, `FrmDialog`, `对话框`, `增删改查` | [skills/management-client](./skills/management-client/SKILL.md) | C# View/Designer/Bus 完整代码、对话框代码 |
| **PDA 移动开发** | `PDA`, `移动端`, `扫码`, `条码`, `MUI`, `扫码枪`, `WebService`, `ASM`, `防错`, `蜂鸣` | [skills/pda-client](./skills/pda-client/SKILL.md) | HTML5/MUI 页面、JS 硬件扫码脚本、C# ASMX 后端服务接口 |
| **全链路协同** | `端到端`, `新建一个XX功能包含管理端和PDA`, `根据表结构完成全套开发` | 跨技能流水线 (1 $\to$ 2 $\to$ 3) | 数据字典 Excel + 管理端窗体 + PDA 扫码端完整套件 |

---

## 2. 专用子技能索引 (Sub-Skills Catalog)

### 2.1 [数据字典生成技能 (mes-data-dictionary)](./skills/data-dictionary/SKILL.md)
- **定位**：依照标杆文件 `LTA1024-静音棉计件表.xlsx` 规范，快速生成符合企业规范的 MES 数据字典 Excel。
- **关键资产**：
  - 模板：[`template.xlsx`](./skills/data-dictionary/templates/template.xlsx)
  - 转换器：[`generate_dict.py`](./skills/data-dictionary/scripts/generate_dict.py)（支持 JSON 与 SQL DDL 直转）
  - 校验器：[`verify_dict.py`](./skills/data-dictionary/scripts/verify_dict.py)
  - 规范库：[字段类型转换映射](./skills/data-dictionary/references/type-mapping.md)、[模块命名规约](./skills/data-dictionary/references/naming-convention.md)

### 2.2 [管理端代码生成技能 (mes-management-client)](./skills/management-client/SKILL.md)
- **定位**：为 `05-MES管理端` 生产标准的 C# WinForms 业务模块。
- **关键资产**：
  - 主窗体模板：[`FormViewTemplate.cs`](./skills/management-client/templates/FormViewTemplate.cs) (单例/图标)、[`FormDesignerTemplate.cs`](./skills/management-client/templates/FormDesignerTemplate.cs)
  - 业务控制器：[`BusClassTemplate.cs`](./skills/management-client/templates/BusClassTemplate.cs) (CRUD、`LSDataGrid` 数据拦截)
  - 弹窗录入套件：[`DialogViewTemplate.cs`](./skills/management-client/templates/DialogViewTemplate.cs)、[`DialogBusTemplate.cs`](./skills/management-client/templates/DialogBusTemplate.cs)
  - 参考：[开发编码规范](./skills/management-client/references/coding-standards.md)、[API 字典](./skills/management-client/references/api-reference.md)

### 2.3 [PDA 移动端代码生成技能 (mes-pda-client)](./skills/pda-client/SKILL.md)
- **定位**：为 `03-MES-PDA` / `04-MES-PDA-NEW` 生产符合现场作业规范的手持终端扫码应用。
- **关键资产**：
  - 前端视图：[`PdaPageTemplate.html`](./skills/pda-client/templates/PdaPageTemplate.html) (MUI 卡片流、音频标签)
  - 交互脚本：[`PdaScriptTemplate.js`](./skills/pda-client/templates/PdaScriptTemplate.js) (硬件扫码回车监听、光标自锁、声光报警)
  - 后端接口：[`WebServiceMethodTemplate.cs`](./skills/pda-client/templates/WebServiceMethodTemplate.cs) (RPC 事务处理)
  - 参考：[扫码生命周期规范](./skills/pda-client/references/pda-workflow.md)、[WebService 接口参考](./skills/pda-client/references/api-reference.md)

---

## 3. 端到端全链路执行示例 (Full-Stack Orchestration)

当用户提出如：“*请根据硫化成型追溯的新需求，创建 LTA1026 表的数据字典，并生成对应的管理端维护界面以及车间 PDA 扫码录入端*”时，路由中枢将编排以下执行链：

```mermaid
sequenceDiagram
    autonumber
    actor Dev as 开发者 / AI Agent
    participant Router as MES Router (SKILL.md)
    participant Dict as mes-data-dictionary
    participant WinForm as mes-management-client
    participant PDA as mes-pda-client

    Dev->>Router: 解析复杂任务需求
    Router->>Dict: 1. 生成 LTA1026 数据字典 Excel，校验字段格式 (I/S/D/N)
    Dict-->>Router: 返回校验合格的数据字典与物理表字段模型
    Router->>WinForm: 2. 传入表模型，生成管理端 LTA1026W / LTA1026B 及弹窗录入代码
    WinForm-->>Router: 完成 WinForms 模块代码及 CSPROJ 注册指令
    Router->>PDA: 3. 传入工序校验逻辑，生成移动端 LTA1026.html / .js 及 WebService 接口
    PDA-->>Router: 完成 PDA 前后端代码
    Router-->>Dev: 汇总输出完整交付清单与工程引导
```

---

## 4. 未来技能扩展指南 (How to Add New Skills)

本架构设计充分考虑了未来功能演进，添加新技能（如报表看板、后台定时服务、看板大屏等）的标准化步骤：

1. **新建子技能目录**：在 `skills/` 下新建规范命名的文件夹（如 `skills/bi-report`、`skills/schedule-job`）。
2. **构建必要资源**：
   - `templates/`：放置官方推荐的基础代码模板。
   - `references/`：编写行业/企业规范、API 备查字典。
   - `scripts/`（可选）：提供自动化脚手架或校验脚本。
3. **编写子技能入口**：创建 `skills/{new-skill}/SKILL.md`，务必包含合法 YAML Frontmatter（`name`, `description`）。
4. **注册至主路由**：在本文件（`messkill/SKILL.md`）的第 1 节“意图识别路由表”与第 2 节“子技能索引”中加入新技能的关键字及链接。
