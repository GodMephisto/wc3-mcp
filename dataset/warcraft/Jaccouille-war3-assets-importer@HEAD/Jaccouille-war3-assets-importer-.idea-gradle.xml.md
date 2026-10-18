---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: ".idea/gradle.xml"
ref: HEAD
---

<?xml version="1.0" encoding="UTF-8"?>
<project version="4">
  <component name="GradleMigrationSettings" migrationVersion="1" />
  <component name="GradleSettings">
    <option name="linkedExternalProjectsSettings">
      <GradleProjectSettings>
        <option name="externalProjectPath" value="$PROJECT_DIR$/../JMPQ3" />
        <option name="gradleJvm" value="17" />
        <option name="modules">
          <set>
            <option value="$PROJECT_DIR$/../JMPQ3" />
          </set>
        </option>
      </GradleProjectSettings>
      <GradleProjectSettings>
        <compositeConfiguration>
          <compositeBuild compositeDefinitionSource="SCRIPT">
            <builds>
              <build path="$PROJECT_DIR$/../wc3libs" name="wc3libs">
                <projects>
                  <project path="$PROJECT_DIR$/../wc3libs" />
                </projects>
              </build>
            </builds>
          </compositeBuild>
        </compositeConfiguration>
        <option name="externalProjectPath" value="$PROJECT_DIR$" />
        <option name="gradleJvm" value="17" />
        <option name="modules">
          <set>
            <option value="$PROJECT_DIR$" />
            <option value="$PROJECT_DIR$/../wc3libs" />
          </set>
        </option>
      </GradleProjectSettings>
    </option>
  </component>
</project>