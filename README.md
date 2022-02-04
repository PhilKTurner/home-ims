# HomeIMS

Simple *Inventory Management System* for home usage.

## Getting Started (kind of)

Provide passwords used to setup the MariaDB container as UTF-8 text files in ./.secrets

- hims-mariadb-pw
- hims-mariadb-rootpw

Build image for build environment:
```
docker build -f build-env.dockerfile -t hims-build-env .
```

Build images and run containers for application and backend:

```
docker-compose --env-file ./.env.dev --profile backend --profile app up -d
```
