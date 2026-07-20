#requires -Version 5.1

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root = $PSScriptRoot
$output = Join-Path $root "OrderFlow-Documentacao-Automatica.md"

$ignoredDirectories = @(
    ".git",
    ".vs",
    ".idea",
    "bin",
    "obj",
    "node_modules",
    "TestResults",
    "coverage",
    "artifacts",
    "packages"
)

$ignoredFilePatterns = @(
    "*.user",
    "*.suo",
    "*.pfx",
    "*.snk",
    "*.pem",
    "*.key",
    "*.cer",
    "*.crt",
    "*.secrets.json",
    ".env",
    ".env.*",
    "appsettings.Production.json",
    "appsettings.Staging.json",
    "launchSettings.json"
)

$importantFileNames = @(
    "Program.cs",
    "Startup.cs",
    "Dockerfile",
    "docker-compose.yml",
    "docker-compose.yaml",
    "compose.yml",
    "compose.yaml",
    "Directory.Build.props",
    "Directory.Build.targets",
    "Directory.Packages.props",
    "global.json",
    "nuget.config"
)

function Test-IgnoredPath {
    param([string]$FullName)

    foreach ($directory in $ignoredDirectories) {
        $pattern = "[\\/]" + [regex]::Escape($directory) + "([\\/]|$)"
        if ($FullName -match $pattern) {
            return $true
        }
    }

    $name = [System.IO.Path]::GetFileName($FullName)

    foreach ($pattern in $ignoredFilePatterns) {
        if ($name -like $pattern) {
            return $true
        }
    }

    return $false
}

