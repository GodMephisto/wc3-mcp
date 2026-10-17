---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "_original/Documents/怪物大纲.txt"
ref: HEAD
---

#! 怪物大纲

怪物设计：
怪物ID = 'mRTI'
m = 固定字母，表示是一个游戏中出现的怪物，代码判定方法： id>'m000' and id<'n000'
R = 数字+大写字母，表示怪物的主分类，代码判定方法： id>'mR0!' and id<'mR~|'
T = 数字+大写字母，表示怪物的子类别，代码判定方法： id>'mRT!' and id<'mRT|'
I = 数字+大写字母，单位索引，具体的某一个单位。

怪物类型 = 普通、强化、精英、首领
普通怪物 = 没有特殊技能
强化怪物 = 自带抗性皮肤
精英怪物 = 自带抗性皮肤、魔法抗性
首领怪物 = 自带抗性皮肤、魔法抗性、韧性等级
魔王怪物 = 自带抗性皮肤、魔法抗性、韧性等级、特殊AI

