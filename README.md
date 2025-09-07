# SecureAPI (.NET 8 + PostgreSQL, HTTPS, parameterized access)

Minimal Web API with secure DB access. HTTPS-only. No SQL concatenation. Least privilege.

## Structure
# SecureAPI (.NET 8 + PostgreSQL, HTTPS, parameterized access)

Minimal Web API with secure DB access. HTTPS-only. No SQL concatenation. Least privilege.

## Structure


## Prerequisites
- .NET SDK 8
- PostgreSQL 16 (local) or Docker
- cURL

## Database (create once)
Connect as an admin (psql/DBeaver) to the **target database** and run:
```sql
CREATE ROLE app_user LOGIN PASSWORD '***' NOSUPERUSER NOCREATEDB NOCREATEROLE;
CREATE DATABASE appdb OWNER app_user;
\c appdb
CREATE TABLE IF NOT EXISTS public.app_users(
  id SERIAL PRIMARY KEY,
  email TEXT NOT NULL UNIQUE,
  password_hash TEXT NOT NULL
);
GRANT SELECT, INSERT, UPDATE ON public.app_users TO app_user;
GRANT USAGE, SELECT ON SEQUENCE public.app_users_id_seq TO app_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT SELECT, INSERT, UPDATE ON TABLES TO app_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT USAGE, SELECT ON SEQUENCES TO app_user;
```
## Configuration (no secrets in repo)
## This is the approach that I took
```
    cd SecureAPI.Api
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:App" "<connection-string>"
```

## Run HTTPS
```
dotnet dev-certs https --trust
dotnet run --project SecureAPI.Api
# Expect: Now listening on: https://localhost:5001
```

## Endpoints
```
curl -k https://localhost:5001/health

curl -k -H "Content-Type: application/json" \
  -d '{"email":"alice@example.com","passwordHash":"hash"}' \
  https://localhost:5001/api/users

curl -k https://localhost:5001/api/users/1

curl -k "https://localhost:5001/api/users/search?q=' OR 1=1 --"   # should not dump data
```

## Use Postman to hit the API Endpoints
- Create Enviroment: SecureAPI Local.
    -- Variable abseUsrl = https://localhost:5001
- Settings -> General -> turn  SSL certificate Verification off for localhost if needed

## Requests in Postman
1. Health
- GET {{baseUrl}}/health
- Expect {"ok":true}
2. Create user
- POST {{baseUrl}}/api/users
- Headers: Content-Type: application/json
- Body (raw JSON):
    ```{ "email": "a@b.com", "passwordHash": "hash" }```
- Expect 201 Created with {"id": <number>}
3. Get by id
- GET {{baseUrl}}/api/users/1
- Expect 200 OK with {"id":1,"email":"a@b.com"}
4. Search
- GET {{baseUrl}}/api/users/search?q=a
- Expect list with the user.
- Probe: {{baseUrl}}/api/users/search?q=' OR 1=1 -- should return empty list, not dump data.


## Security notes
- HTTPS + HSTS enabled.
- Parameterized queries only.
- Least-privilege DB role.
- Local DB can use non-TLS; remote/prod must use TLS verification.
- Do not log request/response bodies.

