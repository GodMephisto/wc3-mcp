---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/blizzard/casc/info/FieldDataType.java"
ref: HEAD
---

package com.hiveworkshop.blizzard.casc.info;

/**
 * Field data types to help with decoding values.
 */
public enum FieldDataType {
	/**
	 * Field contains textual data. Size ignored.
	 */
	STRING,
	/**
	 * Field is a decimal number. Size determines number of bytes used to represent
	 * it.
	 */
	DEC,
	/**
	 * Field is a hexadecimal string. Size is number of bytes used to represent it
	 * with every 2 characters representing 1 byte.
	 */
	HEX,
	/**
	 * This field type is currently not supported.
	 */
	UNSUPPORTED
}
