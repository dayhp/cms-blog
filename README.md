# cms-blog
CMS Blog technical source code

# Using dotnet ef migration DB
1. cd D:\C-SHAP\Roadmap\CMS\cms-blog\src\CMSBlog.Data
2. dotnet ef migrations add Initial
3. dotnet ef migrations script -o script.sql
4. dotnet ef database update