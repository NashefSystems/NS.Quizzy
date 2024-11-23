# NS.Quizzy project 👋
## 🚀 DB migration
### 💻 Run in **Package manager console**
#### ⚡ To install dotnet-ef:
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

#### ➕ To create a new migration:
```bash
dotnet ef migrations add "Initialization DataBase" -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### ❌ To remove all migrations:
```bash
dotnet ef migrations remove -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\ -y
```

#### To add a new migration:
```
dotnet ef migrations add <<MigrationName>> -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### To generate migration script:
```
dotnet ef migrations script <<PreviousMigrationName>> <<NewMigrationName>> -o ScriptName_MigrationScript.sql -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### To get migrations list:
```
dotnet ef migrations list -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\
```

#### To apply the migrations:
```
dotnet ef database update -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\ --configuration Debug
dotnet ef database update -s .\NS.Quizzy.Server\ -p .\NS.Quizzy.Server.DAL\ --configuration Release
```