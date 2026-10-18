---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/units/DataTable.java"
ref: HEAD
---

package com.etheller.warsmash.units;

import com.etheller.warsmash.util.StringBundle;

import java.io.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.util.*;

public class DataTable implements ObjectData {
    public static final boolean DEBUG = false;
    private final StringBundle worldEditStrings;
    final Map<StringKey, Element> dataTable = new LinkedHashMap<>();

    public DataTable(final StringBundle worldEditStrings) {
        this.worldEditStrings = worldEditStrings;
    }

    @Override
    public String getLocalizedString(final String key) {
        return this.worldEditStrings.getString(key);
    }

    @Override
    public Set<String> keySet() {
        final Set<String> outputKeySet = new HashSet<>();
        final Set<StringKey> internalKeySet = this.dataTable.keySet();
        for (final StringKey key : internalKeySet) {
            outputKeySet.add(key.getString());
        }
        return outputKeySet;
    }

    public void readSLK(final File f) {
        try {
            readSLK(Files.newInputStream(f.toPath()));
        } catch (final IOException e) {
            throw new RuntimeException(e);
        }
    }

    public void readTXT(final InputStream txt, final boolean canProduce) throws IOException {
        if (txt == null) {
            return;
        }
        final BufferedReader reader = new BufferedReader(new InputStreamReader(txt, StandardCharsets.UTF_8));
        // BOM marker will only appear on the very beginning
        reader.mark(4);
        if ('\ufeff' != reader.read()) {
            reader.reset(); // not the BOM marker
        }

        String input;
        Element currentUnit = null;
        while ((input = reader.readLine()) != null) {
            if (DEBUG) {
                System.out.println(input);
            }
            if (input.startsWith("//")) {
                continue;
            }
            if (input.startsWith("[") && input.contains("]")) {
                final int start = input.indexOf("[") + 1;
                final int end = input.indexOf("]");
                final String newKey = input.substring(start, end);
                currentUnit = this.dataTable.get(new StringKey(newKey));
                if (currentUnit == null) {
                    currentUnit = new Element(newKey, this);
                    if (canProduce) {
                        currentUnit = new LMUnit(newKey, this);
                        this.dataTable.put(new StringKey(newKey), currentUnit);
                    }
                }
            } else if (input.contains("=")) {
                final int eIndex = input.indexOf("=");
                final String fieldValue = input.substring(eIndex + 1);
                final StringBuilder builder = new StringBuilder();
                boolean withinQuotedString = false;
                final String fieldName = input.substring(0, eIndex);
                boolean wasSlash = false;
                final List<String> values = new ArrayList<>();
                for (int i = 0; i < fieldValue.length(); i++) {
                    final char c = fieldValue.charAt(i);
                    final boolean isSlash = c == '/';
                    if (isSlash && wasSlash && !withinQuotedString) {
                        builder.setLength(builder.length() - 1);
                        break; // comment starts here
                    }
                    if (c == '\"') {
                        withinQuotedString = !withinQuotedString;
                    } else if (!withinQuotedString && (c == ',')) {
                        values.add(builder.toString().trim());
                        builder.setLength(0); // empty buffer
                    } else {
                        builder.append(c);
                    }
                    wasSlash = isSlash;
                }
                if (builder.length() > 0) {
                    if (currentUnit == null) {
                        System.out.println("null for " + input);
                    }
                    values.add(builder.toString().trim());
                }
                Objects.requireNonNull(currentUnit).setField(fieldName, values);
            }
        }

        reader.close();
    }

