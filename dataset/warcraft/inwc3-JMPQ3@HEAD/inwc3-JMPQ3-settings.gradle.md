---
type: vendor-doc
source_repo: "https://github.com/inwc3/JMPQ3"
source_path: "settings.gradle"
ref: HEAD
---

plugins {
    // Lets Gradle download the Java 25 toolchain when the JDK running Gradle is
    // older. Needed for JitPack, whose newest built-in identifier is openjdk17.
    id 'org.gradle.toolchains.foojay-resolver-convention' version '1.0.0'
}

rootProject.name = 'jmpq3'
