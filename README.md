# HomeIMS

Simple *Inventory Management System* for home usage.

[![CI](https://github.com/PhilKTurner/home-ims/actions/workflows/CI.yml/badge.svg)](https://github.com/PhilKTurner/home-ims/actions/workflows/CI.yml)

## Getting Started (kind of)

Provide passwords as UTF-8 text files in ./.secrets in the local repository:

- hims-rootpw
- hims-db-rootpw
- hims-db-userpw
- hims-eventstore-rootpw

Provide HTTPS certificate in folder `~/.aspnet/https`. See [Generate self-signed certificates with the .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/self-signed-certificates-guide#create-a-self-signed-certificate) for reference.

Build images and run containers:

```
docker-compose up --build --detach
```
