# -*- coding: utf-8 -*-
"""
MES 数据字典自动生成脚本 (generate_dict.py)
用于将表结构定义（JSON/SQL/参数）转换为符合浦林成山 MES 规范的标准 Excel 数据字典文件。
"""

import os
import sys
import json
import re
import argparse
import openpyxl
from openpyxl.styles import Font, Alignment, Border, Side

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
DEFAULT_TEMPLATE = os.path.join(SCRIPT_DIR, "..", "templates", "template.xlsx")

# 标准样式常量
FONT_TITLE = Font(name="宋体", size=11, bold=False)
FONT_HEADER = Font(name="宋体", size=12, bold=True)
FONT_DATA_DEFAULT = Font(name="宋体", size=11, bold=False)
FONT_DATA_DESC = Font(name="宋体", size=10, bold=False)

ALIGN_CENTER_V = Alignment(vertical="center")
ALIGN_DESC = Alignment(horizontal="left", vertical="center")

THIN_SIDE = Side(style="thin")
BORDER_THIN = Border(left=THIN_SIDE, right=THIN_SIDE, top=THIN_SIDE, bottom=THIN_SIDE)
BORDER_EMPTY = Border()

# 类型转换映射表：SQL Server 类型 -> MES 数据字典类型代码
TYPE_MAPPING = {
    "int": "I",
    "bigint": "I",
    "smallint": "I",
    "tinyint": "I",
    "bit": "I",
    "nvarchar": "S",
    "varchar": "S",
    "nchar": "S",
    "char": "S",
    "text": "S",
    "ntext": "S",
    "datetime": "D",
    "date": "D",
    "time": "D",
    "timestamp": "D",
    "datetime2": "D",
    "decimal": "N",
    "numeric": "N",
    "float": "N",
    "real": "N",
    "money": "N"
}

def parse_sql_create_table(sql_text):
    """
    解析 SQL Server CREATE TABLE 语句，提取表名与字段定义
    """
    table_match = re.search(r"CREATE\s+TABLE\s+(?:\[?dbo\]?\.)?\[?([A-Za-z0-9_]+)\]?", sql_text, re.IGNORECASE)
    table_name = table_match.group(1) if table_match else "UNKNOWN_TABLE"
    
    fields = []
    # 提取括号内字段行
    body_match = re.search(r"\((.*)\)", sql_text, re.DOTALL)
    if not body_match:
        return table_name, fields

    lines = body_match.group(1).split("\n")
    for line in lines:
        line = line.strip().rstrip(",")
        if not line or line.upper().startswith("CONSTRAINT") or line.upper().startswith("PRIMARY KEY"):
            continue
        
        # 匹配: [COL] [type](len, scale) [NULL] [COMMENT]
        col_match = re.match(r"\[?([A-Za-z0-9_]+)\]?\s+\[?([A-Za-z0-9]+)\]?(?:\s*\(\s*([0-9]+)(?:\s*,\s*([0-9]+))?\s*\))?", line)
        if col_match:
            col_name = col_match.group(1)
            raw_type = col_match.group(2).lower()
            length = int(col_match.group(3)) if col_match.group(3) else None
            scale = int(col_match.group(4)) if col_match.group(4) else None

            dict_type = TYPE_MAPPING.get(raw_type, "S")
            if dict_type in ("I", "D"):
                length = None
                scale = None

            # 提取行末注释（如 -- 注释 或 /* 注释 */）
            comment_match = re.search(r"(?:--|/\*)\s*(.*?)(?:\*/)?$", line)
            desc = comment_match.group(1).strip() if comment_match else None

            fields.append({
                "name": col_name,
                "desc": desc,
                "type": dict_type,
                "length": length,
                "scale": scale,
                "remark": None
            })
    return table_name, fields

