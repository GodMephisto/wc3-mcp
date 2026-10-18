---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/networking/ClientToServerProtocol.java"
ref: HEAD
---

package com.etheller.warsmash.networking;

public class ClientToServerProtocol {

	public static final int ISSUE_TARGET_ORDER = 1;
	public static final int ISSUE_POINT_ORDER = 2;
	public static final int ISSUE_DROP_ITEM_ORDER = 3;
	public static final int ISSUE_DROP_ITEM_ON_TARGET_ORDER = 9;
	public static final int ISSUE_IMMEDIATE_ORDER = 4;
	public static final int UNIT_CANCEL_TRAINING = 5;
	public static final int FINISHED_TURN = 6;
	public static final int JOIN_GAME = 7;
	public static final int FRAMES_SKIPPED = 8;
}
