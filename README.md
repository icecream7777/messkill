# Prinx Chengshan MES AI 智能化开发技能库 (messkill)

本项目是专为**浦林成山 MES 系统**打造的工程级 AI 技能仓库（AI Skill Repository）。通过采用**主路由与专用子技能分离（Router Orchestrator Pattern）**的架构，使 AI 助手或工程人员在面对 MES 研发任务时，能够精准匹配业务上下文、严格依循企业级代码与样式规范，自动化生产高质量工程物料。

---

## 目录结构 (Repository Structure)

```text
messkill/
├── SKILL.md                          # [主路由技能] 智能分发与编排中枢 (mes-workflow-router)
├── README.md                         # 本文件
├── skills/                           # 专用子技能目录
│   ├── data-dictionary/             # [子技能] 数据字典生成与校验 (mes-data-dictionary)
│   │   ├── SKILL.md                 # 数据字典技能入口说明
│   │   ├── templates/               # 标准模板 (template.xlsx)
│   │   ├── scripts/                 # 自动化脚本 (generate_dict.py, verify_dict.py)
│   │   └── references/              # 规范参考 (type-mapping.md, naming-convention.md)
│   ├── management-client/           # [子技能] MES 管理端代码生成 (mes-management-client)
│   │   ├── SKILL.md                 # 管理端技能入口说明
│   │   ├── templates/               # C# WinForms 三层架构模板 (xxxW, xxxB, xxxW1, xxxB1)
│   │   └── references/              # 编码规范与 API 字典 (coding-standards.md, api-reference.md)
│   └── pda-client/                  # [子技能] MES PDA 移动端代码生成 (mes-pda-client)
│       ├── SKILL.md                 # PDA 移动端技能入口说明
│       ├── templates/               # HTML5/MUI/JS 及 C# WebService 模板
│       └── references/              # 扫码生命周期规范与 API 字典 (pda-workflow.md, api-reference.md)
└── tests/                           # 技能完整性与功能回归测试集
    └── test_skills.py
```

---

## 核心技能清单 (Skills Catalog)

### 1. 路由中枢：[`SKILL.md`](./SKILL.md)
- **技能名称**：`mes-workflow-router`
- **功能**：作为统一入口，根据开发者的自然语言、代码片段、SQL 语句或截图特征，精准路由到对应的数据字典、管理端或 PDA 子技能；并支持端到端全链路编排。

### 2. 数据字典技能：[`skills/data-dictionary`](./skills/data-dictionary/SKILL.md)
- **技能名称**：`mes-data-dictionary`
- **对标规范**：`LTA1024-静音棉计件表.xlsx` 标杆视觉与格式规约。
- **能力特性**：
  - 支持从 JSON 表定义或直接从 SQL Server `CREATE TABLE` DDL 语句自动化生成 Excel。
  - 严格映射企业统一类型体系（`I`、`S`、`D`、`N`）。
  - 内置 `verify_dict.py` 自动化合规检查，验证黄色表头、宋体字体、B列边框及列宽自适应。

### 3. 管理端代码生成技能：[`skills/management-client`](./skills/management-client/SKILL.md)
- **技能名称**：`mes-management-client`
- **对标规范**：`05-MES管理端` 经典 C# WinForms 三层架构。
- **能力特性**：
  - 视图类与业务类强解耦规范（`xxxW.cs` + `xxxB.cs`）。
  - 模态编辑对话框规范（`xxxW1.cs` + `xxxB1.cs`）。
  - 强制遵循企业框架（单例模式管理、`LSDataGrid` 数据驱动、`Config.DataBase` 数据访问、`TimeService` 统一服务时间）。

### 4. PDA 移动端代码生成技能：[`skills/pda-client`](./skills/pda-client/SKILL.md)
- **技能名称**：`mes-pda-client`
- **对标规范**：`03-MES-PDA` / `04-MES-PDA-NEW` 现场扫码作业系统。
- **能力特性**：
  - 移动手持端零手触设计：纯扫码驱动、回车按键拦截（兼容键码 13、0、229）。
  - 工业现场声光反馈：报警蜂鸣提示音、错误红框高亮。
  - 焦点自锁保持机制：避免扫码后光标丢失。
  - 后端 C# ASMX WebService RPC 标准化实现与事务保障。

---

## 快速上手与使用方式

### 场景 1：自动生成数据字典 Excel
```bash
# 从建表 SQL 生成数据字典
python skills/data-dictionary/scripts/generate_dict.py --sql table_ddl.sql --table-title "硫化追溯表" --output-dir d:\hlqiao\Desktop

# 校验生成的 Excel 是否符合企业标准
python skills/data-dictionary/scripts/verify_dict.py d:\hlqiao\Desktop\LTA*.xlsx
```

### 场景 2：让 AI 代理根据需求开发模块
在支持 Antigravity / AI 编程助手的环境中，只要将此技能库引入，AI 即可根据主路由自动识别调度。
例如：
> “*请参考数据字典 LTA1024，为半钢成型工序创建一个新的管理端窗体 CKA0035，并配套一个扫码记录的 PDA 界面与后台接口。*”

AI 将自动调用 `mes-workflow-router` 编排工作流，先依据 `mes-management-client` 生成 C# WinForms 代码，再依据 `mes-pda-client` 生成 HTML5/MUI 及 WebService 接口。

---

## 自动化测试 (Automated Testing)

运行测试套件，验证所有子技能的 frontmatter、关键模板、数据字典生成与校验器功能：

```bash
python tests/test_skills.py
```

---

## 未来新技能扩展 (Extensibility)

未来如需添加新的 MES 专用技能（如 `skills/bi-report` 报表看板、`skills/schedule-job` 定时调度等）：
1. 在 `skills/` 下新建子目录，包含 `SKILL.md`、`templates/`、`references/`。
2. 确保子技能 `SKILL.md` 包含标准的 YAML 前置元数据（`name` 和 `description`）。
3. 在根目录 `SKILL.md` 的意图路由表与索引中补充新技能规则。
