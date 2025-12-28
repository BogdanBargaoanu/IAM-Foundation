#!/bin/bash
set -e

echo "Running database seed..."
dotnet Identity.dll /seed

echo "Starting Identity Server..."
exec dotnet Identity.dll