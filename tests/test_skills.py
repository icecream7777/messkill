#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
MES Skill Regression and Conformance Test Suite
Tests:
1. Frontmatter and structure of root SKILL.md and all sub-skills.
2. Template and reference file existence.
3. Functional test of generate_dict.py (JSON & SQL mode).
4. Functional test of verify_dict.py on generated Excel files.
"""

import os
import sys
import json
import re
import tempfile
import unittest
from pathlib import Path

# Paths
REPO_ROOT = Path(__file__).resolve().parent.parent
SKILLS_DIR = REPO_ROOT / "skills"


class TestMesSkills(unittest.TestCase):

    def _parse_yaml_frontmatter(self, skill_path: Path):
        content = skill_path.read_text(encoding="utf-8")
        match = re.match(r"^---\r?\n(.*?)\r?\n---\r?\n", content, re.DOTALL)
        self.assertIsNotNone(
            match, f"Skill file {skill_path} is missing YAML frontmatter ('---')!"
        )
        fm_text = match.group(1)
        name_match = re.search(r"^name:\s*(.+)$", fm_text, re.MULTILINE)
        self.assertIsNotNone(
            name_match, f"{skill_path} frontmatter missing 'name'"
        )
        name = name_match.group(1).strip()

        desc = ""
        # Match multi-line folded block scalar (>-, >, |) or single line
        desc_block_match = re.search(r"^description:\s*(?:[>|]-?)?\r?\n((?:\s+.+\r?\n?)+)", fm_text, re.MULTILINE)
        if desc_block_match:
            desc = " ".join(line.strip() for line in desc_block_match.group(1).splitlines() if line.strip())
        else:
            desc_single_match = re.search(r"^description:\s*(.+)$", fm_text, re.MULTILINE)
            if desc_single_match:
                desc = desc_single_match.group(1).strip()

        self.assertTrue(len(desc) > 10, f"{skill_path} description is too short or empty: '{desc}'")
        return name, desc

    def test_01_root_skill_router(self):
        root_skill = REPO_ROOT / "SKILL.md"
        self.assertTrue(root_skill.exists(), "Root SKILL.md must exist!")
        name, desc = self._parse_yaml_frontmatter(root_skill)
        self.assertEqual(name, "mes-workflow-router")
        self.assertTrue(len(desc) > 20)

    def test_02_sub_skills_structure(self):
        expected_sub_skills = [
            "data-dictionary",
            "management-client",
            "pda-client",
        ]
        for sub in expected_sub_skills:
            sub_dir = SKILLS_DIR / sub
            self.assertTrue(sub_dir.exists(), f"Sub-skill directory {sub} does not exist")
            
            # Check SKILL.md
            sub_skill = sub_dir / "SKILL.md"
            self.assertTrue(sub_skill.exists(), f"{sub}/SKILL.md does not exist")
            name, desc = self._parse_yaml_frontmatter(sub_skill)
            self.assertTrue(name.startswith("mes-"), f"Sub-skill name '{name}' should start with 'mes-'")
            self.assertTrue(len(desc) > 20)
            
            # Check templates
            tpl_dir = sub_dir / "templates"
            self.assertTrue(tpl_dir.exists(), f"{sub}/templates/ does not exist")
            templates = list(tpl_dir.glob("*"))
            self.assertTrue(len(templates) > 0, f"{sub}/templates/ is empty")
            
            # Check references
            ref_dir = sub_dir / "references"
            self.assertTrue(ref_dir.exists(), f"{sub}/references/ does not exist")
            references = list(ref_dir.glob("*.md"))
            self.assertTrue(len(references) > 0, f"{sub}/references/ has no markdown docs")

    def test_03_generate_and_verify_dict_json(self):
        # Import generate_dict and verify_dict
        scripts_dir = SKILLS_DIR / "data-dictionary" / "scripts"
        sys.path.insert(0, str(scripts_dir))
        
        try:
            import generate_dict
            import verify_dict
        except ImportError as e:
            self.fail(f"Failed to import data dictionary scripts: {e}")

        with tempfile.TemporaryDirectory() as tmpdir:
            test_table = {
                "code": "TEST1001",
                "title": "测试自动化追溯表",
                "fields": [
                    ["ID", None, "I", None, None, "主键ID自增"],
                    ["FAC", "工厂代码", "S", 10, None, None],
                    ["WEIGHT", "物料重量", "N", 9, 3, "保留3位小数"],
                    ["CRTIM", "创建时间", "D", None, None, None]
                ]
            }
            json_file = Path(tmpdir) / "test_table.json"
            json_file.write_text(json.dumps([test_table], ensure_ascii=False), encoding="utf-8")

            # Run generate
            out_files = generate_dict.process_json(str(json_file), tmpdir)
            self.assertEqual(len(out_files), 1)
            generated_excel = Path(out_files[0])
            self.assertTrue(generated_excel.exists())
            self.assertEqual(generated_excel.name, "TEST1001-测试自动化追溯表.xlsx")

            # Run verify
            report = verify_dict.verify_excel_file(str(generated_excel))
            self.assertTrue(
                report["compliant"],
                f"Generated Excel failed verification: errors={report['errors']}, warnings={report['warnings']}"
            )

    def test_04_generate_and_verify_dict_sql(self):
        scripts_dir = SKILLS_DIR / "data-dictionary" / "scripts"
        sys.path.insert(0, str(scripts_dir))
        import generate_dict
        import verify_dict

        with tempfile.TemporaryDirectory() as tmpdir:
            sql_content = """
            CREATE TABLE [dbo].[TEST1002](
                [ID] [int] IDENTITY(1,1) NOT NULL,
                [BARCODE] [varchar](32) NULL,
                [QTY] [decimal](10, 2) NULL,
                [SCANDATE] [datetime] NULL
            )
            """
            sql_file = Path(tmpdir) / "test_table.sql"
            sql_file.write_text(sql_content, encoding="utf-8")

            out_files = generate_dict.process_sql(str(sql_file), "SQL测试生成表", tmpdir)
            self.assertEqual(len(out_files), 1)
            generated_excel = Path(out_files[0])
            self.assertTrue(generated_excel.exists())
            self.assertEqual(generated_excel.name, "TEST1002-SQL测试生成表.xlsx")

            report = verify_dict.verify_excel_file(str(generated_excel))
            self.assertTrue(
                report["compliant"],
                f"SQL-generated Excel failed verification: errors={report['errors']}, warnings={report['warnings']}"
            )


if __name__ == "__main__":
    unittest.main()
