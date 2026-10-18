---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "main.go"
ref: HEAD
---

package main

import (
	"embed"
	"flag"
	"fmt"
	"log"
	"os"
	"os/signal"
	"syscall"

	"github.com/StephenSHorton/wc3-forge/internal/bridge"
	"github.com/StephenSHorton/wc3-forge/internal/forge"
	"github.com/StephenSHorton/wc3-forge/internal/mcpserver"
	"github.com/wailsapp/wails/v2"
	"github.com/wailsapp/wails/v2/pkg/options"
	"github.com/wailsapp/wails/v2/pkg/options/assetserver"
)

//go:embed all:frontend/dist
var assets embed.FS

// startupCameraSpec holds the value of --camera so App.startup can forward
// it to the frontend via Wails event. Set by main() before wails.Run is
// invoked; read by App.startup. Defaults to "" which the frontend treats as
// "no override; use whatever framing reloadMap chose".
var startupCameraSpec string

// startupPickSelfTest holds the value of --pick-self-test. When true, App.startup
// fires a "wc3-forge:pick-self-test" event after map-load; the JS side runs the
// project-every-instance-then-rayPick verification and logs results via flog.
// Pure diagnostic affordance — WebView2 drops synthetic mouse input, so the
// only way to drive a pick from an external test is to have the JS side run
// the test against its own state. Defaults to false.
var startupPickSelfTest bool

// AppVersion is the release version of this binary, injected at build time
// via -ldflags "-X main.AppVersion=<version>" (see .github/workflows/release.yml).
// It is empty for local/dev builds; GetAppVersion() reports "dev" in that case.
// The in-app updater compares this against the latest GitHub release.
var AppVersion string

// startupOpenError carries the failure from a --open / file-association /
// new-window startup map-load so App.startup can surface it to the window as a
// toast (rather than only logging it and launching a blank editor — a user who
// double-clicked a .w3x then couldn't tell "no map" from "corrupt map"). Set by
// main() before wails.Run; read + emitted once by App.startup after the JS side
// has subscribed. Empty means the --open either succeeded or wasn't requested.
var startupOpenError string

