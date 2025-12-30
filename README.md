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
