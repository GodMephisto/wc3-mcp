---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/gui/GuiUtils.java"
ref: HEAD
---

package de.peeeq.wurstio.gui;

import java.awt.*;

public class GuiUtils {

    public static void setWindowToCenterOfScreen(Window frm) {
        Rectangle screenBounds = frm.getGraphicsConfiguration().getBounds();

        int center_x = screenBounds.x + screenBounds.width / 2;
        int center_y = screenBounds.y + screenBounds.height / 2;

        frm.setLocation(center_x - frm.getWidth() / 2,
                center_y - frm.getHeight() / 2);
    }


}
