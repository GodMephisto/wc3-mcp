---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/blizzard/casc/vfs/FileNode.java"
ref: HEAD
---

package com.hiveworkshop.blizzard.casc.vfs;

import java.util.List;

/**
 * A file system node containing a logical file.
 */
public class FileNode extends PathNode {
	private final StorageReference[] references;
	
	protected FileNode(final List<byte[]> pathFragments, final List<StorageReference> references) {
		super(pathFragments);
		this.references = references.toArray(new StorageReference[0]);
	}

	public int getFileReferenceCount() {
		return references.length;
	}
	
	public StorageReference getFileReference(final int index) {
		return references[index];
	}

}
