---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/networking/udp/OrderedUdpServerListener.java"
ref: HEAD
---

package com.etheller.warsmash.networking.udp;

import java.net.SocketAddress;

public interface OrderedUdpServerListener extends UdpServerListener {
	void cantReplay(SocketAddress sourceAddress, int seqNo);
}
