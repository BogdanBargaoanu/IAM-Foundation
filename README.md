# IAM-Foundation

A minimal implementation featuring **Duende Identity Server** and a simple **MVC application** for viewing user information.

## How to Run Locally

### 1. Clone the Repository
```bash
git clone https://github.com/BogdanBargaoanu/IAM-Foundation.git
```

### 2. Configure Database

Open the `appsettings.json` file in the Identity project and configure the connection string to point to your local SQL Server instance.

### 3. Seed the Database

Navigate to the Identity project directory and run the following command to seed the database:

```bash
dotnet run /seed

```

### 4. Launch the Applications

Run both projects using the **Development** launch profile.

---

## 🐳 How to Run with Docker

To run the solution in Docker containers, follow these configuration steps using a PowerShell terminal.

### 1. Generate Developer Certificates

Open a PowerShell terminal at the **root** of the project and run the following commands to export and trust the certificate:

```powershell
dotnet dev-certs https -ep ./certs/aspnetapp.pfx -p Your_cert_password
dotnet dev-certs https --trust

```

### 2. Generate Certified Authority and Identity Certificates

In the same PowerShell terminal, navigate to the `certs` directory and run the generation script.
*Note: The default password is `certpass` if not specified.*

```powershell
cd certs
.\generate-dev-certs.ps1 -PfxPassword Your_cert_password
```
> You may experience the error: `Execution of scripts is disabled on this system`.
>
> To fix, open a new powershell as `administrator` and run the following command: `Set-ExecutionPolicy RemoteSigned`.
>
> Afterwards, you can revert the policy change with: `Set-ExecutionPolicy Restricted`.
### 3. Configure Environment Variables

1. Create a `.env` file at the **root** of the project based on the `.env.example` file.
2. Fill in the following variables:
* `SA_PASSWORD` (The password used for the SQL Instance to be run in the `db` container)
* `CERT_PASSWORD` (The password used in the previous steps)
* `OIDC_CLIENT_ID` (From your Identity Client configuration)
* `OIDC_CLIENT_SECRET` (From your Identity Client configuration)



### 4. Build and Run Containers

Run the following commands at the root of the project:

```bash
docker-compose build --no-cache
docker-compose up
```
---
## 🌐 Access the Applications

Once the containers are running, access the applications at the following URLs:

* **IdentityServer:** [https://localhost:5001](https://localhost:5001)
* **MVC Client:** [https://localhost:7151](https://localhost:7151)

## Extension Grants
A custom **Kiosk Login** Extension Grant is implemented via `KioskAuthenticationGrantValidator.cs` with the `grant_type=kiosk_auth`.
> This extension grant is intended to authenticate users providing an `employee_id` and `pin`.

To access the functionality you will need to make a `POST` request to the `/connect/token` endpoint of the `IdentityServer`:
```bash
curl -k -X POST https://localhost:5001/connect/token
     -H "Content-Type: application/x-www-form-urlencoded"
     -d "client_id=kiosk.client"
     -d "client_secret=Kiosk_client_secret"
     -d "grant_type=kiosk_auth"
     -d "employee_id=Your_employee_id"
     -d "pin=Your_employee_pin"
```

> Note: A Kiosk employee is already configured in the seeding step.
> 
> For testing purposes, you can replace `employee_id` with `123456` and `pin` with `1234`.
---