func main() {
	var headless bool
	var noBridge bool
	var openPath string
	var startReforged bool
	var cameraSpec string
	flag.BoolVar(&headless, "headless", false, "run the bridge only; do not open the GUI window")
	flag.BoolVar(&noBridge, "no-bridge", false, "skip starting the MCP bridge (for GUI-only dev)")
	flag.StringVar(&openPath, "open", "", "extracted map folder to load on startup (skips Open Map dialog)")
	flag.BoolVar(&startReforged, "reforged", false, "start in HD (Reforged) graphics mode; default is SD (Classic)")
	// Verification-automation hook: lets external test drivers pin the camera
	// to a known feature on startup without synthetic mouse input (which
	// WebView2 drops). Format: "x,y,z,distance" with z and distance optional.
	// Plumbed into the JS via a Wails event in App.startup so the frontend
	// can apply it on the first map-load.
	flag.StringVar(&cameraSpec, "camera", "", "initial camera spec 'x,y,z,distance' (z+distance optional)")
	var pickSelfTest bool
	flag.BoolVar(&pickSelfTest, "pick-self-test", false, "run a pick-correctness self-test after map load (logs to wc3-forge.log)")
	var mcpMode bool
	flag.BoolVar(&mcpMode, "mcp", false, "run as an MCP stdio server that proxies to a running wc3-forge; no GUI, no bridge of its own")
	flag.Parse()
	startupCameraSpec = cameraSpec
	startupPickSelfTest = pickSelfTest

	// --mcp: act as the MCP stdio server (the in-binary replacement for the Node
	// server in mcp/). This is a *client* of a separately-running editor — it
	// must NOT start the TCP bridge, write a lockfile, open CASC, or launch the
	// GUI, and nothing may write to stdout (the MCP channel). Branch before any
	// of that setup. Logs already go to wc3-forge.log via log.SetOutput below;
	// here we route fatals to stderr so they never corrupt the protocol stream.
	if mcpMode {
		if err := mcpserver.Run(); err != nil {
			fmt.Fprintf(os.Stderr, "wc3-forge --mcp: %v\n", err)
			os.Exit(1)
		}
		return
	}

	if startReforged {
		reforgedMode.Store(true)
	}

	log.SetFlags(log.LstdFlags | log.Lmicroseconds)
	log.SetPrefix("wc3-forge: ")
	// GUI Wails apps have no console; route Go logs to a file alongside
	// the binary so asset-handler diagnostics are visible.
	if logFile, err := os.OpenFile("wc3-forge.log", os.O_APPEND|os.O_CREATE|os.O_WRONLY, 0o644); err == nil {
		log.SetOutput(logFile)
	}

	// The forge package needs to read CASC + map assets to load the Object
	// Editor's stock tables. Wire it through readBaseAsset (the same
	// map-first-then-CASC lookup the typeindex / asset handler use) so
	// feature code in internal/forge doesn't have to import the asset
	// machinery directly. Set BEFORE bridge starts so the first MCP request
	// after startup already sees a live reader.
	forge.SetBaseAssetReader(readBaseAsset)

	// Bridge always runs unless explicitly disabled. The GUI and external MCP
	// clients can coexist — both read/write the same forge.Session.
	var b *bridge.Bridge
	if !noBridge {
		b = bridge.New()
		forge.RegisterAll(b)
		// minimap.bake + map.new live at the App/main layer (their fns depend on
		// top-level CASC-color / tileset-palette / minimap-bake helpers) —
		// register them after the forge catalog.
		registerMinimapBridge(b)
		registerMapNewBridge(b)
		if err := b.Start(); err != nil {
			log.Fatalf("bridge start failed: %v", err)
		}
		defer b.Stop()
		fmt.Printf("wc3-forge: bridge on 127.0.0.1:%d (pid %d)\n", b.Port(), os.Getpid())
	}

	// Open CASC eagerly so the viewer's first asset requests (which fire
	// during loadBaseFiles, immediately after the JS bundle initializes)
	// land on a ready storage. Lazy init introduces a race: the viewer's
	// loadBaseFiles can complete with errors before CASC finishes opening,
	// and the viewer doesn't retry on its own.
	if c, err := getCASC(); err != nil {
		log.Printf("CASC eager open failed (stock asset paths will 404): %v", err)
	} else if c != nil {
		// Sync the just-opened storage to the current reforged-mode atomic
		// (always false on startup; honored here in case --reforged or
		// similar persistent state lands later).
		c.SetReforged(ReforgedMode())
	}

	if openPath != "" {
		if err := forge.Current.Open(openPath); err != nil {
			log.Printf("--open %q failed: %v", openPath, err)
			// Stash the failure so App.startup can toast it to the window. The
			// Open error message already distinguishes the cases (stat → absent,
			// "has no war3map.w3i" → not a map, "parse …"/"map appears corrupt"
			// → corrupt/unsupported), so we forward it verbatim. Without this a
			// failed --open silently launched a blank editor + a 2 MB log line.
			startupOpenError = fmt.Sprintf("Couldn't open %s: %v", openPath, err)
		} else {
			log.Printf("--open %q OK", openPath)
		}
	}

	if headless {
		// Block on signals; no GUI to drive the event loop.
		sigs := make(chan os.Signal, 1)
		signal.Notify(sigs, syscall.SIGINT, syscall.SIGTERM)
		<-sigs
		fmt.Println("wc3-forge: shutting down")
		return
	}

	// GUI mode. wails.Run blocks until the window closes.
	app := NewApp()
	err := wails.Run(&options.App{
		// Initial title is shown for the brief window between Wails creating
		// the OS window and App.startup running refreshWindowTitle. Match the
		// composed format so users don't see the title change shape mid-launch.
		Title:  fmt.Sprintf("wc3-forge — PID %d", os.Getpid()),
		Width:  1600,
		Height: 1000,
		AssetServer: &assetserver.Options{
			Assets:  assets,
			Handler: newAssetHandler(),
		},
		BackgroundColour: &options.RGBA{R: 18, G: 18, B: 20, A: 1},
		OnStartup:        app.startup,
		// OnBeforeClose: gate the window-close on dirty state. When clean,
		// returns false → close proceeds. When dirty, emits a JS event so
		// App.svelte can show its "unsaved changes" modal and returns true
		// → this close is aborted. The user's choice (Save & Quit / Discard
		// & Quit / Cancel) drives App.SaveMap() + App.ForceQuit() or just
		// App.ForceQuit(). Returning true MUST be paired with a JS path
		// that can actually quit — App.ForceQuit exists for that.
		OnBeforeClose: app.onBeforeClose,
		Bind: []interface{}{
			app,
		},
	})
	if err != nil {
		log.Fatalf("wails.Run: %v", err)
	}
}
