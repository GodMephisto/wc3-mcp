---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/build.gradle"
ref: HEAD
---

[compileJava, compileTestJava]*.options*.encoding = 'UTF-8'

sourceCompatibility = 1.17

sourceSets.main.java.srcDirs = [ "src/" ]


eclipse.project {
    name = appName + "-craft3data"
}
//dependencies {
//	compile(name: 'idw-gpl', ext:'jar')
//	compile(name: 'blp-iio-plugin', ext:'jar')
//}
