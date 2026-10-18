---
type: vendor-doc
source_repo: "https://github.com/zzop789/war3MapInfoTranslator"
source_path: "war3map_gui.py"
ref: HEAD
---

#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
War3Map数据转换工具 - 图形化界面
支持TXT <-> CSV双向转换的用户友好界面
"""

import tkinter as tk
from tkinter import ttk, filedialog, messagebox, scrolledtext
import os
import re
import threading
from pathlib import Path
from typing import List, Dict
import sys
import re

# 导入转换器核心模块
try:
    from war3map_converter import War3MapConverter, convert_txt_to_csv, convert_csv_to_txt, merge_txt_files, auto_merge_txt_pairs
    from war3map_w3i_converter import War3MapW3IConverter, convert_w3i_txt_to_csv, convert_csv_to_w3i_txt
except ImportError:
    messagebox.showerror("错误", "无法导入转换器模块，请确保文件存在！")
    sys.exit(1)

class War3MapConverterGUI:
    def __init__(self, root):
        self.root = root
        self.root.title("War3Map数据转换工具 v1.0")
        self.root.geometry("800x600")
        self.root.resizable(True, True)
        
        # 设置样式
        self.style = ttk.Style()
        self.style.theme_use('clam')
        
        # 初始化变量
        self.input_files = []
        self.output_directory = tk.StringVar()
        self.conversion_mode = tk.StringVar(value="txt_to_csv")
        
        self.setup_ui()
        
    def setup_ui(self):
        """设置用户界面"""
        # 主容器
        main_frame = ttk.Frame(self.root, padding="10")
        main_frame.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        
        # 配置网格权重
        self.root.columnconfigure(0, weight=1)
        self.root.rowconfigure(0, weight=1)
        main_frame.columnconfigure(1, weight=1)
        
        # 标题
        title_label = ttk.Label(main_frame, text="War3Map数据格式转换工具", 
                               font=('Arial', 16, 'bold'))
        title_label.grid(row=0, column=0, columnspan=3, pady=(0, 20))
        
        # 转换模式选择
        mode_frame = ttk.LabelFrame(main_frame, text="转换模式", padding="10")
        mode_frame.grid(row=1, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=(0, 10))
        
        ttk.Radiobutton(mode_frame, text="TXT → CSV", 
                       variable=self.conversion_mode, value="txt_to_csv",
                       command=self.on_mode_change).grid(row=0, column=0, padx=(0, 20))
        
        ttk.Radiobutton(mode_frame, text="CSV → TXT", 
                       variable=self.conversion_mode, value="csv_to_txt",
                       command=self.on_mode_change).grid(row=0, column=1, padx=(0, 20))
        
        ttk.Radiobutton(mode_frame, text="合并 TXT", 
                       variable=self.conversion_mode, value="merge_txt",
                       command=self.on_mode_change).grid(row=0, column=2)
        
        # 文件选择区域
        file_frame = ttk.LabelFrame(main_frame, text="文件选择", padding="10")
        file_frame.grid(row=2, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=(0, 10))
        file_frame.columnconfigure(1, weight=1)
        
        # 输入文件
        ttk.Label(file_frame, text="输入文件:").grid(row=0, column=0, sticky=tk.W, pady=(0, 5))
        
        self.file_listbox = tk.Listbox(file_frame, height=4, selectmode=tk.EXTENDED)
        self.file_listbox.grid(row=1, column=0, columnspan=2, sticky=(tk.W, tk.E), pady=(0, 5))
        
        file_button_frame = ttk.Frame(file_frame)
        file_button_frame.grid(row=2, column=0, columnspan=2, sticky=(tk.W, tk.E), pady=(0, 10))
        
        ttk.Button(file_button_frame, text="添加文件", 
                  command=self.add_files).pack(side=tk.LEFT, padx=(0, 5))
        ttk.Button(file_button_frame, text="移除选中", 
                  command=self.remove_files).pack(side=tk.LEFT, padx=(0, 5))
        ttk.Button(file_button_frame, text="清空列表", 
                  command=self.clear_files).pack(side=tk.LEFT)
        
        # 输出目录
        ttk.Label(file_frame, text="输出目录:").grid(row=3, column=0, sticky=tk.W, pady=(5, 0))
        
        output_frame = ttk.Frame(file_frame)
        output_frame.grid(row=4, column=0, columnspan=2, sticky=(tk.W, tk.E), pady=5)
        output_frame.columnconfigure(0, weight=1)
        
        self.output_entry = ttk.Entry(output_frame, textvariable=self.output_directory)
        self.output_entry.grid(row=0, column=0, sticky=(tk.W, tk.E), padx=(0, 5))
        
        ttk.Button(output_frame, text="浏览", 
                  command=self.select_output_directory).grid(row=0, column=1)
        
        # 操作按钮
        button_frame = ttk.Frame(main_frame)
        button_frame.grid(row=3, column=0, columnspan=3, pady=10)
        
        self.convert_button = ttk.Button(button_frame, text="开始转换", 
                                        command=self.start_conversion, 
                                        style='Accent.TButton')
        self.convert_button.pack(side=tk.LEFT, padx=(0, 10))
        
        ttk.Button(button_frame, text="清除日志", 
                  command=self.clear_log).pack(side=tk.LEFT)
        
        # 进度条
        self.progress = ttk.Progressbar(main_frame, mode='indeterminate')
        self.progress.grid(row=4, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=5)
        
        # 日志输出区域
        log_frame = ttk.LabelFrame(main_frame, text="转换日志", padding="5")
        log_frame.grid(row=5, column=0, columnspan=3, sticky=(tk.W, tk.E, tk.N, tk.S), pady=10)
        log_frame.columnconfigure(0, weight=1)
        log_frame.rowconfigure(0, weight=1)
        
        self.log_text = scrolledtext.ScrolledText(log_frame, height=15, width=80)
        self.log_text.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))
        
        # 配置主框架行权重
        main_frame.rowconfigure(5, weight=1)
        
        # 初始化界面状态
        self.on_mode_change()
        
    def on_mode_change(self):
        """转换模式改变时的处理"""
        mode = self.conversion_mode.get()
        if mode == "txt_to_csv":
            self.log("切换到 TXT → CSV 模式")
        elif mode == "csv_to_txt":
            self.log("切换到 CSV → TXT 模式")
        else:
            self.log("切换到 合并 TXT 模式")
        
        # 清空文件列表
        self.clear_files()
    
    def add_files(self):
        """添加文件"""
        mode = self.conversion_mode.get()
        
        if mode == "txt_to_csv":
            filetypes = [("TXT文件", "*.txt"), ("所有文件", "*.*")]
            title = "选择TXT文件"
        elif mode == "csv_to_txt":
            filetypes = [("CSV文件", "*.csv"), ("所有文件", "*.*")]
            title = "选择CSV文件"
        else:  # merge_txt
            filetypes = [("TXT文件", "*.txt"), ("所有文件", "*.*")]
            title = "选择要合并的TXT文件"
        
        files = filedialog.askopenfilenames(
            title=title,
            filetypes=filetypes
        )
        
        for file_path in files:
            if file_path not in self.input_files:
                self.input_files.append(file_path)
                self.file_listbox.insert(tk.END, os.path.basename(file_path))
        
        if files:
            self.log(f"添加了 {len(files)} 个文件")
    
    def remove_files(self):
        """移除选中的文件"""
        selected_indices = self.file_listbox.curselection()
        if not selected_indices:
            messagebox.showwarning("警告", "请先选择要移除的文件")
            return
        
        # 从后往前删除，避免索引变化
        for index in reversed(selected_indices):
            self.file_listbox.delete(index)
            del self.input_files[index]
        
        self.log(f"移除了 {len(selected_indices)} 个文件")
    
    def clear_files(self):
        """清空文件列表"""
        self.input_files.clear()
        self.file_listbox.delete(0, tk.END)
        self.log("清空文件列表")
    
    def select_output_directory(self):
        """选择输出目录"""
        directory = filedialog.askdirectory(title="选择输出目录")
        if directory:
            self.output_directory.set(directory)
            self.log(f"设置输出目录: {directory}")
    
    def start_conversion(self):
        """开始转换"""
        if not self.input_files:
            messagebox.showwarning("警告", "请先添加要转换的文件")
            return
        
        output_dir = self.output_directory.get()
        if not output_dir:
            messagebox.showwarning("警告", "请选择输出目录")
            return
        
        if not os.path.exists(output_dir):
            try:
                os.makedirs(output_dir)
            except Exception as e:
                messagebox.showerror("错误", f"无法创建输出目录: {str(e)}")
                return
        
        # 禁用转换按钮，显示进度条
        self.convert_button.config(state='disabled')
        self.progress.start()
        
        # 在新线程中执行转换
        thread = threading.Thread(target=self.perform_conversion, args=(output_dir,))
        thread.daemon = True
        thread.start()
    
    def perform_conversion(self, output_dir):
        """执行转换操作"""
        try:
            mode = self.conversion_mode.get()
            success_count = 0
            total_count = len(self.input_files)
            
            if mode == "txt_to_csv":
                # TXT转CSV
                for txt_file in self.input_files:
                    try:
                        self.log(f"正在转换: {os.path.basename(txt_file)}")
                        
                        # 检测文件类型并选择合适的转换器
                        file_type = self.detect_file_type(txt_file)
                        converter = self.get_appropriate_converter(file_type)
                        
                        self.log(f"  检测文件类型: {file_type}")
                        
                        result = converter.txt_to_csv(txt_file, output_dir)
                        
                        if result:
                            for file_type_key, file_path in result.items():
                                self.log(f"  生成 {file_type_key}: {os.path.basename(file_path)}")
                            success_count += 1
                        
                        # 显示统计信息
                        stats = converter.get_statistics()
                        if file_type == 'w3i':
                            self.log(f"  版本: {stats['version']}, 条目数: {stats['total_count']}")
                        else:
                            self.log(f"  版本: {stats['version']}, 原生: {stats['origin_count']}, 自定义: {stats['custom_count']}")
                            
                            # 显示字段信息
                            field_info = converter.get_field_info()
                            origin_fields = field_info['origin_fields']
                            custom_fields = field_info['custom_fields']
                            all_fields = field_info['all_fields']
                            
                            if all_fields:
                                self.log(f"  发现字段总数: {len(all_fields)}")
                                if origin_fields:
                                    self.log(f"  ORIGIN字段数: {len(origin_fields)}")
                                if custom_fields:
                                    self.log(f"  CUSTOM字段数: {len(custom_fields)}")
                                
                                # 显示字段差异
                                if origin_fields and custom_fields:
                                    common = origin_fields & custom_fields
                                    origin_only = origin_fields - custom_fields
                                    custom_only = custom_fields - origin_fields
                                    
                                    self.log(f"  共同字段: {len(common)}个")
                                    if origin_only:
                                        self.log(f"  仅ORIGIN有: {len(origin_only)}个")
                                    if custom_only:
                                        self.log(f"  仅CUSTOM有: {len(custom_only)}个")
                                
                                # 显示部分字段名称
                                if len(all_fields) <= 20:
                                    self.log(f"  所有字段: {sorted(list(all_fields))}")
                                else:
                                    sample_fields = sorted(list(all_fields))[:15]
                                    self.log(f"  字段示例: {sample_fields}... (共{len(all_fields)}个)")
                                
                                self.log(f"  详细字段信息已在控制台输出")
                        
                    except Exception as e:
                        self.log(f"  转换失败: {str(e)}")
            
            elif mode == "csv_to_txt":
                # CSV转TXT
                # 按文件名分组
                file_groups = self.group_csv_files(self.input_files)
                
                for group_name, csv_files in file_groups.items():
                    try:
                        output_file = os.path.join(output_dir, f"{group_name}.txt")
                        self.log(f"正在转换组: {group_name}")
                        
                        # 根据输出文件名检测文件类型
                        file_type = self.detect_file_type(output_file)
                        self.log(f"  检测文件类型: {file_type}")
                        
                        # 选择合适的转换函数
                        if file_type == 'w3i':
                            result = convert_csv_to_w3i_txt(csv_files, output_file)
                        else:
                            result = convert_csv_to_txt(csv_files, output_file)
                        
                        self.log(f"  生成: {os.path.basename(result)}")
                        success_count += 1
                        
                    except Exception as e:
                        self.log(f"  转换失败: {str(e)}")
            
            else:
                # 合并TXT文件 - 手动选择文件合并模式
                # 将文件按配对分组
                merge_groups = self.group_txt_files_for_merge(self.input_files)
                
                for group_name, files in merge_groups.items():
                    if 'origin' in files and 'custom' in files:
                        try:
                            output_file = os.path.join(output_dir, f"{group_name}.txt")
                            self.log(f"正在合并: {group_name}")
                            
                            result = merge_txt_files(files['origin'], files['custom'], output_file)
                            self.log(f"  生成: {os.path.basename(result)}")
                            success_count += 1
                            
                        except Exception as e:
                            self.log(f"  合并失败: {str(e)}")
                    else:
                        self.log(f"跳过不完整的文件组: {group_name}")
                
                total_count = len(merge_groups)
            
            # 转换完成
            self.log(f"\n转换完成! 成功: {success_count}/{total_count}")
            if success_count > 0:
                self.log(f"输出目录: {output_dir}")
            
        except Exception as e:
            self.log(f"转换过程中发生错误: {str(e)}")
        
        finally:
            # 恢复UI状态
            self.root.after(0, self.conversion_finished)
    
    def group_csv_files(self, csv_files):
        """将CSV文件按基础名称分组"""
        groups = {}
        
        for csv_file in csv_files:
            base_name = Path(csv_file).stem
            # 移除_origin或_custom后缀
            if base_name.endswith('_origin') or base_name.endswith('_custom'):
                group_name = '_'.join(base_name.split('_')[:-1])
            else:
                group_name = base_name
            
            if group_name not in groups:
                groups[group_name] = []
            groups[group_name].append(csv_file)
        
        return groups
    
    def group_txt_files_for_merge(self, txt_files):
        """将TXT文件按基础名称分组用于合并"""
        groups = {}
        
        for txt_file in txt_files:
            base_name = Path(txt_file).stem
            
            if base_name.endswith('_origin'):
                group_name = base_name[:-7]  # 移除'_origin'
                if group_name not in groups:
                    groups[group_name] = {}
                groups[group_name]['origin'] = txt_file
                
            elif base_name.endswith('_custom'):
                group_name = base_name[:-7]  # 移除'_custom'
                if group_name not in groups:
                    groups[group_name] = {}
                groups[group_name]['custom'] = txt_file
        
        return groups
    
    def conversion_finished(self):
        """转换完成后的UI更新"""
        self.progress.stop()
        self.convert_button.config(state='normal')
        messagebox.showinfo("完成", "转换任务已完成！")
    
    def log(self, message):
        """添加日志信息"""
        self.log_text.insert(tk.END, message + "\n")
        self.log_text.see(tk.END)
        self.root.update_idletasks()
    
    def clear_log(self):
        """清除日志"""
        self.log_text.delete(1.0, tk.END)
    
    def detect_file_type(self, file_path: str) -> str:
        """
        根据文件名检测文件类型
        
        Args:
            file_path: 文件路径
            
        Returns:
            文件类型: 'w3i', 'standard', 'unknown'
        """
        file_name = os.path.basename(file_path).lower()
        
        # 检查是否为war3map格式
        if file_name.startswith('war3map.w3') and file_name.endswith('.txt'):
            # 提取后缀
            suffix_match = re.search(r'war3map\.w3([a-z])\.txt$', file_name)
            if suffix_match:
                suffix = suffix_match.group(1)
                
                # w3i使用特殊格式
                if suffix == 'i':
                    return 'w3i'
                # 其他后缀(a,b,d,h,q,t,u)使用标准格式
                elif suffix in ['a', 'b', 'd', 'h', 'q', 't', 'u']:
                    return 'standard'
        
        return 'unknown'
    
    def get_appropriate_converter(self, file_type: str):
        """
        根据文件类型获取适当的转换器
        
        Args:
            file_type: 文件类型
            
        Returns:
            转换器实例
        """
        if file_type == 'w3i':
            return War3MapW3IConverter()
        elif file_type == 'standard':
            return War3MapConverter()
        else:
            # 默认使用标准转换器
            return War3MapConverter()
        
def main():
    """主函数"""
    root = tk.Tk()
    
    # 设置应用图标（如果有的话）
    try:
        # root.iconbitmap('icon.ico')  # 可以添加图标文件
        pass
    except:
        pass
    
    app = War3MapConverterGUI(root)
    root.mainloop()

if __name__ == "__main__":
    main()
