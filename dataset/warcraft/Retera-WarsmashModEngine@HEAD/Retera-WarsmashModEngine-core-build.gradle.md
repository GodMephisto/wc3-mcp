---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/build.gradle"
ref: HEAD
---

[compileJava, compileTestJava]*.options*.encoding = 'UTF-8'

sourceCompatibility = 1.17

sourceSets.main.java.srcDirs = [ "src/" ]


eclipse.project {
    name = appName + "-core"
}
