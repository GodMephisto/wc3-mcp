---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/util/WorldEditStrings.java"
ref: HEAD
---

package com.etheller.warsmash.util;

import com.etheller.warsmash.datasources.DataSource;

import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.charset.StandardCharsets;
import java.util.MissingResourceException;
import java.util.PropertyResourceBundle;
import java.util.ResourceBundle;

public class WorldEditStrings implements StringBundle {
    private static ResourceBundle bundlegs;
    private static ResourceBundle bundle;

    public WorldEditStrings(final DataSource dataSource) {
        if (bundle != null || bundlegs != null) {
            return;
        }
        if (dataSource.has("UI\\WorldEditStrings.txt")) {
            try (InputStream fis = dataSource.getResourceAsStream("UI\\WorldEditStrings.txt"); InputStreamReader reader = new InputStreamReader(fis, StandardCharsets.UTF_8)) {
                bundle = new PropertyResourceBundle(reader);
            } catch (final IOException e) {
                throw new RuntimeException(e);
            }
        }

        try (InputStream fis = dataSource.getResourceAsStream("UI\\WorldEditGameStrings.txt"); InputStreamReader reader = new InputStreamReader(fis, StandardCharsets.UTF_8)) {
            bundlegs = new PropertyResourceBundle(reader);
        } catch (final IOException e) {
            throw new RuntimeException(e);
        }

    }

    @Override
    public String getString(String str) {
        try {
            while (str.toUpperCase().startsWith("WESTRING")) {
                str = internalGetString(str);
            }
            return str;
        } catch (final MissingResourceException exc) {
            try {
                return bundlegs.getString(str.toUpperCase());
            } catch (final MissingResourceException exc2) {
                return str;
            }
        }
    }

    private String internalGetString(final String key) {
        if (bundle == null) {
            return bundlegs.getString(key.toUpperCase());
        }
        try {
            String string = bundle.getString(key.toUpperCase());
            if ((string.charAt(0) == '"') && (string.length() >= 2) && (string.charAt(string.length() - 1) == '"')) {
                string = string.substring(1, string.length() - 1);
            }
            return string;
        } catch (final MissingResourceException exc) {
            return bundlegs.getString(key.toUpperCase());
        }
    }
}