def create_data_dictionary(table_code, table_title, fields, output_path, template_file=None):
    """
    生成单张表的数据字典 Excel 文件
    :param table_code: 表名代码（如 LTA1019）
    :param table_title: 表中文描述（如 返回胶班次库存统计表）
    :param fields: 字段列表，每项为字典或元组 (name, desc, type, length, scale, remark)
    :param output_path: 输出 Excel 文件绝对路径
    :param template_file: 模板文件路径（默认使用内置标准模板）
    """
    if template_file is None or not os.path.exists(template_file):
        template_file = DEFAULT_TEMPLATE

    wb = openpyxl.load_workbook(template_file)
    ws = wb["Sheet1"] if "Sheet1" in wb.sheetnames else wb.active

    # Row 1: 表名与表中文描述
    ws.cell(1, 1).value = table_code
    ws.cell(1, 2).value = table_title

    start_row = 3
    orig_max_row = ws.max_row
    num_fields = len(fields)

    for idx, f in enumerate(fields):
        curr_row = start_row + idx

        if isinstance(f, (list, tuple)):
            col_name = f[0] if len(f) > 0 else None
            col_desc = f[1] if len(f) > 1 else None
            col_type = f[2] if len(f) > 2 else "S"
            col_len  = f[3] if len(f) > 3 else None
            col_scale= f[4] if len(f) > 4 else None
            col_rem  = f[5] if len(f) > 5 else None
        else:
            col_name = f.get("name")
            col_desc = f.get("desc")
            col_type = f.get("type", "S")
            col_len  = f.get("length")
            col_scale= f.get("scale")
            col_rem  = f.get("remark")

        # Col 1: 列名
        c1 = ws.cell(curr_row, 1)
        c1.value = col_name
        c1.font = FONT_DATA_DEFAULT
        c1.alignment = ALIGN_CENTER_V

        # Col 2: 描述 (细实线全包边框，左对齐居中)
        c2 = ws.cell(curr_row, 2)
        c2.value = col_desc
        c2.font = FONT_DATA_DESC
        c2.border = BORDER_THIN
        c2.alignment = ALIGN_DESC

        # Col 3: 类型 (I/S/D/N 等)
        c3 = ws.cell(curr_row, 3)
        c3.value = col_type
        c3.font = FONT_DATA_DEFAULT
        c3.alignment = ALIGN_CENTER_V

        # Col 4: 长度
        c4 = ws.cell(curr_row, 4)
        c4.value = col_len
        c4.font = FONT_DATA_DEFAULT
        c4.alignment = ALIGN_CENTER_V

        # Col 5: 小数位
        c5 = ws.cell(curr_row, 5)
        c5.value = col_scale
        c5.font = FONT_DATA_DEFAULT
        c5.alignment = ALIGN_CENTER_V

        # Col 6: 备注
        c6 = ws.cell(curr_row, 6)
        c6.value = col_rem
        c6.font = FONT_DATA_DEFAULT
        c6.alignment = ALIGN_CENTER_V

    # 清除模板中超出当前字段数的行
    if start_row + num_fields <= orig_max_row:
        for r in range(start_row + num_fields, orig_max_row + 1):
            for c in range(1, 7):
                cell = ws.cell(r, c)
                cell.value = None
                cell.border = BORDER_EMPTY

    os.makedirs(os.path.dirname(os.path.abspath(output_path)), exist_ok=True)
    wb.save(output_path)
    print(f"[SUCCESS] 数据字典生成完成: {output_path}")
    return output_path

def process_json(json_path, output_dir=".", template_file=None):
    """
    从 JSON 文件批量生成数据字典 Excel 文件
    """
    with open(json_path, "r", encoding="utf-8") as f:
        data = json.load(f)
    tables = data if isinstance(data, list) else [data]
    generated_files = []
    for tbl in tables:
        code = tbl.get("code") or tbl.get("table_code")
        title = tbl.get("title") or tbl.get("table_title", "")
        fields = tbl.get("fields", [])
        filename = f"{code}-{title}.xlsx" if title else f"{code}.xlsx"
        out_file = os.path.join(output_dir, filename)
        create_data_dictionary(code, title, fields, out_file, template_file)
        generated_files.append(out_file)
    return generated_files

def process_sql(sql_path, table_title=None, output_dir=".", template_file=None):
    """
    从包含 CREATE TABLE 的 SQL 文件生成数据字典 Excel 文件
    """
    with open(sql_path, "r", encoding="utf-8") as f:
        sql_content = f.read()
    code, fields = parse_sql_create_table(sql_content)
    title = table_title or code
    out_file = os.path.join(output_dir, f"{code}-{title}.xlsx")
    create_data_dictionary(code, title, fields, out_file, template_file)
    return [out_file]

def main():
    parser = argparse.ArgumentParser(description="MES 数据字典 Excel 文件生成工具")
    parser.add_argument("--json", help="包含表结构定义的 JSON 文件路径")
    parser.add_argument("--sql", help="包含 CREATE TABLE 语句的 SQL 文件路径")
    parser.add_argument("--table-code", help="数据表编码，例如 LTA1019")
    parser.add_argument("--table-title", help="数据表中文名，例如 返回胶班次库存统计表")
    parser.add_argument("--output-dir", default=".", help="输出目录路径")
    parser.add_argument("--template", help="基础 Excel 模板文件路径")
    args = parser.parse_args()

    if args.json:
        process_json(args.json, args.output_dir, args.template)
    elif args.sql:
        process_sql(args.sql, args.table_title, args.output_dir, args.template)
    elif args.table_code:
        code = args.table_code
        title = args.table_title or code
        out_file = os.path.join(args.output_dir, f"{code}-{title}.xlsx")
        create_data_dictionary(code, title, [], out_file, args.template)
    else:
        parser.print_help()

if __name__ == "__main__":
    main()
