---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/rms/editor/render3d/GLTestMain.java"
ref: HEAD
---

package com.hiveworkshop.rms.editor.render3d;

import java.awt.BorderLayout;
import java.awt.Frame;

import javax.swing.SwingUtilities;

import com.badlogic.gdx.backends.lwjgl.LwjglNativesLoader;

public class GLTestMain {

	public static void main(String[] args) {
		LwjglNativesLoader.load();
		SwingUtilities.invokeLater(new Runnable() {
			@Override
			public void run() {
				
				Frame frame = new Frame("GL Test");
				
				frame.setLayout(new BorderLayout());
				try {
					frame.add(new GLTestCanvas());
				} catch (Exception e) {
					throw new RuntimeException(e);
				}
				frame.pack();
				frame.setLocationRelativeTo(null);
				frame.setVisible(true);
			}
		});
	}
}
