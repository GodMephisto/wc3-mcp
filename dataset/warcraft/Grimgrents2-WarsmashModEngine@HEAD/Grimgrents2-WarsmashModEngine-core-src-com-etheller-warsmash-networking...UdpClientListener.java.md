---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/networking/udp/UdpClientListener.java"
ref: HEAD
---

package com.etheller.warsmash.networking.udp;

import java.nio.ByteBuffer;

public interface UdpClientListener {
	void parse(ByteBuffer buffer);
}
