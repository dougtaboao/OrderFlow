#requires -Version 5.1

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root = $PSScriptRoot
$output = Join-Path $root "OrderFlow-Inventario-Arquitetural.md"

$ignoredDirectories = @(
    ".git", ".vs", ".idea", "bin", "obj", "logs",
    "coverage-report", "TestResults", "node_modules", "artifacts", "packages"
)

function Test-IgnoredPath {
    param([string]$FullName)

    foreach ($directory in $ignoredDirectories) {
        $pattern = "[\\/]" + [regex]::Escape($directory) + "([\\/]|$)"
        if ($FullName -match $pattern) { return $true }
    }

    return $false
}

function Get-RelativePath {
    param([string]$BasePath, [string]$TargetPath)

    $base = $BasePath.TrimEnd("\") + "\"
    $baseUri = New-Object System.Uri($base)
    $targetUri = New-Object System.Uri($TargetPath)
    $relative = $baseUri.MakeRelativeUri($targetUri).ToString()
    return [System.Uri]::UnescapeDataString($relative.Replace("/", "\"))
}

function Get-ProjectName {
    param([string]$FilePath)

    $directory = Split-Path -Parent $FilePath

    while ($directory -and $directory.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        $project = Get-ChildItem -Path $directory -Filter *.csproj -File -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($project) { return [System.IO.Path]::GetFileNameWithoutExtension($project.Name) }

        $parent = Split-Path -Parent $directory
        if ($parent -eq $directory) { break }
        $directory = $parent
    }

    return "(sem projeto identificado)"
}

function Remove-Comments {
    param([string]$Content)

    $withoutBlocks = [regex]::Replace(
        $Content,
        "/\*.*?\*/",
        "",
        [System.Text.RegularExpressions.RegexOptions]::Singleline
    )

    return [regex]::Replace(
        $withoutBlocks,
        "^\s*//.*$",
        "",
        [System.Text.RegularExpressions.RegexOptions]::Multiline
    )
}

function Get-Namespace {
    param([string]$Content)

    $match = [regex]::Match($Content, "(?m)^\s*namespace\s+([A-Za-z_][A-Za-z0-9_.]*)\s*[;{]")
    if ($match.Success) { return $match.Groups[1].Value }
    return "(namespace nao identificado)"
}

function Get-TypeDeclarations {
    param([string]$Content)

    $pattern = "(?m)^\s*(?:public|internal|protected|private)?\s*(?:abstract\s+|sealed\s+|static\s+|partial\s+)*" +
               "(class|interface|record|struct|enum)\s+([A-Za-z_][A-Za-z0-9_]*)" +
               "(?:\s*<[^>{}]+>)?\s*(?:\:\s*([^{\r\n]+))?"

    return [regex]::Matches($Content, $pattern)
}

function Get-ConstructorDependencies {
    param([string]$Content, [string]$TypeName)

    $escapedType = [regex]::Escape($TypeName)
    $pattern = "(?s)(?:public|internal|protected|private)\s+" + $escapedType + "\s*\((.*?)\)\s*(?:\{|:)"
    $match = [regex]::Match($Content, $pattern)

    if (-not $match.Success) { return @() }

    $parameters = $match.Groups[1].Value -split ","
    $result = New-Object System.Collections.Generic.List[string]

    foreach ($parameter in $parameters) {
        $clean = ($parameter -replace "\s+", " ").Trim()
        if ([string]::IsNullOrWhiteSpace($clean)) { continue }

        $clean = $clean -replace "^(?:\[.*?\]\s*)+", ""
        $clean = $clean -replace "^(?:this\s+|ref\s+|out\s+|in\s+|params\s+)", ""
        $clean = $clean -replace "\s*=\s*.*$", ""

        $parts = $clean -split "\s+"
        if ($parts.Count -ge 2) {
            $type = ($parts[0..($parts.Count - 2)] -join " ")
            $name = $parts[$parts.Count - 1]
            $result.Add($type + " " + $name)
        }
        else {
            $result.Add($clean)
        }
    }

    return $result.ToArray()
}

function Get-PublicMethods {
    param([string]$Content, [string]$TypeName)

    $pattern = "(?m)^\s*public\s+(?:(?:async|static|virtual|override|sealed|new)\s+)*" +
               "([A-Za-z_][A-Za-z0-9_<>,?.\[\]\s]*)\s+" +
               "([A-Za-z_][A-Za-z0-9_]*)\s*\(([^)]*)\)"

    $matches = [regex]::Matches($Content, $pattern)
    $result = New-Object System.Collections.Generic.List[string]

    foreach ($match in $matches) {
        $returnType = ($match.Groups[1].Value -replace "\s+", " ").Trim()
        $methodName = $match.Groups[2].Value.Trim()
        $parameters = ($match.Groups[3].Value -replace "\s+", " ").Trim()

        if ($methodName -eq $TypeName) { continue }

        $signature = $returnType + " " + $methodName + "(" + $parameters + ")"
        if (-not $result.Contains($signature)) { $result.Add($signature) }
    }

    return $result.ToArray()
}

function Get-Category {
    param([string]$Project, [string]$RelativePath)

    if ($Project -match "\.Tests$|IntegrationTests") { return "Tests" }

    if ($Project -match "\.Api$") {
        if ($RelativePath -match "Controllers") { return "API - Controllers" }
        if ($RelativePath -match "Middlewares") { return "API - Middlewares" }
        if ($RelativePath -match "Security") { return "API - Security" }
        if ($RelativePath -match "Settings") { return "API - Settings" }
        return "API - Other"
    }

    if ($Project -match "\.Application$") {
        if ($RelativePath -match "UseCases") { return "Application - Use Cases" }
        if ($RelativePath -match "Validators") { return "Application - Validators" }
        if ($RelativePath -match "Strategies") { return "Application - Strategies" }
        if ($RelativePath -match "Services") { return "Application - Services" }
        if ($RelativePath -match "Messaging") { return "Application - Messaging" }
        if ($RelativePath -match "Observability") { return "Application - Observability" }
        if ($RelativePath -match "Interfaces") { return "Application - Interfaces" }
        if ($RelativePath -match "Dtos") { return "Application - DTOs" }
        return "Application - Other"
    }

    if ($Project -match "\.Domain$") {
        if ($RelativePath -match "Entities") { return "Domain - Entities" }
        if ($RelativePath -match "Interfaces") { return "Domain - Interfaces" }
        if ($RelativePath -match "Exceptions") { return "Domain - Exceptions" }
        if ($RelativePath -match "Enums") { return "Domain - Enums" }
        if ($RelativePath -match "ReadModels") { return "Domain - Read Models" }
        if ($RelativePath -match "Common") { return "Domain - Common" }
        return "Domain - Other"
    }

    if ($Project -match "\.Infrastructure$") {
        if ($RelativePath -match "Repositories") { return "Infrastructure - Repositories" }
        if ($RelativePath -match "Messaging") { return "Infrastructure - Messaging" }
        if ($RelativePath -match "Cache") { return "Infrastructure - Cache" }
        if ($RelativePath -match "HealthChecks") { return "Infrastructure - Health Checks" }
        if ($RelativePath -match "Observability") { return "Infrastructure - Observability" }
        if ($RelativePath -match "Gateways") { return "Infrastructure - Gateways" }
        if ($RelativePath -match "Data") { return "Infrastructure - Data" }
        return "Infrastructure - Other"
    }

    if ($Project -match "\.Worker$") { return "Worker" }
    if ($Project -match "\.Grpc") { return "gRPC" }
    if ($Project -match "\.Simulator$") { return "Simulator" }

    return "Other"
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$writer = New-Object System.IO.StreamWriter($output, $false, $utf8NoBom)

function Write-Line {
    param([string]$Text = "")
    $script:writer.WriteLine($Text)
}

try {
    Write-Line "# OrderFlow - Inventario Arquitetural"
    Write-Line
    Write-Line ("> Gerado em " + (Get-Date -Format "yyyy-MM-dd HH:mm:ss") + ".")
    Write-Line
    Write-Line "> Analise textual automatica dos arquivos C#."
    Write-Line "> O resultado deve ser validado contra o codigo-fonte."
    Write-Line

    $sourceFiles = Get-ChildItem -Path $root -Recurse -Filter *.cs -File |
        Where-Object { -not (Test-IgnoredPath -FullName $_.FullName) } |
        Sort-Object FullName

    $items = New-Object System.Collections.Generic.List[object]

    foreach ($file in $sourceFiles) {
        $rawContent = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        $content = Remove-Comments -Content $rawContent
        $namespace = Get-Namespace -Content $content
        $project = Get-ProjectName -FilePath $file.FullName
        $relative = Get-RelativePath -BasePath $root -TargetPath $file.FullName
        $declarations = Get-TypeDeclarations -Content $content

        foreach ($declaration in $declarations) {
            $kind = $declaration.Groups[1].Value
            $typeName = $declaration.Groups[2].Value
            $baseTypesText = $declaration.Groups[3].Value.Trim()
            $baseTypes = @()

            if (-not [string]::IsNullOrWhiteSpace($baseTypesText)) {
                $baseTypes = $baseTypesText -split "," |
                    ForEach-Object { $_.Trim() } |
                    Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
            }

            $dependencies = Get-ConstructorDependencies -Content $content -TypeName $typeName
            $methods = Get-PublicMethods -Content $content -TypeName $typeName
            $category = Get-Category -Project $project -RelativePath $relative

            $items.Add([pscustomobject]@{
                Category = $category
                Project = $project
                RelativePath = $relative
                Namespace = $namespace
                Kind = $kind
                TypeName = $typeName
                BaseTypes = $baseTypes
                Dependencies = $dependencies
                Methods = $methods
            })
        }
    }

    Write-Line "## Resumo"
    Write-Line
    Write-Line ("- Arquivos C# analisados: " + $sourceFiles.Count)
    Write-Line ("- Tipos encontrados: " + $items.Count)
    Write-Line ("- Projetos encontrados: " + (($items.Project | Sort-Object -Unique).Count))
    Write-Line

    Write-Line "## Projetos"
    Write-Line

    foreach ($projectName in ($items.Project | Sort-Object -Unique)) {
        $count = @($items | Where-Object { $_.Project -eq $projectName }).Count
        Write-Line ("- " + $projectName + ": " + $count + " tipos")
    }

    Write-Line
    Write-Line "## Indice por categoria"
    Write-Line

    foreach ($categoryName in ($items.Category | Sort-Object -Unique)) {
        Write-Line ("### " + $categoryName)
        Write-Line

        $categoryItems = $items | Where-Object { $_.Category -eq $categoryName } | Sort-Object TypeName
        foreach ($item in $categoryItems) {
            Write-Line ("- " + $item.TypeName + " (" + $item.Kind + ")")
        }

        Write-Line
    }

    Write-Line "## Catalogo detalhado"
    Write-Line

    foreach ($categoryName in ($items.Category | Sort-Object -Unique)) {
        Write-Line ("# " + $categoryName)
        Write-Line

        $categoryItems = $items | Where-Object { $_.Category -eq $categoryName } | Sort-Object Project, TypeName

        foreach ($item in $categoryItems) {
            Write-Line ("## " + $item.TypeName)
            Write-Line
            Write-Line ("- Projeto: " + $item.Project)
            Write-Line ("- Tipo: " + $item.Kind)
            Write-Line ("- Namespace: " + $item.Namespace)
            Write-Line ("- Arquivo: " + $item.RelativePath)

            if ($item.BaseTypes.Count -gt 0) {
                Write-Line "- Herda ou implementa:"
                foreach ($baseType in $item.BaseTypes) { Write-Line ("  - " + $baseType) }
            }

            if ($item.Dependencies.Count -gt 0) {
                Write-Line "- Dependencias de construtor:"
                foreach ($dependency in $item.Dependencies) { Write-Line ("  - " + $dependency) }
            }
            else {
                Write-Line "- Dependencias de construtor: nenhuma identificada"
            }

            if ($item.Methods.Count -gt 0) {
                Write-Line "- Metodos publicos:"
                foreach ($method in $item.Methods) { Write-Line ("  - " + $method) }
            }
            else {
                Write-Line "- Metodos publicos: nenhum identificado"
            }

            Write-Line
        }
    }

    Write-Line "## Candidatos prioritarios para documentacao"
    Write-Line

    $priorityPatterns = @(
        "Controller$", "UseCase$", "Worker$", "Middleware$", "Repository$",
        "Publisher$", "Gateway$", "CacheService$", "HealthCheck$", "Strategy$", "Validator$"
    )

    foreach ($item in ($items | Sort-Object Category, TypeName)) {
        $isPriority = $false

        foreach ($pattern in $priorityPatterns) {
            if ($item.TypeName -match $pattern) {
                $isPriority = $true
                break
            }
        }

        if ($isPriority) {
            Write-Line ("- " + $item.Category + " / " + $item.TypeName + " / " + $item.RelativePath)
        }
    }

    Write-Line
    Write-Line "## Checklist de validacao"
    Write-Line
    Write-Line "- [ ] Validar tipos que usam primary constructors."
    Write-Line "- [ ] Validar records e classes parciais."
    Write-Line "- [ ] Confirmar dependencias injetadas."
    Write-Line "- [ ] Confirmar metodos publicos relevantes."
    Write-Line "- [ ] Enviar este inventario para gerar o pacote da Sprint 2."
}
finally {
    $writer.Flush()
    $writer.Close()
}

Write-Host
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host " Inventario arquitetural gerado" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "Arquivo:"
Write-Host $output -ForegroundColor Green
