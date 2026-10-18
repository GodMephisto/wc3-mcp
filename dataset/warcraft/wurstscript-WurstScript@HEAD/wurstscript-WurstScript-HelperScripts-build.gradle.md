---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "HelperScripts/build.gradle"
ref: HEAD
---

plugins {
    id 'java'
    id 'application'
}

repositories {
    maven { url 'https://jitpack.io' }
    mavenCentral()
}

dependencies {
    implementation 'com.github.inwc3:wc3libs:9a11e1f089'
    implementation 'com.google.guava:guava:31.1-jre'
    testImplementation 'org.testng:testng:7.11.0'
}

application {
    mainClass = 'objEditing.abilities.GenAbilities'
}

run {
    workingDir = project.projectDir
}

sourceSets {
    main {
        java {
            srcDirs = ['src']
            exclude 'test/**'
        }
    }
}

sourceSets.test.java.srcDir('../test-support')

test {
    timeout = java.time.Duration.ofMinutes(2)
    failFast = true
    systemProperty 'wurst.test.timeoutMillis', project.findProperty('testTimeoutMillis') ?: '60000'
    useTestNG {
        listeners.add('org.wurstscript.testing.TestTimeoutWatchdog')
    }
}
