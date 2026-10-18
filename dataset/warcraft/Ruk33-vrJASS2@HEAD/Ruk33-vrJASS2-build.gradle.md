---
type: vendor-doc
source_repo: "https://github.com/Ruk33/vrJASS2"
source_path: "build.gradle"
ref: HEAD
---

apply plugin: 'java'
apply plugin: 'idea'
apply plugin: 'net.anshulverma.gradle.estilo'

repositories {
    mavenCentral()
    maven { url "https://jitpack.io" } // To fetch from Github
}

dependencies {
    testCompile 'junit:junit:4.11'
    compile 'com.github.peq:jMpq:e9ddcaa0cf2a3d860d2bc59779690312bbbb3c20'
    compile 'org.antlr:antlr4:4.5'
    compile 'junit:junit:4.11'
    compile 'com.google.guava:guava:20.0'
    compile group: 'com.eclipsesource.minimal-json', name: 'minimal-json', version: '0.9.4'
}

buildscript {
    repositories {
        mavenCentral()
    }

    dependencies {
        classpath 'net.anshulverma.gradle:gradle-estilo-plugin:0.4.8'
    }
}

jar {
    baseName = 'vrjc'
    manifest {
        attributes 'Main-Class': 'ruke.vrj.Main'
    }
    // Include dependencies in final jar
    from configurations.compile.collect { it.isDirectory() ? it : zipTree(it) }
}

compileJava {
    sourceCompatibility = '1.8'
}

estilo {
    source 'google'

    suppressions {
        prefix('antl') {
            checks('')
        }
    }

    checks {
        CustomImportOrder(remove: true)
        JavadocParagraph(remove: true)
    }
}

task buildGrammar(type: Exec, description: 'Build ANTLR grammar') {
    commandLine 'java', '-jar', 'bin/antlr.jar', '-o', 'src/main/java/ruke/vrj/antlr', '-package', 'ruke.vrj.antlr', '-Dlanguage=Java', '-encoding', 'utf-8', '-visitor', '-no-listener', '-lib', '.', 'vrj.g4'
}

compileJava.dependsOn buildGrammar