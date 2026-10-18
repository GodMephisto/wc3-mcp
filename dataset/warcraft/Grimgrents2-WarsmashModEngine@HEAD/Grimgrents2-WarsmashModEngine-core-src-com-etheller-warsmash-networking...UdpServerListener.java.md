---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/networking/udp/UdpServerListener.java"
ref: HEAD
---

package com.etheller.warsmash.networking.udp;

import java.net.SocketAddress;
import java.nio.ByteBuffer;

public interface UdpServerListener {
	void parse(SocketAddress sourceAddress, ByteBuffer buffer);
}