function Get-RelativePath {
    param(
        [string]$BasePath,
        [string]$TargetPath
    )

    $base = $BasePath.TrimEnd("\") + "\"
    $baseUri = New-Object System.Uri($base)
    $targetUri = New-Object System.Uri($TargetPath)

    $relative = $baseUri.MakeRelativeUri($targetUri).ToString()
    return [System.Uri]::UnescapeDataString($relative.Replace("/", "\"))
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$writer = New-Object System.IO.StreamWriter($output, $false, $utf8NoBom)

function Write-Line {
    param([string]$Text = "")
    $script:writer.WriteLine($Text)
}

function Write-Section {
    param([string]$Title)
    Write-Line
    Write-Line ("## " + $Title)
    Write-Line
}

function Write-CodeBlock {
    param(
        [string]$Language,
        [string[]]$Content
    )

    Write-Line ("~~~~" + $Language)

    foreach ($line in $Content) {
        Write-Line $line
    }

    Write-Line "~~~~"
}

function Get-SafeFileContent {
    param([string]$Path)

    if (Test-IgnoredPath -FullName $Path) {
        return @("[ARQUIVO IGNORADO POR SEGURANCA]")
    }

    try {
        $lines = Get-Content -Path $Path -Encoding UTF8 -ErrorAction Stop
        $result = New-Object System.Collections.Generic.List[string]

        foreach ($line in $lines) {
            $current = $line

            $current = $current -replace '(?i)(password|pwd|secret|token|apikey|api_key|accesskey|access_key)\s*[:=]\s*["'']?[^"'',;\s]+', '$1=***REDACTED***'
            $current = $current -replace '(?i)(User ID|UID)\s*=\s*[^;]+', '$1=***REDACTED***'
            $current = $current -replace '(?i)(Password|Pwd)\s*=\s*[^;]+', '$1=***REDACTED***'

            $result.Add($current)
        }

        return $result.ToArray()
    }
    catch {
        return @("[ERRO AO LER ARQUIVO: " + $_.Exception.Message + "]")
    }
}

try {
    $solutions = Get-ChildItem -Path $root -Filter *.sln -File -ErrorAction SilentlyContinue

    Write-Line "# OrderFlow - Documentacao Automatica"
    Write-Line
    Write-Line ("> Gerado automaticamente em " + (Get-Date -Format "yyyy-MM-dd HH:mm:ss") + ".")
    Write-Line
    Write-Line "> Este documento representa um snapshot tecnico da solucao."
    Write-Line "> Revise o arquivo antes de compartilhar."

    Write-Section "Solucoes encontradas"

    if ($solutions) {
        foreach ($solution in $solutions) {
            $relative = Get-RelativePath -BasePath $root -TargetPath $solution.FullName
            Write-Line ("- " + $relative)
        }
    }
    else {
        Write-Line "_Nenhuma solucao encontrada._"
    }

    Write-Section "Estrutura da solucao"
    Write-Line "~~~~text"

    $allItems = Get-ChildItem -Path $root -Recurse -Force |
        Where-Object { -not (Test-IgnoredPath -FullName $_.FullName) } |
        Sort-Object FullName

    foreach ($item in $allItems) {
        $relative = Get-RelativePath -BasePath $root -TargetPath $item.FullName
        $depth = ($relative -split '[\\/]').Count - 1
        $indent = "    " * $depth

        if ($item.PSIsContainer) {
            Write-Line ($indent + $item.Name + "/")
        }
        else {
            Write-Line ($indent + $item.Name)
        }
    }

    Write-Line "~~~~"

    $projects = Get-ChildItem -Path $root -Recurse -Filter *.csproj -File |
        Where-Object { -not (Test-IgnoredPath -FullName $_.FullName) } |
        Sort-Object FullName

    Write-Section "Projetos .NET"

    if ($projects) {
        foreach ($project in $projects) {
            $relative = Get-RelativePath -BasePath $root -TargetPath $project.FullName
            Write-Line ("- " + $relative)
        }
    }
    else {
        Write-Line "_Nenhum projeto encontrado._"
    }

    Write-Section "Pacotes NuGet por projeto"

    foreach ($project in $projects) {
        $relativeProject = Get-RelativePath -BasePath $root -TargetPath $project.FullName

        Write-Line
        Write-Line ("### " + $relativeProject)
        Write-Line

        try {
            [xml]$xml = Get-Content -Path $project.FullName -Raw -Encoding UTF8
            $packageReferences = @($xml.Project.ItemGroup.PackageReference)

            if ($packageReferences.Count -eq 0) {
                Write-Line "_Nenhum PackageReference encontrado._"
            }
            else {
                foreach ($package in $packageReferences) {
                    $include = [string]$package.Include

                    if ($package.Version) {
                        $version = [string]$package.Version
                    }
                    elseif ($package.VersionOverride) {
                        $version = [string]$package.VersionOverride
                    }
                    else {
                        $version = "(versao centralizada ou nao informada)"
                    }

                    Write-Line ("- " + $include + " : " + $version)
                }
            }
        }
        catch {
            Write-Line ("_Nao foi possivel interpretar o projeto: " + $_.Exception.Message + "_")
        }
    }

    Write-Section "Dependencias entre projetos"

    foreach ($project in $projects) {
        $relativeProject = Get-RelativePath -BasePath $root -TargetPath $project.FullName

        try {
            [xml]$xml = Get-Content -Path $project.FullName -Raw -Encoding UTF8
            $references = @($xml.Project.ItemGroup.ProjectReference)

            if ($references.Count -gt 0) {
                Write-Line
                Write-Line ("### " + $relativeProject)
                Write-Line

                foreach ($reference in $references) {
                    Write-Line ("- " + [string]$reference.Include)
                }
            }
        }
        catch {
            Write-Line ("_Erro ao ler referencias de " + $relativeProject + "._")
        }
    }

    $importantFiles = Get-ChildItem -Path $root -Recurse -File |
        Where-Object {
            (-not (Test-IgnoredPath -FullName $_.FullName)) -and (
                ($importantFileNames -contains $_.Name) -or
                ($_.FullName -match "[\\/]\.github[\\/]workflows[\\/].+\.(yml|yaml)$") -or
                ($_.Name -like "appsettings*.json")
            )
        } |
        Sort-Object FullName -Unique

    Write-Section "Arquivos importantes"

    if ($importantFiles) {
        foreach ($file in $importantFiles) {
            $relative = Get-RelativePath -BasePath $root -TargetPath $file.FullName
            Write-Line ("- " + $relative)
        }
    }
    else {
        Write-Line "_Nenhum arquivo importante encontrado._"
    }

    Write-Section "Conteudo dos arquivos importantes"
    Write-Line "> Revise este trecho antes de compartilhar."
    Write-Line "> O script mascara alguns padroes comuns de segredo, mas a revisao humana continua obrigatoria."

    foreach ($file in $importantFiles) {
        $relative = Get-RelativePath -BasePath $root -TargetPath $file.FullName
        $extension = $file.Extension.TrimStart(".")

        if ([string]::IsNullOrWhiteSpace($extension)) {
            $extension = "text"
        }

        Write-Line
        Write-Line ("### " + $relative)
        Write-Line

        $content = Get-SafeFileContent -Path $file.FullName
        Write-CodeBlock -Language $extension -Content $content
    }

    $codeGroups = [ordered]@{
        "Controllers" = "*Controller.cs"
        "Workers e Background Services" = "*Worker.cs"
        "Use Cases" = "*UseCase.cs"
        "Consumers" = "*Consumer.cs"
        "Publishers" = "*Publisher.cs"
        "Repositories" = "*Repository.cs"
        "Health Checks" = "*HealthCheck.cs"
    }

    foreach ($groupName in $codeGroups.Keys) {
        Write-Section $groupName

        $pattern = $codeGroups[$groupName]

        $files = Get-ChildItem -Path $root -Recurse -Filter $pattern -File |
            Where-Object { -not (Test-IgnoredPath -FullName $_.FullName) } |
            Sort-Object FullName

        if ($files) {
            foreach ($file in $files) {
                $relative = Get-RelativePath -BasePath $root -TargetPath $file.FullName
                Write-Line ("- " + $relative)
            }
        }
        else {
            Write-Line ("_Nenhum arquivo encontrado pelo padrao " + $pattern + "._")
        }
    }

    $observabilityTerms = @(
        "Serilog",
        "OpenTelemetry",
        "AddOpenTelemetry",
        "HealthChecks",
        "MapHealthChecks",
        "Prometheus",
        "Grafana",
        "CorrelationId",
        "ActivitySource",
        "Meter",
        "AddMetrics",
        "AddTracing",
        "UseSerilog",
        "Loki",
        "Tempo"
    )

    Write-Section "Indicadores de observabilidade encontrados no codigo"

    $sourceFiles = Get-ChildItem -Path $root -Recurse -Include *.cs,*.json,*.yml,*.yaml -File |
        Where-Object { -not (Test-IgnoredPath -FullName $_.FullName) }

    foreach ($term in $observabilityTerms) {
        $matches = @()

        if ($sourceFiles) {
            $matches = Select-String -Path $sourceFiles.FullName -Pattern $term -SimpleMatch -ErrorAction SilentlyContinue
        }

        if ($matches) {
            Write-Line
            Write-Line ("### " + $term)
            Write-Line

            foreach ($match in ($matches | Select-Object -First 20)) {
                $relative = Get-RelativePath -BasePath $root -TargetPath $match.Path
                Write-Line ("- " + $relative + " - linha " + $match.LineNumber)
            }
        }
    }

    if (Test-Path (Join-Path $root ".git")) {
        Write-Section "Git"

        try {
            $branch = git -C $root branch --show-current 2>$null
            $lastCommit = git -C $root log -1 --pretty=format:"%h - %s (%ad)" --date=short 2>$null

            Write-Line ("- Branch atual: " + $branch)
            Write-Line ("- Ultimo commit: " + $lastCommit)
        }
        catch {
            Write-Line "_Nao foi possivel consultar informacoes do Git._"
        }
    }

    Write-Section "Checklist de revisao manual"
    Write-Line "- [ ] Confirmar se nenhum segredo permaneceu no arquivo."
    Write-Line "- [ ] Confirmar se a solucao e os projetos listados estao corretos."
    Write-Line "- [ ] Confirmar se Docker, workflows e configuracoes representam o estado atual."
    Write-Line "- [ ] Confirmar se os componentes de observabilidade foram detectados."
    Write-Line "- [ ] Enviar este arquivo para revisao arquitetural e documental."
}
finally {
    $writer.Flush()
    $writer.Close()
}

Write-Host
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host " Documentacao gerada com sucesso" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "Arquivo:"
Write-Host $output -ForegroundColor Green
Write-Host
Write-Host "Revise o arquivo antes de compartilhar." -ForegroundColor Yellow
