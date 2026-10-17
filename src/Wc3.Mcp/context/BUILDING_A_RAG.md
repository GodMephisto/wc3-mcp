# The Warcraft III context and RAG

Two ways to give a model the Warcraft III knowledge. The built-in one needs nothing but the install, the other is for a bigger or private corpus.

## Built in, the server does the retrieval

The `wc3-mcp` server serves the bundled documents two ways, so a client configures nothing.

- MCP resources, the deterministic core. `wc3://context/abilities` and `wc3://context/ui-frames` return a whole document. Per the MCP spec a knowledge server is Resources and Prompts, so this is the intended path, read a document rather than hope a search returns the right chunk.
- `docs_search`, which finds a passage inside them. Lexical to start, and semantic once the index is installed.

Install the semantic index, one command.

```
wc3ctl rag install
```

It fetches `BAAI/bge-small-en-v1.5` once, about 130 MB, and builds an index over the bundled context. After that `docs_search` is semantic and auto-connected, and a client still configures nothing. The MCP tools `rag_install` and `rag_status` do the same from a client.

### Tuning

The install is a one time job and it is set low on CPU by default, four threads with the OpenMP pool in passive wait rather than every core spinning. Raise it to build faster.

- `--threads N` CPU threads for embedding. Default `min(cores/2, 4)`. `W3C_RAG_THREADS` sets it from the environment.
- `--batch N` chunks per inference, default 16. Larger finishes sooner and uses more memory.
- `--corpus <dir>` the folder of `.md` to index, default the bundled context.

```
wc3ctl rag install --threads 8 --batch 32 --corpus "C:/my-notes"
```

Measured on 606 chunks, four threads batch 16 is 1m34s and eight threads batch 32 is 1m25s, so the low CPU default costs little wall clock and a machine is not pinned unless the user asks.

### The graph

`rag install` also builds a graph beside the index, nodes for a document and its sections, and edges for a document to its section, a document to one it links, and a document to one it is close to by embedding. The MCP exposes it as `graph_neighbors`, `graph_hubs` and `graph_path`, and the CLI as `rag neighbors`, `rag hubs` and `rag path`. No server, the graph is a local file queried in process. A Neo4j backend, for Cypher and vault parity, is a later option behind a flag, fed from these same nodes and edges rather than a second extraction.

## Build your own, for a bigger or private corpus

For thousands of documents, or a set a person adds their own maps and notes to, a local RAG of your own is the right shape. One machine, no account, its only cost is disk and the model.

1. Python 3.11 or newer, one venv.
2. `pip install sentence-transformers faiss-cpu`, or the llama-index path. Free and offline after the first model pull.
3. A small embedding model, the same `BAAI/bge-small-en-v1.5` the server uses, so results match.
4. Chunk by heading, never by fixed length alone. A chunk spanning two headings embeds the average of two ideas and retrieves neither. Late chunking, embedding the document then pooling per chunk, is the current best practice.
5. Store the vectors in FAISS, or SQLite for zero operation. A few hundred thousand chunks fit in memory.
6. Serve it as a function a local agent calls, an HTTP endpoint, or an MCP server of its own.

Failure points to design around. Index the whole document, not a summary, or missing content can never be retrieved. Pick the embedding model for the domain and test on real questions, or the top chunk is missed. Keep the retrieved passage verbatim, do not paraphrase it on the way in. A prompt that cites the passage and refuses to answer from memory fixes most of the not extracted case. Keep the structure, tables and headings, because flattening to prose loses meaning.

## The corpus

The public documents worth indexing are the format specifications, the tutorials, and the two references in the context folder. A private RAG adds the map corpus, your own notes and any tooling docs, and the same recipe applies at any size.

## Licensing

These are community documents. Index them for personal use freely, and keep the attribution and the source with every chunk so a citation can name the author.
