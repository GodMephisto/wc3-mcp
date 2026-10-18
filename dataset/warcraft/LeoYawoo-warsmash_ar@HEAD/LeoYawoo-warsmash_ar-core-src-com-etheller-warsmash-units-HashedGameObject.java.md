---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/units/HashedGameObject.java"
ref: HEAD
---

package com.etheller.warsmash.units;

import java.util.*;

public abstract class HashedGameObject implements GameObject {
    HashMap<StringKey, List<String>> fields = new HashMap<>();
    final String id;
    final ObjectData parentTable;

    public HashedGameObject(final String id, final ObjectData table) {
        this.id = id;
        this.parentTable = table;
    }

    @Override
    public void setField(final String field, final String value) {
        final StringKey key = new StringKey(field);
        List<String> list = this.fields.get(key);
        if (list == null) {
            list = new ArrayList<>();
            this.fields.put(key, list);
            list.add(value);
        } else {
            list.set(0, value);
        }
    }

    public void setField(final String field, final List<String> value) {
        final StringKey key = new StringKey(field);
        if (value.isEmpty()) {
            this.fields.remove(key);
        } else {
            this.fields.put(key, value);
        }
    }

    @Override
    public String getField(final String field) {
        final String value = "";
        if (this.fields.get(new StringKey(field)) != null) {
            final List<String> list = this.fields.get(new StringKey(field));
            final StringBuilder sb = new StringBuilder();
            if (list != null) {
                for (final String str : list) {
                    if (sb.length() != 0) {
                        sb.append(',');
                    }
                    sb.append(str);
                }
                return sb.toString();
            }
        }
        return value;
    }

    public boolean hasField(final String field) {
        return this.fields.containsKey(new StringKey(field));
    }

    @Override
    public int getFieldValue(final String field) {
        int i = 0;
        try {
            i = Integer.parseInt(getField(field).trim());
        } catch (final NumberFormatException ignored) {

        }
        return i;
    }

    @Override
    public float getFieldFloatValue(final String field) {
        float i = 0;
        try {
            i = Float.parseFloat(getField(field).trim());
        } catch (final NumberFormatException ignored) {

        }
        return i;
    }

    @Override
    public float getFieldFloatValue(final String field, final int index) {
        float i = 0;
        try {
            i = Float.parseFloat(getField(field, index).trim());
        } catch (final NumberFormatException ignored) {

        }
        return i;
    }

    @Override
    public void setField(final String field, final String value, final int index) {
        final StringKey key = new StringKey(field);
        List<String> list = this.fields.get(key);
        if (list == null) {
            if (index == 0) {
                list = new ArrayList<>();
                this.fields.put(key, list);
                list.add(value);
            } else {
                throw new IndexOutOfBoundsException();
            }
        } else {
            if (list.size() == index) {
                list.add(value);
            } else {
                list.set(index, value);
            }
        }
    }

    @Override
    public String getField(final String field, final int index) {
        String value = "";
        if (this.fields.get(new StringKey(field)) != null) {
            final List<String> list = this.fields.get(new StringKey(field));
            if (list != null) {
                if (list.size() > index) {
                    value = list.get(index);
                } else if (list.size() > 0) {
                    value = list.get(list.size() - 1);
                }
            }
        }
        return value;
    }

    @Override
    public int getFieldValue(final String field, final int index) {
        int i = 0;
        try {
            i = Integer.parseInt(getField(field, index).trim());
        } catch (final NumberFormatException ignored) {

        }
        return i;
    }

    @Override
    public List<GameObject> getFieldAsList(final String field, final ObjectData parentTable) {
        List<GameObject> fieldAsList;
        fieldAsList = new ArrayList<>();
        final String stringList = getField(field);
        final String[] listAsArray = stringList.split(",");
        for (final String buildingId : listAsArray) {
            final GameObject referencedUnit = parentTable.get(buildingId);
            if (referencedUnit != null) {
                fieldAsList.add(referencedUnit);
            }
        }
        return fieldAsList;
    }

    @Override
    public String toString() {
        return getField("Name");
    }

    @Override
    public String getId() {
        return this.id;
    }

    @Override
    public String getName() {
        StringBuilder name = new StringBuilder(getField("Name"));
        boolean nameKnown = name.length() >= 1;
        if (!nameKnown && !getField("code").equals(this.id) && (getField("code").length() >= 4)) {
            final GameObject other = this.parentTable.get(getField("code").substring(0, 4));
            if (other != null) {
                name = new StringBuilder(other.getName());
                nameKnown = true;
            }
        }
        if (!nameKnown && (getField("EditorName").length() > 1)) {
            name = new StringBuilder(getField("EditorName"));
            nameKnown = true;
        }
        if (!nameKnown && (getField("Editorname").length() > 1)) {
            name = new StringBuilder(getField("Editorname"));
            nameKnown = true;
        }
        if (!nameKnown && (getField("BuffTip").length() > 1)) {
            name = new StringBuilder(getField("BuffTip"));
            nameKnown = true;
        }
        if (!nameKnown && (getField("Bufftip").length() > 1)) {
            name = new StringBuilder(getField("Bufftip"));
            nameKnown = true;
        }
        if (nameKnown && name.toString().startsWith("WESTRING")) {
            if (!name.toString().contains(" ")) {
                name = new StringBuilder(this.parentTable.getLocalizedString(name.toString()));
            } else {
                final String[] names = name.toString().split(" ");
                name = new StringBuilder();
                for (final String subName : names) {
                    if (name.length() > 0) {
                        name.append(" ");
                    }
                    if (subName.startsWith("WESTRING")) {
                        name.append(this.parentTable.getLocalizedString(subName));
                    } else {
                        name.append(subName);
                    }
                }
            }
            if (name.toString().startsWith("\"") && name.toString().endsWith("\"")) {
                name = new StringBuilder(name.substring(1, name.length() - 1));
            }
            setField("Name", name.toString());
        }
        if (!nameKnown) {
            name = new StringBuilder(this.parentTable.getLocalizedString("WESTRING_UNKNOWN") + " '" + getId() + "'");
        }
        if (getField("campaign").startsWith("1") && Character.isUpperCase(getId().charAt(0))) {
            name = new StringBuilder(getField("Propernames"));
            if (name.toString().contains(",")) {
                name = new StringBuilder(name.toString().split(",")[0]);
            }
        }
        String suf = getField("EditorSuffix");
        if ((suf.length() > 0) && !suf.equals("_")) {
            if (suf.startsWith("WESTRING")) {
                suf = this.parentTable.getLocalizedString(suf);
            }
            if (!suf.startsWith(" ")) {
                name.append(" ");
            }
            name.append(suf);
        }
        return name.toString();
    }

    @Override
    public ObjectData getTable() {
        return this.parentTable;
    }

    @Override
    public Set<String> keySet() {
        final Set<String> keySet = new HashSet<>();
        for (final StringKey key : this.fields.keySet()) {
            keySet.add(key.getString());
        }
        return keySet;
    }
}
