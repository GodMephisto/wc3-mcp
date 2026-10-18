---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "internal/forge/triggers_addcategory_guard.go"
ref: HEAD
---

package forge

import (
	"sync"
	"time"
)

// Idempotency guard for triggers.add_category.
//
// Live-confirmed bug: a single triggers.add_category call produced TWO category
// nodes on disk (ids 50331649 + 50331650, same name written twice into
// war3map.wtg). The pure Go path (Session.AddTriggerCategory → wtg.Encode) is
// provably single-add (see triggers_fixes_test.go / the fixture + empty-tree
// probes), and the bridge dispatches each request exactly once. The duplication
// is therefore a double-INVOCATION at the boundary: the same logical add
// arriving twice in quick succession (a retried/echoed MCP request, or the same
// action racing in over two control surfaces). The two consecutive ids are the
// signature — allocTriggerID handed out N then N+1 for what the user meant as
// one add.
//
// The guard collapses a duplicate add_category that targets the SAME name under
// the SAME parent within addCategoryDedupeWindow of the previous one, returning
// the id of the category that was just created instead of allocating a second
// node. WC3 permits same-named categories, so we deliberately scope the dedupe
// to a tight time window: a user (or agent) who genuinely wants two same-named
// categories simply spaces the calls out, or renames — the common case of an
// accidental double-fire is suppressed without blocking intentional duplicates.
//
// Only the MCP handler routes through this guard. Session.AddTriggerCategory
// (used by the Wails GUI button and by tests directly) stays unguarded so its
// behavior is unchanged and surgical.

const addCategoryDedupeWindow = 600 * time.Millisecond

type addCategoryGuardState struct {
	mu       sync.Mutex
	session  *Session // the session the remembered add targeted (map switch invalidates)
	name     string
	parentID int32
	id       int32
	at       time.Time
	valid    bool
}

var addCategoryGuard addCategoryGuardState

// nowFn is overridable in tests.
var nowFn = time.Now

// dedupedAddTriggerCategory wraps Session.AddTriggerCategory with the
// short-window same-(name,parent) idempotency guard described above. Returns
// (id, deduped, err): deduped is true when the call was collapsed onto the
// immediately-preceding identical add (no new node was created).
func dedupedAddTriggerCategory(name string, parentID int32) (int32, bool, error) {
	addCategoryGuard.mu.Lock()
	defer addCategoryGuard.mu.Unlock()

	cur := Current
	now := nowFn()
	if addCategoryGuard.valid &&
		addCategoryGuard.session == cur &&
		addCategoryGuard.name == name &&
		addCategoryGuard.parentID == parentID &&
		now.Sub(addCategoryGuard.at) <= addCategoryDedupeWindow {
		// Same add as a moment ago, same session — treat as the duplicate of a
		// single logical action. Refresh the timestamp so a 3rd echo is also
		// caught, but do not allocate a new node.
		addCategoryGuard.at = now
		return addCategoryGuard.id, true, nil
	}

	id, err := cur.AddTriggerCategory(name, parentID)
	if err != nil {
		// Don't poison the guard on failure.
		addCategoryGuard.valid = false
		return 0, false, err
	}
	addCategoryGuard.session = cur
	addCategoryGuard.name = name
	addCategoryGuard.parentID = parentID
	addCategoryGuard.id = id
	addCategoryGuard.at = now
	addCategoryGuard.valid = true
	return id, false, nil
}

// resetAddCategoryGuard clears the dedupe memory. Exposed for tests that need a
// clean slate between cases.
func resetAddCategoryGuard() {
	addCategoryGuard.mu.Lock()
	addCategoryGuard.valid = false
	addCategoryGuard.mu.Unlock()
}
