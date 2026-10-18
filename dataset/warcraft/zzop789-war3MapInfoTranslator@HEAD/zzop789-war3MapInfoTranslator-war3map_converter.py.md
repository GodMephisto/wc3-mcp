---
type: vendor-doc
source_repo: "https://github.com/zzop789/war3MapInfoTranslator"
source_path: "war3map_converter.py"
ref: HEAD
---

#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
War3Map数据格式转换核心模块 - 简单工作版本
支持TXT(Lua格式) <-> CSV 双向转换
"""

import re
import csv
import os
from typing import Dict, Any, List, Tuple, Optional
from pathlib import Path

class War3MapConverter:
    """War3Map数据格式转换器核心类"""
    
    def __init__(self):
        self.version = 2  # 默认版本号
        self.origin_data = {}
        self.custom_data = {}
        
    def txt_to_csv(self, txt_file: str, output_dir: str = None) -> Dict[str, str]:
        """
        将TXT文件转换为CSV文件
        
        Args:
            txt_file: 输入的TXT文件路径
            output_dir: 输出目录，默认为TXT文件同目录
            
        Returns:
            包含生成文件路径的字典
        """
        try:
            # 解析TXT文件
            self._parse_txt_file(txt_file)
            
            # 确定输出目录
            if output_dir is None:
                output_dir = os.path.dirname(txt_file)
            os.makedirs(output_dir, exist_ok=True)
            
            # 生成文件路径
            base_name = Path(txt_file).stem
            origin_csv = os.path.join(output_dir, f"{base_name}_origin.csv")
            custom_csv = os.path.join(output_dir, f"{base_name}_custom.csv")
            
            result = {}
            
            # 生成CSV文件
            if self.origin_data:
                self._write_csv(self.origin_data, origin_csv, "ORIGIN")
                result['origin'] = origin_csv
                
            if self.custom_data:
                self._write_csv(self.custom_data, custom_csv, "CUSTOM")
                result['custom'] = custom_csv
                
            return result
            
        except Exception as e:
            raise Exception(f"TXT转CSV失败: {str(e)}")
    
    def csv_to_txt(self, csv_files: List[str], output_file: str) -> str:
        """
        将CSV文件转换为TXT文件
        
        Args:
            csv_files: CSV文件路径列表 [origin_csv, custom_csv]
            output_file: 输出的TXT文件路径
            
        Returns:
            生成的TXT文件路径
        """
        try:
            origin_data = {}
            custom_data = {}
            
            # 读取CSV文件
            for csv_file in csv_files:
                if not os.path.exists(csv_file):
                    continue
                    
                file_name = os.path.basename(csv_file).lower()
                if 'origin' in file_name:
                    origin_data = self._read_csv(csv_file)
                elif 'custom' in file_name:
                    custom_data = self._read_csv(csv_file)
            
            # 生成TXT文件
            self._write_txt_file(output_file, origin_data, custom_data)
            
            return output_file
            
        except Exception as e:
            raise Exception(f"CSV转TXT失败: {str(e)}")
    
    def _parse_txt_file(self, txt_file: str) -> None:
        """解析TXT文件内容"""
        print(f"正在解析文件: {txt_file}")
        
        # 读取文件内容
        content = self._read_file_with_encoding(txt_file)
        
        # 提取VERSION
        version_match = re.search(r'VERSION\s*=\s*(\d+)', content)
        if version_match:
            self.version = int(version_match.group(1))
            print(f"发现版本号: {self.version}")
        
        # 提取整个文件的主结构：return { ... }
        main_structure = re.search(r'return\s*\{(.*)\}', content, re.DOTALL)
        if not main_structure:
            print("警告：未找到标准的return结构，尝试直接解析...")
            main_content = content
        else:
            main_content = main_structure.group(1)
        
        # 提取ORIGIN和CUSTOM部分
        origin_content = self._extract_section(main_content, "ORIGIN")
        custom_content = self._extract_section(main_content, "CUSTOM")
        
        if origin_content:
            print(f"找到ORIGIN部分，长度: {len(origin_content)}")
            self.origin_data = self._parse_data_section(origin_content)
            print(f"ORIGIN数据解析完成，条目数: {len(self.origin_data)}")
        
        if custom_content:
            print(f"找到CUSTOM部分，长度: {len(custom_content)}")
            self.custom_data = self._parse_data_section(custom_content)
            print(f"CUSTOM数据解析完成，条目数: {len(self.custom_data)}")
    
    def _extract_section(self, content: str, section_name: str) -> str:
        """提取指定段落的内容"""
        # 找到段落开始位置
        start_pattern = rf'{section_name}\s*=\s*\{{'
        start_match = re.search(start_pattern, content)
        if not start_match:
            return ""
        
        start_pos = start_match.end() - 1  # 指向开始的{
        
        # 找到匹配的结束大括号
        brace_count = 0
        pos = start_pos
        while pos < len(content):
            if content[pos] == '{':
                brace_count += 1
            elif content[pos] == '}':
                brace_count -= 1
                if brace_count == 0:
                    # 提取内容（去掉外层大括号）
                    return content[start_pos + 1:pos]
            pos += 1
        return ""
    
    def _parse_data_section(self, content: str) -> Dict[str, Dict[str, str]]:
        """解析数据段"""
        data = {}
        
        # 改进的条目匹配：使用递归匹配嵌套大括号
        def find_matching_brace(text, start_pos):
            """找到匹配的大括号位置"""
            brace_count = 0
            pos = start_pos
            while pos < len(text):
                if text[pos] == '{':
                    brace_count += 1
                elif text[pos] == '}':
                    brace_count -= 1
                    if brace_count == 0:
                        return pos
                pos += 1
            return -1
        
        # 手动解析条目
        pos = 0
        entry_count = 0
        
        while pos < len(content):
            # 查找条目ID模式：ID名称 = { (支持前导空白字符)
            id_match = re.search(r'^\s*([A-Za-z0-9_]+)\s*=\s*\{', content[pos:], re.MULTILINE)
            if not id_match:
                break
                
            entry_id = id_match.group(1)
            start_brace_pos = pos + id_match.end() - 1  # 指向开始的{
            
            # 找到匹配的结束大括号
            end_brace_pos = find_matching_brace(content, start_brace_pos)
            if end_brace_pos == -1:
                print(f"  警告：条目 {entry_id} 的大括号不匹配")
                pos += id_match.end()
                continue
            
            # 提取条目内容（去掉外层大括号）
            entry_content = content[start_brace_pos + 1:end_brace_pos]
            
            # 解析字段
            fields = self._parse_entry_fields(entry_content)
            data[entry_id] = fields
            entry_count += 1
            
            # 移动到下一个位置
            pos = end_brace_pos + 1
            
            # 每处理1000个条目打印一次进度
            if entry_count % 1000 == 0:
                print(f"    已处理 {entry_count} 个条目")
        
        print(f"  完成解析，共 {len(data)} 个条目")
        return data
    
    def _parse_entry_fields(self, content: str) -> Dict[str, str]:
        """更严谨地解析字段 - 修复字段识别问题"""
        fields = {}
        pos = 0
        length = len(content)

        while pos < length:
            # 跳过空白字符
            while pos < length and content[pos] in [' ', '\t', '\n', '\r']:
                pos += 1
            
            if pos >= length:
                break
            
            # 匹配字段名：只有在行首或大括号后的字母开头才是字段名
            field_match = re.match(r'([a-zA-Z][a-zA-Z0-9_]*)\s*=\s*', content[pos:])
            if not field_match:
                pos += 1
                continue

            field_name = field_match.group(1)
            pos += field_match.end()

            # 判断值类型并提取完整的字段值
            if pos >= length:
                break

            if content[pos] == '"':
                # 字符串值
                end_pos = pos + 1
                while end_pos < length and content[end_pos] != '"':
                    if content[end_pos] == '\\':  # 跳过转义字符
                        end_pos += 1
                    end_pos += 1
                if end_pos < length:
                    end_pos += 1  # 包含右引号
                field_value = content[pos:end_pos]
                pos = end_pos
            elif content[pos] == '{':
                # 嵌套表结构 - 找到完整的大括号匹配
                brace_count = 1
                end_pos = pos + 1
                while end_pos < length and brace_count > 0:
                    if content[end_pos] == '"':
                        # 跳过字符串内容，避免字符串内的大括号干扰
                        end_pos += 1
                        while end_pos < length and content[end_pos] != '"':
                            if content[end_pos] == '\\':  # 跳过转义字符
                                end_pos += 1
                            end_pos += 1
                        if end_pos < length:
                            end_pos += 1  # 跳过结束引号
                    elif content[end_pos] == '{':
                        brace_count += 1
                        end_pos += 1
                    elif content[end_pos] == '}':
                        brace_count -= 1
                        end_pos += 1
                    else:
                        end_pos += 1
                field_value = content[pos:end_pos]
                pos = end_pos
            else:
                # 普通标识符或数字 - 直到逗号、换行或下一个字段
                end_pos = pos
                while end_pos < length:
                    char = content[end_pos]
                    if char == ',':
                        break
                    elif char == '\n':
                        # 检查下一行是否是新字段（字母开头后跟=）
                        next_line_start = end_pos + 1
                        while next_line_start < length and content[next_line_start] in [' ', '\t']:
                            next_line_start += 1
                        if next_line_start < length and re.match(r'[a-zA-Z][a-zA-Z0-9_]*\s*=', content[next_line_start:]):
                            break
                        end_pos += 1
                    else:
                        end_pos += 1
                
                field_value = content[pos:end_pos].strip()
                pos = end_pos

            # 跳过结尾的逗号和空白
            while pos < length and content[pos] in [',', '\n', '\r', ' ', '\t']:
                pos += 1

            # 处理重复字段（使用分隔符合并）
            if field_name in fields:
                fields[field_name] = fields[field_name] + "おなに" + field_value
            else:
                fields[field_name] = field_value

        return fields

    def _read_file_with_encoding(self, file_path: str) -> str:
        """使用多种编码尝试读取文件"""
        encodings = ['utf-8', 'gbk', 'utf-8-sig', 'latin1']
        
        for encoding in encodings:
            try:
                with open(file_path, 'r', encoding=encoding) as file:
                    return file.read()
            except UnicodeDecodeError:
                continue
        
        # 最后尝试忽略错误
        with open(file_path, 'r', encoding='utf-8', errors='ignore') as file:
            return file.read()
    
    def _write_csv(self, data: Dict[str, Dict[str, str]], filename: str, data_type: str) -> None:
        """写入CSV文件"""
        # 收集所有唯一字段
        all_fields = set()
        for entry_data in data.values():
            all_fields.update(entry_data.keys())
        
        # 构建表头：ID, Suffix + 所有字段
        headers = ['ID', 'Suffix'] + sorted(list(all_fields))
        
        print(f"写入{data_type} CSV文件: {filename}")
        print(f"  数据条目数: {len(data)}")
        print(f"  表头字段数: {len(all_fields)}")
        
        with open(filename, 'w', newline='', encoding='utf-8-sig') as csvfile:
            writer = csv.writer(csvfile)
            
            # 版本号和表头
            version_row = [f"Version: {self.version}"] + [''] * (len(headers) - 1)
            writer.writerow(version_row)
            writer.writerow(headers)
            
            # 数据行
            for full_id, fields in data.items():
                main_id, suffix = self._split_id(full_id)
                
                row = [main_id, suffix]
                
                # 按表头顺序填充字段值
                for field_name in headers[2:]:  # 跳过ID和Suffix
                    value = fields.get(field_name, '')
                    cleaned_value = self._clean_value_for_csv(value)
                    row.append(cleaned_value)
                
                writer.writerow(row)
    
    def _clean_value_for_csv(self, value: str) -> str:
        """清理字段值，移除可能破坏CSV结构的字符"""
        if not value:
            return ''
        
        # 移除换行符、制表符，压缩多余空格
        cleaned = value.replace('\n', ' ').replace('\r', ' ').replace('\t', ' ')
        # 压缩多个连续空格为单个空格
        cleaned = ' '.join(cleaned.split())
        
        return cleaned
    
    def _read_csv(self, csv_file: str) -> Dict[str, Dict[str, str]]:
        """读取CSV文件"""
        data = {}
        
        with open(csv_file, 'r', encoding='utf-8-sig') as csvfile:
            reader = csv.reader(csvfile)
            
            # 跳过版本号行
            version_row = next(reader)
            if version_row and version_row[0].startswith('Version:'):
                version_str = version_row[0].replace('Version:', '').strip()
                try:
                    self.version = int(version_str)
                except ValueError:
                    pass
            
            # 读取表头
            headers = next(reader)
            
            # 读取数据
            for row in reader:
                if len(row) >= 2 and row[0].strip():
                    main_id = row[0].strip()
                    suffix = row[1].strip() if len(row) > 1 else ''
                    
                    # 重构完整ID
                    full_id = f"{main_id}_{suffix}" if suffix else main_id
                    
                    # 提取字段
                    fields = {}
                    for i, header in enumerate(headers[2:], 2):  # 跳过ID和Suffix
                        if i < len(row) and row[i].strip():
                            fields[header] = row[i].strip()
                    
                    if fields:  # 只有在有字段数据时才添加
                        data[full_id] = fields
        
        return data
    
    def _write_txt_file(self, output_file: str, origin_data: Dict, custom_data: Dict) -> None:
        """写入TXT文件"""
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write("return\n{\n")
            f.write(f"\tVERSION={self.version},\n")
            
            # 写入ORIGIN数据
            f.write("\tORIGIN={\n")
            for entry_id, fields in origin_data.items():
                f.write(f"\t\t{entry_id}={{\n")
                
                # 处理字段
                for field_name, field_value in fields.items():
                    # 检查是否包含重复字段分隔符 "おなに"
                    if "おなに" in field_value:
                        # 分割重复字段的多个值
                        values = field_value.split("おなに")
                        for value in values:
                            formatted_value = self._format_field_value(value)
                            f.write(f'\t\t\t{field_name}={formatted_value},\n')
                    else:
                        # 单个字段值
                        formatted_value = self._format_field_value(field_value)
                        f.write(f'\t\t\t{field_name}={formatted_value},\n')
                        
                f.write("\t\t},\n")
            f.write("\t},\n")
            
            # 写入CUSTOM数据
            f.write("\tCUSTOM={\n")
            for entry_id, fields in custom_data.items():
                f.write(f"\t\t{entry_id}={{\n")
                
                # 处理字段
                for field_name, field_value in fields.items():
                    # 检查是否包含重复字段分隔符 "おなに"
                    if "おなに" in field_value:
                        # 分割重复字段的多个值
                        values = field_value.split("おなに")
                        for value in values:
                            formatted_value = self._format_field_value(value)
                            f.write(f'\t\t\t{field_name}={formatted_value},\n')
                    else:
                        # 单个字段值
                        formatted_value = self._format_field_value(field_value)
                        f.write(f'\t\t\t{field_name}={formatted_value},\n')
                        
                f.write("\t\t},\n")
            f.write("\t},\n")
            
            f.write("}\n")
    
    def _format_field_value(self, field_value: str) -> str:
        """格式化字段值"""
        if not field_value:
            return '""'
        
        # 如果是嵌套表格式，直接返回（不添加外层引号）
        if field_value.strip().startswith('{') and field_value.strip().endswith('}'):
            return field_value.strip()
        
        # 判断是否为数字
        try:
            float(field_value)
            return field_value
        except ValueError:
            # 字符串值：保持原有的引号结构
            # 如果字段值已经包含引号，直接返回；否则添加引号
            if (field_value.startswith('"') and field_value.endswith('"')) or \
               (field_value.startswith("'") and field_value.endswith("'")):
                return field_value
            else:
                return f'"{field_value}"'
    
    def _split_id(self, full_id: str) -> Tuple[str, str]:
        """分离ID和后缀"""
        if '_' in full_id:
            parts = full_id.split('_', 1)
            return parts[0], parts[1]
        else:
            return full_id, ''
    
    def get_field_info(self) -> Dict[str, set]:
        """获取字段信息"""
        origin_fields = set()
        custom_fields = set()
        
        # 收集ORIGIN字段
        for entry_data in self.origin_data.values():
            origin_fields.update(entry_data.keys())
            
        # 收集CUSTOM字段
        for entry_data in self.custom_data.values():
            custom_fields.update(entry_data.keys())
            
        all_fields = origin_fields | custom_fields
        
        return {
            'origin_fields': origin_fields,
            'custom_fields': custom_fields,
            'all_fields': all_fields
        }
    
    def get_statistics(self) -> Dict[str, Any]:
        """获取数据统计信息"""
        return {
            'version': self.version,
            'origin_count': len(self.origin_data),
            'custom_count': len(self.custom_data),
            'total_count': len(self.origin_data) + len(self.custom_data)
        }

# 便捷函数
def convert_txt_to_csv(txt_file: str, output_dir: str = None) -> Dict[str, str]:
    """便捷函数：TXT转CSV"""
    converter = War3MapConverter()
    return converter.txt_to_csv(txt_file, output_dir)

def convert_csv_to_txt(csv_files: List[str], output_file: str) -> str:
    """便捷函数：CSV转TXT"""
    converter = War3MapConverter()
    return converter.csv_to_txt(csv_files, output_file)

def merge_txt_files(origin_file: str, custom_file: str, output_file: str) -> str:
    """便捷函数：合并origin和custom TXT文件"""
    # 简单实现：这里可以添加合并逻辑
    return output_file

def auto_merge_txt_pairs(input_dir: str, output_dir: str) -> List[str]:
    """便捷函数：自动合并配对的TXT文件"""
    # 简单实现：这里可以添加自动合并逻辑
    return []

if __name__ == "__main__":
    # 测试代码
    print("War3Map转换器核心模块 - 简单工作版本")
    print("支持TXT <-> CSV双向转换")