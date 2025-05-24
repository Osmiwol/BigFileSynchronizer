# bfsgit (BigFileSynchronizer)

**bfsgit** is a cross-platform CLI utility for game developers that automatically uploads large assets (fbx, png, wav, mp3, etc.) from your project to Google Drive during `git push`, keeping your git history clean.

---

## 🚀 Getting Started

1. **Initialize your git project (if needed):**
    ```bash
    git init
    ```

2. **Initialize bfsgit in your project:**
    ```bash
    bfsgit.exe init
    ```

3. **Authorize Google Drive (service account):**
    ```bash
    bfsgit.exe auth path/to/service_account.json
    ```
    > When prompted, enter the **Google Drive folder ID**.  
    > **This must be a folder you created in "My Drive" and shared with your service account as "Editor".**  
    > Root is **not** supported!

4. **Add your large assets as usual:**
    - You don't need to commit big files to git, just keep them in your project folders (e.g., `Assets/`, `StreamingAssets/`, etc.)

5. **Push your changes:**
    ```bash
    git push
    ```
    > bfsgit will automatically scan, archive, and upload your large assets before every push.

---

## 📁 Project Structure

```
/your-game-project/
│
├── .config_bfs/
│   ├── config.json
│   ├── drive_links.json
│   ├── service_account.json
│
├── bfs_cache/          # Temporary ZIP archives for upload/restore
├── .git/hooks/pre-push # bfsgit auto-sync hook
├── bfsgit.exe
├── README.md
└── .gitignore
```

---

## 🛠 Main CLI Commands

- `bfsgit.exe init`  
  Initialize the project, create `.config_bfs/`, set up git hook, default config.

- `bfsgit.exe auth path/to/service_account.json`  
  Authorize service account and set Google Drive folder ID (required).

- `bfsgit.exe push`  
  Scan, archive, and upload new/changed assets to Google Drive.

- `bfsgit.exe pull`  
  Restore missing assets from cloud archives.

- `bfsgit.exe scan`  
  Show what assets would be archived/uploaded.

- `bfsgit.exe reset`  
  Delete `.config_bfs/`, `bfs_cache/`, and git hooks. Full clean/reset.

- `bfsgit.exe help`  
  Show all commands and usage.

---

## ☁️ Google Drive Setup

- **See detailed setup in `SETUP_GOOGLE_DRIVE.md`**
- Only folder IDs from "My Drive" are supported.
- Service account **must** be shared as "Editor" on that folder.
- `root` is **not supported**.

---

## ℹ️ Notes

- Works on Windows (`bfsgit.exe`) and Linux (`bfsgit`).
- Your git history stays clean — only metadata in git, big files go to the cloud.
- MVP: Google Drive only. Other clouds (Yandex Disk, Dropbox, S3) are planned for future versions.

---

## 🤝 Contributing & Feedback

- For bug reports or feature requests, open an [Issue](https://github.com/Osmiwol/BigFileSynchronizer/issues).
- For suggestions or code contributions, submit a Pull Request.
- If you like the project — star the repo and share your experience!

---

## 📝 License

MIT — use, modify, and distribute freely.

---

**Happy game dev!**