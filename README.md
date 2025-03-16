# HomeIMS

Simple *Inventory Management System* for home usage.

[![CI](https://github.com/PhilKTurner/home-ims/actions/workflows/CI.yml/badge.svg)](https://github.com/PhilKTurner/home-ims/actions/workflows/CI.yml)

## Getting Started (kind of)

Provide passwords as UTF-8 text files in ./.secrets in the local repository:

- hims-rootpw
- hims-db-rootpw
- hims-db-userpw
- hims-eventstore-rootpw

Build images and run containers:

```
docker-compose up --build --detach
```
