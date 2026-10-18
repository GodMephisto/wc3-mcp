---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/units/GameObject.java"
ref: HEAD
---

package com.etheller.warsmash.units;

import java.util.Collections;
import java.util.List;
import java.util.Set;

public interface GameObject {

    GameObject EMPTY = new GameObject() {

        @Override
        public void setField(final String field, final String value, final int index) {
        }

        @Override
        public void setField(final String field, final String value) {
        }

        @Override
        public Set<String> keySet() {
            return Collections.emptySet();
        }

        @Override
        public ObjectData getTable() {
            return null;
        }

        @Override
        public String getName() {
            return "<No data>";
        }

        @Override
        public String getId() {
            return "0000";
        }

        @Override
        public int getFieldValue(final String field, final int index) {
            return 0;
        }

        @Override
        public int getFieldValue(final String field) {
            return 0;
        }

        @Override
        public float getFieldFloatValue(final String field) {
            return 0;
        }

        @Override
        public float getFieldFloatValue(final String field, final int index) {
            return 0;
        }

        @Override
        public List<? extends GameObject> getFieldAsList(final String field, final ObjectData objectData) {
            return Collections.emptyList();
        }

        @Override
        public String getField(final String field, final int index) {
            return "";
        }

        @Override
        public String getField(final String field) {
            return "";
        }

        @Override
        public String getLegacyName() {
            return "custom_0000";
        }
    };

    void setField(String field, String value);

    void setField(String field, String value, int index);

    String getField(String field);

    String getField(String field, int index);

    int getFieldValue(String field);

    int getFieldValue(String field, int index);

    float getFieldFloatValue(String field);

    float getFieldFloatValue(String field, int index);

    List<? extends GameObject> getFieldAsList(String field, ObjectData objectData);

    String getId();

    ObjectData getTable();

    String getName();

    String getLegacyName();

    Set<String> keySet();

}