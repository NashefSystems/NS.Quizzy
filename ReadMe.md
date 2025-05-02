# NS.Quizzy project 👋
## 🚀 DB migration
### 💻 Run in **Package manager console**
#### ⚡ To install dotnet-ef:
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

#### ➕ To add a new migration:
```bash
dotnet ef migrations add "Add devices table" -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### ❌ To remove last migrations:
```bash
dotnet ef migrations remove -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\ -y
```

#### To generate migration script:
```
dotnet ef migrations script "20250418111313_Remove IsSeen property" "20250419085214_changes in Notifications table" -o ScriptName_MigrationScript.sql -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### To get migrations list:
```
dotnet ef migrations list -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### To apply the migrations:
```
$env:DOTNET_ENVIRONMENT="Development"
dotnet ef database update -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\ --configuration Debug

$env:DOTNET_ENVIRONMENT="Release"
dotnet ef database update -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\ --configuration Release
```