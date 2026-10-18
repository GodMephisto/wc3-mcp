---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/wurstio/languageserver/requests/RequestFailedException.java"
ref: HEAD
---

package de.peeeq.wurstio.languageserver.requests;

import org.eclipse.lsp4j.MessageType;

/**
 *
 */
public class RequestFailedException extends RuntimeException {
    private final MessageType messageType;

    public RequestFailedException(MessageType messageType, String s) {
        super(s);
        this.messageType = messageType;
    }

    public RequestFailedException(MessageType messageType, String s, Throwable ex) {
        super(s + "\n" + ex, ex);
        this.messageType = messageType;
    }

    public MessageType getMessageType() {
        return messageType;
    }
}
