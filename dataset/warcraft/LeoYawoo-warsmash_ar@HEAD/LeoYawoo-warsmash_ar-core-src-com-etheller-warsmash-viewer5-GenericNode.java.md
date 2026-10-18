---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/GenericNode.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import com.badlogic.gdx.math.Matrix4;
import com.badlogic.gdx.math.Quaternion;
import com.badlogic.gdx.math.Vector3;

import java.util.List;

public abstract class GenericNode {

    public Vector3 pivot;
    public Vector3 localLocation;
    public Quaternion localRotation;
    public Vector3 localScale;
    public Vector3 worldLocation;
    public Quaternion worldRotation;
    public Vector3 worldScale;
    public Vector3 inverseWorldLocation;
    public Quaternion inverseWorldRotation;
    public Vector3 inverseWorldScale;
    public Matrix4 localMatrix;
    public Matrix4 worldMatrix;
    public GenericNode parent;
    public List<Node> children;
    public boolean dontInheritTranslation;
    public boolean dontInheritRotation;
    public boolean dontInheritScaling;
    public boolean visible;
    public boolean wasDirty;
    public boolean dirty;

    protected abstract void update(float dt, Scene scene);
}
