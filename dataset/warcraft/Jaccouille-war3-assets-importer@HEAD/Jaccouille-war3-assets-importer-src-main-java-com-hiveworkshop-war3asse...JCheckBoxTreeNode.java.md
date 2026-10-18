---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "src/main/java/com/hiveworkshop/war3assetsimporter/gui/JCheckBoxTreeNode.java"
ref: HEAD
---

package com.hiveworkshop.war3assetsimporter.gui;

import javax.swing.tree.DefaultMutableTreeNode;

/**
 * Tree node with tri-state checkbox support (checked / unchecked / partial).
 * Used by {@link JCheckBoxTree}.
 */
public class JCheckBoxTreeNode extends DefaultMutableTreeNode {

    private boolean checked;
    private boolean allChildrenSelected;
    private boolean hasPersonalState; // NYI, do not use

    public JCheckBoxTreeNode() {
        super();
    }

    public JCheckBoxTreeNode(Object userObject, boolean checked) {
        super(userObject);
        this.checked = checked;
    }

    public JCheckBoxTreeNode(Object userObject) {
        super(userObject);
    }

    public JCheckBoxTreeNode(boolean checked) {
        super();
        this.checked = checked;
    }

    public boolean isChecked() {
        return checked;
    }

    public void setChecked(boolean checked) {
        this.checked = checked;
    }

    public boolean isAllChildrenSelected() {
        return allChildrenSelected;
    }

    public void setAllChildrenSelected(boolean v) {
        this.allChildrenSelected = v;
    }

    // NYI, buggy — do not use
    public boolean isHasPersonalState() {
        return hasPersonalState;
    }

    public void setHasPersonalState(boolean v) {
        this.hasPersonalState = v;
    }
}