    public void readSLK(final InputStream txt) throws IOException {
        if (txt == null) {
            return;
        }
        final BufferedReader reader = new BufferedReader(new InputStreamReader(txt, StandardCharsets.UTF_8));

        String input;
        Element currentUnit = null;
        input = reader.readLine();
        if (!input.contains("ID")) {
            System.err.println("Formatting of SLK is unusual.");
        }
        input = reader.readLine();
        while (input.startsWith("P;") || input.startsWith("F;")) {
            input = reader.readLine();
        }
        final int yIndex = input.indexOf("Y") + 1;
        final int xIndex = input.indexOf("X") + 1;
        int colCount;
        boolean flipMode = false;
        if (xIndex > yIndex) {
            colCount = Integer.parseInt(input.substring(xIndex, input.lastIndexOf(";")));
        } else {
            colCount = Integer.parseInt(input.substring(xIndex, yIndex - 2));
            flipMode = true;
        }
        int rowStartCount = 0;
        String[] dataNames = new String[colCount];
        int lastFieldId = 0;
        while ((input = reader.readLine()) != null) {
            if (DEBUG) {
                System.out.println(input);
            }
            if (input.startsWith("E")) {
                break;
            }
            if (input.startsWith("O;")) {
                continue;
            }
            if (input.contains("X1;") || input.endsWith(";X1")) {
                rowStartCount++;
            }
            String kInput;
            if (input.startsWith("F;")) {
                kInput = reader.readLine();
                if (DEBUG) {
                    System.out.println(kInput);
                }
            } else {
                kInput = input;
            }
            if (rowStartCount <= 1) {
                final int subXIndex = input.indexOf("X");
                final int subYIndex = input.indexOf("Y");
                if ((subYIndex >= 0) && (subYIndex < subXIndex)) {
                    final int eIndex = kInput.indexOf("K");
                    final int fieldIdEndIndex = !kInput.equals(input) ? input.length() : eIndex - 1;
                    if ((eIndex == -1) || (kInput.charAt(eIndex - 1) != ';')) {
                        continue;
                    }
                    final int fieldId;
                    fieldId = Integer.parseInt(input.substring(subXIndex + 1, fieldIdEndIndex));

                    final int quotationIndex = kInput.indexOf("\"");
                    if ((fieldId - 1) >= dataNames.length) {
                        dataNames = Arrays.copyOf(dataNames, fieldId);
                    }
                    if (quotationIndex == -1) {
                        dataNames[fieldId - 1] = kInput.substring(eIndex + 1);
                    } else {
                        dataNames[fieldId - 1] = kInput.substring(quotationIndex + 1, kInput.lastIndexOf("\""));
                    }
                    lastFieldId = fieldId;
                } else {
                    int eIndex = kInput.indexOf("K");
                    if ((eIndex == -1) || (kInput.charAt(eIndex - 1) != ';')) {
                        continue;
                    }
                    final int fieldId;
                    if (subXIndex < 0) {
                        if (lastFieldId == 0) {
                            rowStartCount++;
                        }
                        fieldId = lastFieldId + 1;
                    } else {
                        if (flipMode && input.contains("Y") && (input.equals(kInput))) {
                            eIndex = Math.min(subYIndex, eIndex);
                        }
                        final int fieldIdEndIndex = !kInput.equals(input) ? input.length() : eIndex - 1;
                        fieldId = Integer.parseInt(input.substring(subXIndex + 1, fieldIdEndIndex));
                    }

                    final int quotationIndex = kInput.indexOf("\"");
                    if ((fieldId - 1) >= dataNames.length) {
                        dataNames = Arrays.copyOf(dataNames, fieldId);
                    }
                    if (quotationIndex == -1) {
                        dataNames[fieldId - 1] = kInput.substring(eIndex + 1);
                    } else {
                        dataNames[fieldId - 1] = kInput.substring(quotationIndex + 1, kInput.lastIndexOf("\""));
                    }
                    lastFieldId = fieldId;
                }
                continue;
            }
            if (input.contains("X1;") || ((!input.equals(kInput)) && input.endsWith("X1"))) {
                final int start = kInput.indexOf("\"") + 1;
                final int end = kInput.lastIndexOf("\"");
                if ((start - 1) != end) {
                    final String newKey = kInput.substring(start, end);
                    currentUnit = this.dataTable.get(new StringKey(newKey));
                    if (currentUnit == null) {
                        currentUnit = new Element(newKey, this);
                        this.dataTable.put(new StringKey(newKey), currentUnit);
                    }
                }
            } else if (kInput.contains("K")) {
                final int subXIndex = input.indexOf("X");
                int eIndex = kInput.indexOf("K");
                if (flipMode && kInput.contains("Y")) {
                    eIndex = Math.min(kInput.indexOf("Y"), eIndex);
                }
                final int fieldIdEndIndex = !kInput.equals(input) ? input.length() : eIndex - 1;
                final int fieldId = (subXIndex == -1) || (subXIndex > fieldIdEndIndex) ? 1
                        : Integer.parseInt(input.substring(subXIndex + 1, fieldIdEndIndex));
                String fieldValue = kInput.substring(eIndex + 1);
                if ((fieldValue.length() > 1) && fieldValue.startsWith("\"") && fieldValue.endsWith("\"")) {
                    fieldValue = fieldValue.substring(1, fieldValue.length() - 1);
                }
                if (dataNames[fieldId - 1] != null) {
                    Objects.requireNonNull(currentUnit).setField(dataNames[fieldId - 1], fieldValue);
                }
            }
        }

        reader.close();
    }

    @Override
    public Element get(final String id) {
        return this.dataTable.get(new StringKey(id));
    }

    @Override
    public void setValue(final String id, final String field, final String value) {
        get(id).setField(field, value);
    }

    public void put(final String id, final Element e) {
        this.dataTable.put(new StringKey(id), e);
    }
}