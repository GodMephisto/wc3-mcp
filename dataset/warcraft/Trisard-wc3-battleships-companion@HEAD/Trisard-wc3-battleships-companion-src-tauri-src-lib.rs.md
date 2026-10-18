---
type: vendor-doc
source_repo: "https://github.com/Trisard/wc3-battleships-companion"
source_path: "src-tauri/src/lib.rs"
ref: HEAD
---

use tauri::Emitter;

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .setup(|app| {
            let app_handle = app.handle().clone();
            std::thread::spawn(move || {
                loop {
                    // Try to connect to the WC3 local API
                    let handle = match wc3_local_api::connect_auto() {
                        Ok(h) => h,
                        Err(e) => {
                            // Notify the frontend that we are disconnected with the reason
                            let error_str = match e {
                                wc3_local_api::Wc3Error::ProcessNotFound => "ProcessNotFound".to_string(),
                                wc3_local_api::Wc3Error::CannotOpenProcess => "CannotOpenProcess".to_string(),
                                wc3_local_api::Wc3Error::GuidNotFound => "GuidNotFound".to_string(),
                                wc3_local_api::Wc3Error::PortNotFound => "PortNotFound".to_string(),
                                _ => format!("{:?}", e),
                            };
                            let _ = app_handle.emit("wc3-status", format!("Disconnected:{}", error_str));
                            std::thread::sleep(std::time::Duration::from_secs(3));
                            continue;
                        }
                    };

                    let _ = app_handle.emit("wc3-status", "Connected");

                    // Monitor connection status changes on the handle
                    let status_rx = handle.status;
                    let status_app_handle = app_handle.clone();
                    let status_thread = std::thread::spawn(move || {
                        for status in status_rx {
                            match status {
                                wc3_local_api::ConnectionStatus::Connected { port } => {
                                    let _ = status_app_handle.emit("wc3-status", format!("Connected:{}", port));
                                }
                                wc3_local_api::ConnectionStatus::Disconnected => {
                                    let _ = status_app_handle.emit("wc3-status", "Disconnected");
                                }
                                wc3_local_api::ConnectionStatus::Reconnecting => {
                                    let _ = status_app_handle.emit("wc3-status", "Reconnecting");
                                }
                                wc3_local_api::ConnectionStatus::Connecting { port } => {
                                    let _ = status_app_handle.emit("wc3-status", format!("Connecting:{}", port));
                                }
                            }
                        }
                    });

                    // Forward all WebSocket messages to the frontend
                    for msg in handle.messages {
                        let _ = app_handle.emit("wc3-message", msg);
                    }

                    // If we break out of the message loop, connection was lost or closed
                    let _ = app_handle.emit("wc3-status", "Disconnected");
                    let _ = status_thread.join();
                    std::thread::sleep(std::time::Duration::from_secs(3));
                }
            });
            Ok(())
        })
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
