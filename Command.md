BigFileSynchronizer — CLI Commands:

init
Description:
Initializes the project by creating:
	.bfs/config.json
	.bfs/drive_links.json
	.git/hooks/pre-push

Usage:
BigFileSynchronizer.exe init
--
auth <path-to-service_account.json>
Description:
Authorizes Google Drive access via a service account.
Copies service_account.json into .bfs/
Activates the service account
Allows saving a Google Drive folder ID (optional)
Usage:
BigFileSynchronizer.exe auth path/to/service_account.json
--
push

Description:
Scans the project for new or modified large assets
Archives them
Uploads them to Google Drive
Updates .bfs/drive_links.json
Usage:
BigFileSynchronizer.exe push
--
pull

Description:
Checks for missing or corrupted local assets
Downloads and restores them from Google Drive archives
Usage:
BigFileSynchronizer.exe pull
--
scan

Description:
Scans the project
Displays all large assets found according to the current configuration
Allows updating the paths in config.json (planned)
Usage:
BigFileSynchronizer.exe scan
--
reset

Description:
Deletes generated files and folders:
.bfs/
build/
Git hooks
Does NOT delete the running .exe
Usage:
BigFileSynchronizer.exe reset
--
help

Description:
Displays all available commands and short descriptions.
Usage:
BigFileSynchronizer.exe help