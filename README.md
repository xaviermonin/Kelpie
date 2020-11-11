# Kelpie

This library is experimental.

## Features

- Entity Framework based and fully compatible (v6 only currently).
- Dynamic model (entities, fields, ...) generation from metadata tables. 
- Metadata/model manipulation. Thats mean you can CRUD your own entities and allows, for exemple, custom fields support.
- Runtime usage of entites. Using of entities without know their name and fields at runtime (with dynamic/reflection).
- Static usage of entites at compile time via classes proxy.
- Auto creation of metadata tables.

## How to use

### Load
#### Dynamic/Runtime
```csharp
using (var context = Context.New("Name=ConnectionStringName"))
{
  var users = context.Set("User") as IEnumerable<dynamic>;
  var xavier = users.Where(u => u.FirstName == "Xavier").Single();
}
```
#### With proxies

```csharp

using (var context = Context.New("Name=ConnectionStringName"))
{
  var xavier = context.ProxySet<IUser>() // IUser must be declared in metadata
                      .Where(u => u.FirstName == "Xavier")
                      .Single();
}
```
