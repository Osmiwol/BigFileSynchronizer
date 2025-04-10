# BigFileSynchronizer

> Automatically uploads large game assets (models, audio, textures, etc.) to cloud storage (Google Drive) during `git push`.  
> Designed for Unity, extendable to Godot, Unreal, and other engines.

---

## 🔧 Why this tool?

Game projects often contain huge files (FBX, MP3, PNG, WAV) that shouldn't be versioned in Git.  
BigFileSynchronizer offloads them to the cloud during push — so your repository stays fast and clean.

No Git LFS, no Dropbox hacking, no manual steps.

---

## ✅ Features (MVP)

- 🔗 Integrates with `git` via `pre-push` hook
- 🔍 Scans your Unity project for large assets
- 📦 Archives and uploads only new or changed files
- ☁️ Google Drive backend
- 📝 Tracks uploaded files in local cache (`drive_links.json`)
- ⚙️ Configurable via `BigFileSynchronizer.config.json`

---

## 🚀 Getting started

1. **Clone your Unity project**  
   (or create one with `git init`)

2. **Download and run the synchronizer**  
   ```bash
   BigFileSynchronizer.exe init
   ```

3. **Push as usual**  
   ```bash
   git push
   ```

   The tool will scan, archive, and upload large assets before push.

---

## 📁 Example config (auto-created)

```json
{
  "project": "My Unity Game",
  "cloud": "GoogleDrive",
  "paths": [ "Assets/", "StreamingAssets/" ],
  "archiveFormat": "zip",
  "maxArchiveSizeMB": 750,
  "minFileSizeMB": 5,
  "includeExtensions": [
    ".fbx", ".obj", ".png", ".jpg", ".wav", ".mp3", ".txt", ".json"
  ]
}
```

---

## 📦 Roadmap

- [x] Git hook integration (`pre-push`)
- [x] Local file scanner with extension & size filter
- [x] Archive creation (`.zip`)
- [x] Mock uploader with cache
- [ ] Real Google Drive integration
- [ ] `pull` command to restore missing assets
- [ ] Support for Godot / Unreal / GameMaker
- [ ] GUI version (Pro)
- [ ] Multi-cloud support (Dropbox, S3, etc.)

---

## 📜 License

MIT — use it, modify it, integrate it.  
Made by [@osmiwol](https://github.com/osmiwol)

---

## 🇷🇺 Русская версия

**BigFileSynchronizer** — это инструмент, который автоматически загружает крупные игровые ассеты (3D-модели, музыку, текстуры и т.д.) в облачное хранилище (Google Drive) при `git push`.

Разработано для Unity, в будущем будет расширено до Godot, Unreal и других движков.

### Возможности:

- Интеграция через git `pre-push` хук
- Сканирование ассетов по расширениям и размеру
- Архивирование только новых/изменённых файлов
- Загрузка в облако
- Хранение информации о загруженных файлах
- Настройка через `BigFileSynchronizer.config.json`

### Как начать:

```bash
git init
BigFileSynchronizer.exe init
git push
```

### Пример конфига:

```json
{
  "paths": [ "Assets/", "StreamingAssets/" ],
  "minFileSizeMB": 5,
  "includeExtensions": [".fbx", ".png", ".wav"]
}
```

---

[Back to English version ↑](#bigfilesynchronizer)
