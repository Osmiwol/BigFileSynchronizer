# bfsgit Command Reference

## init
Initializes the project by creating:
- .config_bfs/config.json
- .config_bfs/drive_links.json
- .git/hooks/pre-push

**Usage:**
```
bfsgit.exe init
```

---

## auth <path-to_service_account.json>
Authorizes Google Drive access via a service account.
- Copies `service_account.json` into `.config_bfs/`
- Activates the service account
- Requires you to enter a Google Drive folder ID (root is NOT supported!)

**Usage:**
```
bfsgit.exe auth path/to/service_account.json
```

---

## push
Scans the project for new or modified large assets, archives them, uploads to Google Drive, and updates `.config_bfs/drive_links.json`.

**Usage:**
```
bfsgit.exe push
```

---

## pull
Checks for missing or corrupted local assets. Downloads and restores them from Google Drive archives.

**Usage:**
```
bfsgit.exe pull
```

---

## scan
Scans the project and displays all large assets found according to the current configuration.

**Usage:**
```
bfsgit.exe scan
```

---

## reset
Deletes generated files and folders:
- `.config_bfs/`
- `bfs_cache/`
- git hooks

**Does NOT delete the running .exe**

**Usage:**
```
bfsgit.exe reset
```

---

## help
Displays all available commands and short descriptions.

**Usage:**
```
bfsgit.exe help
```