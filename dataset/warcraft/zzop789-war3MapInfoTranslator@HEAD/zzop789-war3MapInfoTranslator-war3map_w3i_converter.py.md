---
type: vendor-doc
source_repo: "https://github.com/zzop789/war3MapInfoTranslator"
source_path: "war3map_w3i_converter.py"
ref: HEAD
---

#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
War3Map W3I格式转换器 - 专门处理 {key, value} 格式
"""

import re
import csv
import os
from typing import Dict, Any, List, Tuple, Optional
from pathlib import Path

class War3MapW3IConverter:
    """War3Map W3I格式转换器"""
    
    def __init__(self):
        self.version = 25  # 默认版本号
        self.data = {}
        
    def txt_to_csv(self, txt_file: str, output_dir: str = None) -> Dict[str, str]:
        """
        将W3I TXT文件转换为CSV文件
        
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
            csv_file = os.path.join(output_dir, f"{base_name}.csv")
            
            # 生成CSV文件
            self._write_csv(self.data, csv_file)
            
            return {'w3i': csv_file}
            
        except Exception as e:
            raise Exception(f"W3I TXT转CSV失败: {str(e)}")
    
    def csv_to_txt(self, csv_files: List[str], output_file: str) -> str:
        """
        将CSV文件转换为W3I TXT文件
        
        Args:
            csv_files: CSV文件路径列表
            output_file: 输出的TXT文件路径
            
        Returns:
            生成的TXT文件路径
        """
        try:
            data = {}
            
            # 读取CSV文件
            for csv_file in csv_files:
                if os.path.exists(csv_file):
                    data = self._read_csv(csv_file)
                    break
            
            # 生成TXT文件
            self._write_txt_file(output_file, data)
            
            return output_file
            
        except Exception as e:
            raise Exception(f"CSV转W3I TXT失败: {str(e)}")
    
    def _parse_txt_file(self, txt_file: str) -> None:
        """解析W3I TXT文件内容"""
        print(f"正在解析W3I文件: {txt_file}")
        
        # 读取文件内容
        content = self._read_file_with_encoding(txt_file)
        
        # 提取整个文件的主结构：return { ... }
        main_structure = re.search(r'return\s*\{(.*)\}', content, re.DOTALL)
        if not main_structure:
            raise Exception("未找到标准的return结构")
        
        main_content = main_structure.group(1)
        
        # 解析键值对
        self.data = self._parse_key_value_pairs(main_content)
        print(f"W3I数据解析完成，条目数: {len(self.data)}")
    
    def _parse_key_value_pairs(self, content: str) -> Dict[str, str]:
        """解析 {key, value} 格式的键值对"""
        data = {}
        pos = 0
        length = len(content)
        entry_count = 0
        
        while pos < length:
            # 查找 {key, value} 模式
            pair_match = re.search(r'\{\s*([^,}]+)\s*,\s*([^}]*)\}', content[pos:])
            if not pair_match:
                break
            
            key = pair_match.group(1).strip()
            value = pair_match.group(2).strip()
            
            # 清理键名（去掉引号）
            if key.startswith("'") and key.endswith("'"):
                key = key[1:-1]
            elif key.startswith('"') and key.endswith('"'):
                key = key[1:-1]
            
            # 处理复杂值（如嵌套表）
            if value.startswith('{'):
                # 找到完整的嵌套表
                brace_count = 0
                start_pos = pos + pair_match.start(2)
                end_pos = start_pos
                
                while end_pos < len(content):
                    if content[end_pos] == '{':
                        brace_count += 1
                    elif content[end_pos] == '}':
                        brace_count -= 1
                        if brace_count == 0:
                            break
                    end_pos += 1
                
                if brace_count == 0:
                    value = content[start_pos:end_pos + 1]
                    # 更新匹配位置
                    pos = end_pos + 1
                else:
                    pos += pair_match.end()
            else:
                pos += pair_match.end()
            
            # 处理重复键
            if key in data:
                data[key] = data[key] + "おなに" + value
            else:
                data[key] = value
            
            entry_count += 1
            
            # 跳过逗号和空白
            while pos < length and content[pos] in [',', '\n', '\r', ' ', '\t']:
                pos += 1
        
        print(f"  完成解析，共 {len(data)} 个条目")
        return data
    
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
    
    def _write_csv(self, data: Dict[str, str], filename: str) -> None:
        """写入CSV文件"""
        print(f"写入W3I CSV文件: {filename}")
        print(f"  数据条目数: {len(data)}")
        
        with open(filename, 'w', newline='', encoding='utf-8-sig') as csvfile:
            writer = csv.writer(csvfile)
            
            # 版本号和表头
            version_row = [f"Version: {self.version}", '']
            writer.writerow(version_row)
            writer.writerow(['Key', 'Value'])
            
            # 数据行
            for key, value in data.items():
                cleaned_value = self._clean_value_for_csv(value)
                writer.writerow([key, cleaned_value])
    
    def _clean_value_for_csv(self, value: str) -> str:
        """清理字段值，移除可能破坏CSV结构的字符"""
        if not value:
            return ''
        
        # 移除换行符、制表符，压缩多余空格
        cleaned = value.replace('\n', ' ').replace('\r', ' ').replace('\t', ' ')
        # 压缩多个连续空格为单个空格
        cleaned = ' '.join(cleaned.split())
        
        return cleaned
    
    def _read_csv(self, csv_file: str) -> Dict[str, str]:
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
            
            # 跳过表头
            next(reader)
            
            # 读取数据
            for row in reader:
                if len(row) >= 2 and row[0].strip():
                    key = row[0].strip()
                    value = row[1].strip() if len(row) > 1 else ''
                    
                    if key:
                        data[key] = value
        
        return data
    
    def _write_txt_file(self, output_file: str, data: Dict[str, str]) -> None:
        """写入W3I TXT文件"""
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write("return\n{\n")
            
            # 写入数据
            for key, value in data.items():
                # 检查是否包含重复字段分隔符
                if "おなに" in value:
                    # 分割重复字段的多个值
                    values = value.split("おなに")
                    for val in values:
                        formatted_value = self._format_field_value(val)
                        f.write(f"\t{{'{key}',{formatted_value}}},\n")
                else:
                    # 单个字段值
                    formatted_value = self._format_field_value(value)
                    f.write(f"\t{{'{key}',{formatted_value}}},\n")
            
            f.write("}\n")
    
    def _format_field_value(self, field_value: str) -> str:
        """格式化字段值"""
        if not field_value:
            return '""'
        
        # 如果已经是嵌套表格式，直接返回
        if field_value.strip().startswith('{') and field_value.strip().endswith('}'):
            return field_value
        
        # 判断是否为数字
        try:
            float(field_value)
            return field_value
        except ValueError:
            # 字符串值，添加引号
            cleaned_value = field_value.strip('"\'')
            return f'"{cleaned_value}"'
    
    def get_statistics(self) -> Dict[str, Any]:
        """获取数据统计信息"""
        return {
            'version': self.version,
            'total_count': len(self.data)
        }

# 便捷函数
def convert_w3i_txt_to_csv(txt_file: str, output_dir: str = None) -> Dict[str, str]:
    """便捷函数：W3I TXT转CSV"""
    converter = War3MapW3IConverter()
    return converter.txt_to_csv(txt_file, output_dir)

def convert_csv_to_w3i_txt(csv_files: List[str], output_file: str) -> str:
    """便捷函数：CSV转W3I TXT"""
    converter = War3MapW3IConverter()
    return converter.csv_to_txt(csv_files, output_file)

if __name__ == "__main__":
    # 测试代码
    print("War3Map W3I转换器 - 专门处理 {key, value} 格式")
