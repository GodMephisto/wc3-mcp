---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/build.gradle"
ref: HEAD
---

apply plugin: "java"

sourceCompatibility = 1.8
[compileJava, compileTestJava]*.options*.encoding = 'UTF-8'

sourceSets.main.java.srcDirs = [ "src/" ]


eclipse.project {
    name = appName + "-core"
}
