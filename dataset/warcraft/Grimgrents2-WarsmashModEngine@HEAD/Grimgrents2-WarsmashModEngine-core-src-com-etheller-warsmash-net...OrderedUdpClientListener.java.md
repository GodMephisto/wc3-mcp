---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/networking/udp/OrderedUdpClientListener.java"
ref: HEAD
---

package com.etheller.warsmash.networking.udp;

public interface OrderedUdpClientListener extends UdpClientListener {
	void cantReplay(int seqNo);
}
