param(
    [string]$OutputDir = (Join-Path $PSScriptRoot "."),
    [string]$PfxPassword = "certpass",
    [int]$Days = 3650
)

$ErrorActionPreference = "Stop"

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

$caDir = Join-Path $OutputDir "ca"
$identityDir = Join-Path $OutputDir "identity"

New-Item -ItemType Directory -Force -Path $caDir | Out-Null
New-Item -ItemType Directory -Force -Path $identityDir | Out-Null

$caKey = Join-Path $caDir "iam-dev-ca.key"
$caCrt = Join-Path $caDir "iam-dev-ca.crt"

$serverKey = Join-Path $identityDir "identity.key"
$serverCsr = Join-Path $identityDir "identity.csr"
$serverCrt = Join-Path $identityDir "identity.crt"
$serverPfx = Join-Path $identityDir "identity.pfx"
$extFile = Join-Path $identityDir "identity.ext"

@"
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage=digitalSignature,keyEncipherment
extendedKeyUsage=serverAuth
subjectAltName=@alt_names

[alt_names]
DNS.1=identity
DNS.2=localhost
"@ | Set-Content -NoNewline -Path $extFile -Encoding ascii

Write-Host "Using OpenSSL: $openSsl"
Write-Host "Generating dev CA..."
& $openSsl genrsa -out $caKey 4096 | Out-Null
& $openSsl req -x509 -new -nodes -key $caKey -sha256 -days $Days `
    -subj "/CN=iam-dev-ca" -out $caCrt | Out-Null

Write-Host "Generating Identity server key/csr..."
& $openSsl genrsa -out $serverKey 2048 | Out-Null
& $openSsl req -new -key $serverKey -subj "/CN=identity" -out $serverCsr | Out-Null

Write-Host "Signing server cert with dev CA..."
& $openSsl x509 -req -in $serverCsr -CA $caCrt -CAkey $caKey -CAcreateserial `
    -out $serverCrt -days $Days -sha256 -extfile $extFile | Out-Null

Write-Host "Creating PFX for Kestrel..."
if (Test-Path -LiteralPath $serverPfx) { Remove-Item -LiteralPath $serverPfx -Force }
& $openSsl pkcs12 -export -out $serverPfx -inkey $serverKey -in $serverCrt `
    -certfile $caCrt -password ("pass:$PfxPassword") | Out-Null

Write-Host ""
Write-Host "Done."
Write-Host "CA cert:       $caCrt"
Write-Host "Server cert:   $serverCrt"
Write-Host "Server PFX:    $serverPfx"
Write-Host ""
Write-Host "Next: import the CA cert into your OS trust (Windows) and rebuild containers."