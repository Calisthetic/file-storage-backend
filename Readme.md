# File storage backend

## Project

### Docs

[Database in dbdiagram.io](https://dbdiagram.io/d/64bd4ad502bd1c4a5e8bf005)

## Dev

### Useful links

[API Gateway Ocelot](https://ocelot.readthedocs.io/en/latest/features/authorization.html)
[XML tags](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags)
[Documentation comments](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments)

### Guides

<details>
<summary><h4>Tutorials<h4></summary>

[connect to psql](https://youtu.be/z7G6HV7WWz0?si=cHTbdEOE16KJ5W0O)
[read user claims](https://youtu.be/7vqAHD9DlIA?si=KhJ1cYMce9Fa0GRs)
[Quartz background jobs](https://youtu.be/iD3jrj3RBuc?si=wSZ_Okv8HND8j9cA)
[Ocelot with swagger](https://mahedee.net/configure-swagger-on-api-gateway-using-ocelot-in-asp.net-core-application/)

</details>

### Commands

```bash
Add-Migration ""
```

```bash
Update-Database
```
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(_configuration.GetSection("ConnectionStrings:DatabaseConnection").Value!);

```bash
Scaffold-DbContext "Data Source=DESKTOP-GJJERNN;Initial Catalog=FileStorageFeedback;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models/Db -force
```
