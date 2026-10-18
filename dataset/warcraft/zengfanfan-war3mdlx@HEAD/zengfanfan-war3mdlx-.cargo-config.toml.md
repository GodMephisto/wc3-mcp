---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: ".cargo/config.toml"
ref: HEAD
---

[registry]
default = "tuna"

[source.crates-io]
# replace-with = 'tuna'

[source.zju]
registry = "sparse+https://mirrors.zju.edu.cn/crates.io-index/"
[registries.zju]
index = "sparse+https://mirrors.zju.edu.cn/crates.io-index/"

[source.tuna]
registry = "sparse+https://mirrors.tuna.tsinghua.edu.cn/crates.io-index/"
[registries.tuna]
index = "sparse+https://mirrors.tuna.tsinghua.edu.cn/crates.io-index/"

[source.aliyun]
registry = "sparse+https://mirrors.aliyun.com/crates.io-index/"
[registries.aliyun]
index = "sparse+https://mirrors.aliyun.com/crates.io-index/"
