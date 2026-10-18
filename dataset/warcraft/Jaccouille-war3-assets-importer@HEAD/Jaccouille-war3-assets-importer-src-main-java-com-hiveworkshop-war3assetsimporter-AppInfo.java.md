---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "src/main/java/com/hiveworkshop/war3assetsimporter/AppInfo.java"
ref: HEAD
---

package com.hiveworkshop.war3assetsimporter;

import java.io.InputStream;
import java.util.Properties;

/**
 * Application-level constants: name and version.
 *
 * <p>The canonical application name is defined here as a Java constant so it is
 * never accidentally translated by the i18n system.  The version string is read
 * at class-load time from the {@code /version.properties} resource that Gradle
 * stamps during {@code processResources}.
 */
public final class AppInfo {

    /** Display name shown in window titles and dialogs — never translated. */
    public static final String APP_NAME = "Warcraft 3 Assets Importer";

    /** Version string injected by Gradle at build time (e.g. {@code "0.0.1"}). */
    public static final String VERSION;

    static {
        String v = "dev";
        try (InputStream is = AppInfo.class.getResourceAsStream("/version.properties")) {
            if (is != null) {
                Properties props = new Properties();
                props.load(is);
                String val = props.getProperty("version");
                if (val != null && !val.isBlank()) v = val.trim();
            }
        } catch (Exception ignored) {
        }
        VERSION = v;
    }

    private AppInfo() {
    }
}
