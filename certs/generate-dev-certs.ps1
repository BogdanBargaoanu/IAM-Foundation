param(
    [string]$OutputDir = (Join-Path $PSScriptRoot "."),
    [string]$PfxPassword = "certpass",
    [int]$Days = 3650
)

$ErrorActionPreference = "Continue"

$gitOpenSslExe = "C:\Program Files\Git\usr\bin\openssl.exe"

function Resolve-OpenSslPath {
    if (Test-Path -LiteralPath $gitOpenSslExe) {
        return $gitOpenSslExe
    }

    $cmd = Get-Command "openssl" -ErrorAction SilentlyContinue
    if ($null -ne $cmd) {
        return $cmd.Source
    }

    throw "OpenSSL not found. Expected '$gitOpenSslExe' or 'openssl' available on PATH."
}

$openSsl = Resolve-OpenSslPath
Write-Host "Using OpenSSL: $openSsl"

# CA Setup
$caDir = Join-Path $OutputDir "ca"
New-Item -ItemType Directory -Force -Path $caDir | Out-Null

$caKey = Join-Path $caDir "iam-dev-ca.key"
$caCrt = Join-Path $caDir "iam-dev-ca.crt"

Write-Host "Generating dev CA..."
& $openSsl genrsa -out $caKey 4096 2>&1 | Out-Null
& $openSsl req -x509 -new -nodes -key $caKey -sha256 -days $Days `
    -subj "/CN=iam-dev-ca" -out $caCrt 2>&1 | Out-Null

Write-Host "CA Generated: $caCrt"

# Function to Generate Service Certs
function New-ServiceCert {
    param (
        [string]$ServiceName,
        [string[]]$DnsNames
    )

    $serviceDir = Join-Path $OutputDir $ServiceName
    New-Item -ItemType Directory -Force -Path $serviceDir | Out-Null

    $serverKey = Join-Path $serviceDir "$ServiceName.key"
    $serverCsr = Join-Path $serviceDir "$ServiceName.csr"
    $serverCrt = Join-Path $serviceDir "$ServiceName.crt"
    $serverPfx = Join-Path $serviceDir "$ServiceName.pfx"
    $extFile   = Join-Path $serviceDir "$ServiceName.ext"

    # Build the SANs string (e.g., DNS.1=name, DNS.2=localhost)
    $dnsList = ""
    $i = 1
    foreach ($dns in $DnsNames) {
        $dnsList += "DNS.$i=$dns`n"
        $i++
    }

    @"
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage=digitalSignature,keyEncipherment
extendedKeyUsage=serverAuth
subjectAltName=@alt_names

[alt_names]
$dnsList
"@ | Set-Content -NoNewline -Path $extFile -Encoding ascii

    Write-Host "Processing [$ServiceName]..."
    
    # Generate Key and CSR
    & $openSsl genrsa -out $serverKey 2048 2>&1 | Out-Null
    & $openSsl req -new -key $serverKey -subj "/CN=$ServiceName" -out $serverCsr 2>&1 | Out-Null

    # Sign with CA
    & $openSsl x509 -req -in $serverCsr -CA $caCrt -CAkey $caKey -CAcreateserial `
        -out $serverCrt -days $Days -sha256 -extfile $extFile 2>&1 | Out-Null

    # Export PFX
    if (Test-Path -LiteralPath $serverPfx) { Remove-Item -LiteralPath $serverPfx -Force }
    & $openSsl pkcs12 -export -out $serverPfx -inkey $serverKey -in $serverCrt `
        -certfile $caCrt -password ("pass:$PfxPassword") 2>&1 | Out-Null

    Write-Host "  -> Created PFX: $serverPfx"
}

# Generate Certs for Services

# Identity Service
New-ServiceCert -ServiceName "identity" -DnsNames @("identity", "localhost")

# Transactions API Service
New-ServiceCert -ServiceName "transactions-api" -DnsNames @("transactions-api", "localhost")

Write-Host ""
Write-Host "Done."