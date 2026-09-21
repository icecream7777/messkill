---
name: mes-data-dictionary
description: >-
  Automatically generate, update, and validate standard Prinx Chengshan MES Excel data dictionary
  tables (数据字典表) following the LTA1024 standard format. Use when creating data dictionaries from
  SQL Server tables, Navicat/SSMS screenshots, SQL DDL, or business requirements.
---

# MES 数据字典生成与维护技能 (MES Data Dictionary Skill)

本技能用于为浦林成山 MES 系统的物理数据表生成符合企业标准的数据字典 Excel 文件（完全依循 `LTA1024-静音棉计件表.xlsx` 标杆规范）。

---

## 快速工作流 (Step-by-Step Workflow)

```mermaid
flowchart LR
    A["输入: SQL DDL / 截图 / 需求"] --> B["字段类型映射 (I/S/D/N)"]
    B --> C["调用 generate_dict.py 生成"]
    C --> D["调用 verify_dict.py 校验"]
    D --> E["输出: 交付桌面并归档至 06-数据字典"]
```

### 步骤 1：梳理表结构定义
从用户提供的 SQL 语句、Navicat 表设计截图或业务字段清单中，提炼出以下核心要素：
1. **表编码（Table Code）**：大写英数字编码（如 `LTA1019`、`EDA0004`）。
2. **表中文名（Table Title）**：清晰准确的业务表名（如 `返回胶班次库存统计表`）。
3. **字段明细**：
   - 列名（如 `FAC`、`ITNBR`、`CZQTY`）
   - 中文描述（如 `工厂`、`物料编码`、`称重数量`）
   - 数据类型代码（映射为 `I`、`S`、`D`、`N`）
   - 长度（`S` 填字符数；`N` 填总精度；`I`/`D` 为空）
   - 小数位（`N` 填小数位数；其他类型为空）
   - 备注（选填，如主外键关联、状态枚举等）

### 步骤 2：类型转换映射规则
依据 [type-mapping.md](./references/type-mapping.md) 规约进行映射：
- `int` / `bigint` / `bit` $\to$ **`I`**（无长度和小数位）
- `nvarchar(N)` / `varchar(N)` $\to$ **`S`**（长度填 $N$，无小数位）
- `datetime` / `date` $\to$ **`D`**（无长度和小数位）
- `decimal(P, S)` / `numeric(P, S)` $\to$ **`N`**（长度填 $P$，小数位填 $S$）

### 步骤 3：调用自动化生成脚本
在当前技能的 `scripts/` 目录下提供了现成的自动化生成器：

#### 方式 A：通过 JSON 文件批量生成
```bash
python skills/data-dictionary/scripts/generate_dict.py --json table_defs.json --output-dir d:\hlqiao\Desktop
```
JSON 结构示例：
```json
[
  {
    "code": "LTA1019",
    "title": "返回胶班次库存统计表",
    "fields": [
      ["ID", null, "I", null, null, null],
      ["FAC", "工厂", "S", 10, null, null],
      ["CZQTY", "称重数量", "N", 9, 3, null],
      ["CRTIM", "创建时间", "D", null, null, null]
    ]
  }
]
```

#### 方式 B：从 SQL Server `CREATE TABLE` 语句直接生成
```bash
python skills/data-dictionary/scripts/generate_dict.py --sql create_table.sql --table-title "某某业务表" --output-dir d:\hlqiao\Desktop
```

### 步骤 4：自动化校验
生成完成后，必须执行校验脚本，确保样式与格式 100% 达标：
```bash
python skills/data-dictionary/scripts/verify_dict.py d:\hlqiao\Desktop\*.xlsx
```

### 步骤 5：文件归档
1. 保存至用户交付目录（如桌面）。
2. 同时复制到版本控制的数据字典归档目录：
   `d:\git\mes-major\06-数据字典\{模块大类}\{子分类}\`

---

## 格式基准与视觉规范
- **基准模板文件**：[`templates/template.xlsx`](./templates/template.xlsx)
- **字体**：全部统一为 **宋体（SimSun）**，表头 12pt 加粗，表名 11pt，数据行 11pt（描述列 10pt）。
- **标头填充**：第 2 行标头填充 Excel 默认黄色（Indexed Color 4 / `#FFFF00`）。
- **边框**：列 B（描述列）强制添加细实线全包围边框（Thin Border）。
- **文件命名**：`{表名代码}-{中文名称}.xlsx`
