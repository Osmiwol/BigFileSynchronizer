📅 SETUP_GOOGLE_DRIVE.md (BigFileSynchronizer)
🔧 How to set up Google Drive access
1. Create a Google Cloud Project
Go to Google Cloud Console
Create a new project (name it like BigFileSync)

2. Enable Google Drive API
In the Cloud Console, navigate to "APIs & Services" > "Library"
Search for Google Drive API
Click "Enable"

3. Create a Service Account
Go to "IAM & Admin" > "Service Accounts"
Click "Create Service Account"
Name it (e.g., bigfilesync-bot)
Grant the role: Editor
Click Done

4. Create and download the service_account.json
Select your service account
Go to the "Keys" tab
Click "Add Key" > "Create New Key"
Choose JSON
Download the service_account.json file

5. Share your Google Drive folder
Create a folder on your Google Drive (e.g., BigFileSyncAssets)
Right-click > "Share"
Add your service account email (client_email from your .json) as Editor

6. Obtain the Folder ID
Open your Drive folder
Copy the folderId from the URL:
Example:
https://drive.google.com/drive/folders/1aBcD3EfGhIjKlMnOpQ
Folder ID = 1aBcD3EfGhIjKlMnOpQ

7. Configure BigFileSynchronizer
Place service_account.json into the .bfs/ directory
Run:
BigFileSynchronizer.exe auth path/to/service_account.json

When prompted, paste the Folder ID.

That's it! Now your heavy assets will be safely synced to Google Drive!

🌐 РУССКАЯ ВЕРСИЯ

🔧 Как настроить доступ к Google Drive
1. Создание проекта Google Cloud
Зайдите в Google Cloud Console
Создайте новый проект ("BigFileSync")

2. Включите Google Drive API
Перейдите в "APIs & Services" > "Library"
Найдите Google Drive API
Нажмите "Enable"

3. Создание Service Account
В разделе "IAM & Admin" > "Service Accounts"
Нажмите "Create Service Account"
Дайте имя (напр., bigfilesync-bot)
Присвойте роль: Editor
Нажмите Done

4. Скачайте service_account.json
Выберите service account
В таблице "Keys"
Нажмите "Add Key" > "Create New Key", выберите JSON
Скачайте файл

5. Предоставьте доступ
Создайте папку на Google Drive ("BigFileSyncAssets")
Правая кнопка мыши > "Предоставить доступ"
Добавьте email из client_email (из .json) как Editor

6. Найдите ID папки
Откройте папку в браузере
URL вида:
https://drive.google.com/drive/folders/1aBcD3EfGhIjKlMnOpQ
ID папки = 1aBcD3EfGhIjKlMnOpQ

7. Настройте BigFileSynchronizer
Поместите service_account.json в .bfs/
Выполните:
BigFileSynchronizer.exe auth path/to/service_account.json
При запросе вставьте ID папки.
🔹 Все! BigFileSynchronizer будет автоматически загружать ассеты в облако!