---
type: vendor-doc
source_repo: "https://github.com/GrimForgeBot/wc3-orc-bot"
source_path: "tools/shorts_pipeline/upload_youtube.py"
ref: HEAD
---

"""YouTube upload — schedule and publish the Short via YouTube Data API v3.

Setup (one-time):
    1. Go to console.cloud.google.com → create project "GrimForge"
    2. Enable YouTube Data API v3
    3. Create OAuth 2.0 credentials (Desktop App)
    4. Download JSON → .secrets/yt_client_secrets.json
    5. Run once with --auth to generate .secrets/yt_token.json:
           python upload_youtube.py --auth

Requires:
    pip install google-api-python-client google-auth-httplib2 google-auth-oauthlib
"""
from __future__ import annotations
import datetime
import json
import sys
from pathlib import Path

from tools.shorts_pipeline.config import (
    YT_CLIENT_SECRETS, YT_TOKEN_FILE,
    YT_CATEGORY_GAMING, YT_DEFAULT_TAGS, YT_DEFAULT_PRIVACY,
)

_SCOPES = ["https://www.googleapis.com/auth/youtube.upload"]


def _get_authenticated_service():
    from google.oauth2.credentials import Credentials           # type: ignore
    from google_auth_oauthlib.flow import InstalledAppFlow      # type: ignore
    from googleapiclient.discovery import build                  # type: ignore
    from google.auth.transport.requests import Request           # type: ignore

    creds = None
    if YT_TOKEN_FILE.exists():
        creds = Credentials.from_authorized_user_file(str(YT_TOKEN_FILE), _SCOPES)

    if not creds or not creds.valid:
        if creds and creds.expired and creds.refresh_token:
            creds.refresh(Request())
        else:
            if not YT_CLIENT_SECRETS.exists():
                raise FileNotFoundError(
                    f"YouTube client secrets not found: {YT_CLIENT_SECRETS}\n"
                    "See setup instructions at top of this file."
                )
            flow  = InstalledAppFlow.from_client_secrets_file(str(YT_CLIENT_SECRETS), _SCOPES)
            creds = flow.run_local_server(port=0)
        YT_TOKEN_FILE.parent.mkdir(parents=True, exist_ok=True)
        YT_TOKEN_FILE.write_text(creds.to_json())

    return build("youtube", "v3", credentials=creds)


def upload_short(
    session_dir: Path,
    title: str,
    description: str = "",
    publish_at: datetime.datetime | None = None,
    privacy: str = YT_DEFAULT_PRIVACY,
    tags: list[str] | None = None,
) -> str:
    """
    Upload short.mp4 from session_dir to YouTube.

    Args:
        session_dir:   session folder containing short.mp4 + thumbnail.jpg
        title:         video title (keep ≤70 chars)
        description:   video description (add Ko-fi + GitHub links here)
        publish_at:    scheduled publish datetime (UTC); None = upload as-is
        privacy:       "private" | "unlisted" | "public"
        tags:          tag list; defaults to YT_DEFAULT_TAGS

    Returns:
        YouTube video URL
    """
    from googleapiclient.http import MediaFileUpload  # type: ignore

    video_path = session_dir / "short.mp4"
    thumb_path = session_dir / "thumbnail.jpg"

    if not video_path.exists():
        raise FileNotFoundError(f"short.mp4 missing in {session_dir}")

    tags = tags or YT_DEFAULT_TAGS

    # Build status block
    status: dict = {"privacyStatus": privacy}
    if publish_at is not None:
        # YouTube requires RFC 3339 format
        status["publishAt"] = publish_at.strftime("%Y-%m-%dT%H:%M:%S.0Z")
        status["privacyStatus"] = "private"   # must be private for scheduled publish

    body = {
        "snippet": {
            "title":       title,
            "description": description,
            "tags":        tags,
            "categoryId":  YT_CATEGORY_GAMING,
        },
        "status": status,
    }

    yt = _get_authenticated_service()

    print(f"  [Upload] uploading {video_path.name} ({video_path.stat().st_size // 1000} KB)...")
    request = yt.videos().insert(
        part="snippet,status",
        body=body,
        media_body=MediaFileUpload(str(video_path), chunksize=-1, resumable=True),
    )

    response = None
    while response is None:
        status_up, response = request.next_chunk()
        if status_up:
            pct = int(status_up.progress() * 100)
            print(f"  [Upload] {pct}%", end="\r")

    video_id  = response["id"]
    video_url = f"https://youtu.be/{video_id}"
    print(f"  [Upload] done → {video_url}")

    # Set thumbnail if available
    if thumb_path.exists():
        try:
            yt.thumbnails().set(
                videoId=video_id,
                media_body=MediaFileUpload(str(thumb_path)),
            ).execute()
            print(f"  [Upload] thumbnail set")
        except Exception as e:
            print(f"  [Upload] thumbnail upload failed (need verified channel): {e}")

    # Save video URL to session
    (session_dir / "youtube_url.txt").write_text(video_url)
    return video_url


if __name__ == "__main__":
    if "--auth" in sys.argv:
        print("Running OAuth flow — a browser window will open.")
        _get_authenticated_service()
        print(f"Token saved → {YT_TOKEN_FILE}")
        sys.exit(0)

    if len(sys.argv) < 3:
        print("Usage: python upload_youtube.py <session_dir> \"Video Title\" [publish_at_UTC]")
        print("       python upload_youtube.py --auth   # one-time OAuth setup")
        sys.exit(1)

    publish = None
    if len(sys.argv) > 3:
        publish = datetime.datetime.fromisoformat(sys.argv[3])

    url = upload_short(Path(sys.argv[1]), title=sys.argv[2], publish_at=publish)
    print(f"URL: {url}")
