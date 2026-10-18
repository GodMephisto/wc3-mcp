---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/build.gradle"
ref: HEAD
---

[compileJava, compileTestJava]*.options*.encoding = 'UTF-8'

sourceSets.main.java.srcDirs = [ "src/" ]


eclipse.project {
    name = appName + "-core"
}
