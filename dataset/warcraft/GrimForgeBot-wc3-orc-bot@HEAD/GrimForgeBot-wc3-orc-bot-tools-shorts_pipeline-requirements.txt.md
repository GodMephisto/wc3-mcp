---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/requirements.txt"
ref: HEAD
---

# GrimForge Shorts Pipeline — Python dependencies
# Install: pip install -r tools/shorts_pipeline/requirements.txt

# OBS WebSocket control
obsws-python>=1.7.0

# AI voice (free, no API key)
edge-tts>=6.1.0

# Script generation (Claude API)
anthropic>=0.25.0

# Auto-captions (local, M-series friendly)
faster-whisper>=1.0.0

# Video assembly (Python layer over FFmpeg)
# ffmpeg itself: brew install ffmpeg
moviepy>=2.0.0

# YouTube upload
google-api-python-client>=2.100.0
google-auth-httplib2>=0.2.0
google-auth-oauthlib>=1.2.0

# ElevenLabs (optional premium voice)
# elevenlabs>=1.0.0

# Thumbnail (already installed)
# pillow>=12.0.0
