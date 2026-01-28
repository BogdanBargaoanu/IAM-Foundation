# IAM-Foundation

A minimal implementation for **Identity and Access Management** featuring **Duende Identity Server** and OAuth2.0 authentication flows.

It features the following projects:
- `Identity` (Duende Identity Server)
- `MvcDemo` (MVC Web App) - primary role: display user information based on issued `access_token`.
> The `MvcDemo` is authorized to the `TransactionsApi` and can consume it.
- `TransactionsApi` (ASP.NET Web API) - primary role: serves transactions info to authenticated clients.
> Note: `TransactionsApi` enforces a `AccountOwner` policy for every other client consuming it besides the `TransactionsClient`, which limits users to altering transaction data to their authentication scope.
- `TransactionsClient` (MVC Web App) - primary role: consumes `TransactionsApi` to display transaction data and request metadata.
- `TransactionsLibrary` (Class Library) - primary role: centralized models and constants.
- `TransactionsApiClient` (Class Library) - primary role: exposes an `ITransactionsApiClient` to be consumed by the applications.
---
## Authorization Code Flow
The **Authorization Code Flow** will be showcased using the `Identity` and `MvcDemo`.

For **Api Access**, `TransactionsApi` will be used and accessible via the `Transactions` page in `MvcDemo`.
### How to Run Locally

#### 1. Clone the Repository
```bash
git clone https://github.com/BogdanBargaoanu/IAM-Foundation.git
```

#### 2. Configure Database

Open the `appsettings.json` files in the `Identity` and in the `TransactionsApi` projects and configure the connection string to point to your local SQL Server instance.

#### 3. Seed the Database

Navigate to the Identity project directory and run the following command to seed the database:

```bash
dotnet run /seed

```

#### 4. Launch the Applications

Run the projects using the **Development** launch profile.

---

### 🐳 How to Run with Docker

To run the solution in Docker containers, follow these configuration steps using a PowerShell terminal.

#### 1. Generate Developer Certificates

Open a PowerShell terminal at the **root** of the project and run the following commands to export and trust the certificate:

```powershell
dotnet dev-certs https -ep ./certs/aspnetapp.pfx -p Your_cert_password
dotnet dev-certs https --trust

```

#### 2. Generate Certified Authority, Identity and TransactionsApi Certificates

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

#### 3. Configure Environment Variables

1. Create a `.env` file at the **root** of the project based on the `.env.example` file.
2. Fill in the following variables:
* `SA_PASSWORD` (The password used for the SQL Instance to be run in the `db` container)
* `CERT_PASSWORD` (The password used in the previous steps)
* `OIDC_CLIENT_ID` (From your Identity Client configuration)
* `OIDC_CLIENT_SECRET` (From your Identity Client configuration)

#### 4. Build and Run Containers

Run the following commands at the root of the project:

```bash
docker-compose build --no-cache
docker-compose up
```

### 🌐 Access the Applications

Once the containers are running, access the applications at the following URLs:

* `Identity`: [https://localhost:5001](https://localhost:5001)
* `MvcDemo`: [https://localhost:7151](https://localhost:7151)
* `TransactionsApi`: [https://localhost:7001/swagger](https://localhost:7001/swagger)

---
## Extension Grants
A custom **Kiosk Login** Extension Grant is implemented via `KioskAuthenticationGrantValidator.cs` with the `grant_type=kiosk_auth`.
> This extension grant is intended to authenticate users providing an `employee_id` and `pin`.

To access the functionality you will need to make a `POST` request to the `/connect/token` endpoint of the `Identity`:
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
## Client Credentials Flow
For the **Client Credentials Flow**, the following projects will be involved: `Identity`, `TransactionsApi`, `TransactionsClient`.

### How to Run Locally
For steps **1**, **2**, and **3**, follow the instructions given in the [Authorization Code Flow](#authorization-code-flow) section.
> Note: A client for the transactions functionality is already configured in the seeding step.

#### 4. Launch the Applications

Run all projects using the **Transactions** launch profile.

### Manual Testing

If you want to manually test the **Client Credentials Flow**, you can request an `access_token` by making a `POST` request to the `/connect/token` endpoint of the `Identity`:
```bash
curl -k -X POST https://localhost:5001/connect/token
     -H "Content-Type: application/x-www-form-urlencoded"
     -d "client_id=transactions.client"
     -d "client_secret=Transactions_client_secret"
     -d "grant_type=client_credentials"
```

After obtaining the `access_token`, you can use the **Swagger** interface of the `TransactionsApi` to make requests, or do them in you preferred manner. 
* `TransactionsApi`: [https://localhost:7001/swagger](https://localhost:7001/swagger)

---
## Postman
A postman collection is included in the repository for testing the **Kiosk Login** external grant and the **Client Credentials Flow**. 

Download the [postman collection](https://github.com/BogdanBargaoanu/IAM-Foundation/blob/main/utils/IAM-Foundation.postman_collection.json), import it in the app and modify the `baseUrl` to the `Identity` URL , `kiosk_secret` to the Kiosk `client_secret` and respectively the `transactions_secret` to the Transactions `client_secret` in the collection variables.

---
