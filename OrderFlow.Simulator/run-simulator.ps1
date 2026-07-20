$root = Split-Path -Parent $PSScriptRoot

Set-Location $root

Write-Host ""
Write-Host "========================================"
Write-Host " Building OrderFlow Simulator..."
Write-Host "========================================"

dotnet publish OrderFlow.Simulator -c Release

if ($LASTEXITCODE -ne 0)
{
    Write-Host "Erro no build."
    exit
}

Write-Host ""
Write-Host "========================================"
Write-Host " Running Simulator..."
Write-Host "========================================"

.\OrderFlow.Simulator\bin\Release\net10.0\publish\OrderFlow.Simulator.exe