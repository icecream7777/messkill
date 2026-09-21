# -*- coding: utf-8 -*-
"""
MES 数据字典合规校验脚本 (verify_dict.py)
检查指定 Excel 数据字典文件是否符合浦林成山 MES 标准规范。
"""

import os
import sys
import argparse
import openpyxl

EXPECTED_HEADERS = ["列名", "描述", "类型", "长度", "小数位", "备注"]
VALID_TYPES = {"I", "S", "D", "N", "U"}

def verify_file(filepath):
    issues = []
    if not os.path.exists(filepath):
        return False, [f"文件不存在: {filepath}"]

    try:
        wb = openpyxl.load_workbook(filepath)
    except Exception as e:
        return False, [f"无法读取 Excel 文件: {e}"]

    ws = wb["Sheet1"] if "Sheet1" in wb.sheetnames else wb.active

    # 1. 检查第 1 行：表名与表中文名
    tbl_code = ws.cell(1, 1).value
    tbl_name = ws.cell(1, 2).value
    if not tbl_code:
        issues.append("第1行第1列(A1)缺少表名编码（例如 LTA1024）")
    if not tbl_name:
        issues.append("第1行第2列(B1)缺少数据表中文名称")

    # 2. 检查第 2 行：表头
    actual_headers = [ws.cell(2, c).value for c in range(1, 7)]
    for idx, expected in enumerate(EXPECTED_HEADERS):
        actual = actual_headers[idx]
        if actual != expected:
            issues.append(f"第2行第{idx+1}列表头不匹配: 期望 '{expected}', 实际 '{actual}'")

    # 3. 检查第 3 行及之后的数据字段
    data_rows_found = 0
    for r in range(3, ws.max_row + 1):
        col_name = ws.cell(r, 1).value
        col_desc = ws.cell(r, 2).value
        col_type = ws.cell(r, 3).value
        col_len = ws.cell(r, 4).value
        col_scale = ws.cell(r, 5).value

        if not col_name and not col_type:
            continue

        data_rows_found += 1

        # 检查类型是否在合法集合中
        if col_type not in VALID_TYPES:
            issues.append(f"第{r}行字段 [{col_name}] 类型 '{col_type}' 非标准 MES 类型 (应为 I/S/D/N)")

        # 检查数值类型小数位与长度
        if col_type == "N":
            if col_len is None:
                issues.append(f"第{r}行数值型字段 [{col_name}] 未指定长度 (建议如 9, 10, 18)")
            if col_scale is None:
                issues.append(f"第{r}行数值型字段 [{col_name}] 未指定小数位 (建议如 2, 3)")

        # 检查字符串类型是否有长度
        if col_type == "S" and col_len is None:
            issues.append(f"第{r}行字符型字段 [{col_name}] 未指定长度")

    if data_rows_found == 0:
        issues.append("未检索到任何数据字段定义（第3行及之后全为空）")

    is_valid = len(issues) == 0
    return is_valid, issues

def verify_excel_file(filepath):
    """
    程序化调用校验，返回字典结构
    """
    valid, issues = verify_file(filepath)
    return {
        "compliant": valid,
        "errors": issues,
        "warnings": []
    }

def main():
    parser = argparse.ArgumentParser(description="MES 数据字典 Excel 合规校验工具")
    parser.add_argument("files", nargs="+", help="待校验的 Excel 文件路径")
    args = parser.parse_args()

    total = len(args.files)
    passed = 0

    for f in args.files:
        valid, issues = verify_file(f)
        status_str = "[PASS]" if valid else "[FAIL]"
        print(f"{status_str} {os.path.basename(f)}")
        if valid:
            passed += 1
        else:
            for issue in issues:
                print(f"   * {issue}")

    print(f"\n校验统计: 共 {total} 个文件, 通过 {passed}, 不合规 {total - passed}")
    sys.exit(0 if passed == total else 1)

if __name__ == "__main__":
    main()
