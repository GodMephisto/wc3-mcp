---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "src/main/java/com/hiveworkshop/war3assetsimporter/gui/i18n/Messages.java"
ref: HEAD
---

package com.hiveworkshop.war3assetsimporter.gui.i18n;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.util.Locale;
import java.util.MissingResourceException;
import java.util.PropertyResourceBundle;
import java.util.ResourceBundle;

/**
 * Centralised i18n helper for all GUI string lookups.
 *
 * <p>Usage:
 * <pre>
 *   JButton btn = new JButton(Messages.get("button.openMap"));
 *   Messages.setLocale(Locale.FRENCH);  // switch language at runtime
 * </pre>
 *
 * <p>If a key is missing the value {@code !key!} is returned so broken keys are
 * immediately visible without crashing the application.
 */
public final class Messages {

    private static final ResourceBundle.Control UTF8_CONTROL = new ResourceBundle.Control() {
        @Override
        public ResourceBundle newBundle(String baseName, Locale locale, String format,
                                        ClassLoader loader, boolean reload) throws IOException {
            String resourceName = toResourceName(toBundleName(baseName, locale), "properties");
            try (InputStream is = loader.getResourceAsStream(resourceName)) {
                if (is == null) return null;
                return new PropertyResourceBundle(new InputStreamReader(is, StandardCharsets.UTF_8));
            }
        }
    };

    private static Locale activeLocale = Locale.ENGLISH;
    private static ResourceBundle bundle = ResourceBundle.getBundle("messages", activeLocale, UTF8_CONTROL);

    private Messages() {
    }

    /**
     * Returns the current locale used for string lookups.
     * Always returns the locale explicitly passed to {@link #setLocale}, not
     * the bundle's internal locale (which may be ROOT when the base bundle is used).
     */
    public static Locale getLocale() {
        return activeLocale;
    }

    /**
     * Switches the active locale and reloads the resource bundle.
     * Call {@code applyI18n()} on all GUI components afterwards to refresh labels.
     */
    public static void setLocale(Locale locale) {
        activeLocale = locale;
        bundle = ResourceBundle.getBundle("messages", locale, UTF8_CONTROL);
        Locale.setDefault(locale);
    }

    /**
     * Returns the translated string for {@code key}, or {@code !key!} if not found.
     */
    public static String get(String key) {
        try {
            return bundle.getString(key);
        } catch (MissingResourceException e) {
            return '!' + key + '!';
        }
    }
}
