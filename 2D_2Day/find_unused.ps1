$assetDir = "Assets"

Write-Host "Finding all asset files..."
$assetFiles = Get-ChildItem -Path $assetDir -Recurse -Include *.prefab, *.unity, *.asset, *.mat, *.controller, *.anim

Write-Host "Extracting GUIDs from assets..."
$usedGuids = New-Object System.Collections.Generic.HashSet[string]
foreach ($file in $assetFiles) {
    $content = Get-Content $file.FullName -Raw
    $matches = [regex]::Matches($content, "guid:\s*([a-f0-9]{32})")
    foreach ($m in $matches) {
        $null = $usedGuids.Add($m.Groups[1].Value)
    }
}

Write-Host "Finding all cs files..."
$csFiles = Get-ChildItem -Path $assetDir -Recurse -Filter *.cs

Write-Host "Checking unused scripts..."
$unusedScripts = @()
foreach ($cs in $csFiles) {
    $metaPath = $cs.FullName + ".meta"
    $isUsed = $false
    if (Test-Path $metaPath) {
        $metaContent = Get-Content $metaPath -Raw
        if ($metaContent -match "guid:\s*([a-f0-9]{32})") {
            $guid = $matches[1]
            if ($usedGuids.Contains($guid)) {
                $isUsed = $true
            }
        }
    }
    
    if (-not $isUsed) {
        $className = [System.IO.Path]::GetFileNameWithoutExtension($cs.Name)
        $classUsed = $false
        foreach ($other in $csFiles) {
            if ($other.FullName -ne $cs.FullName) {
                $otherContent = Get-Content $other.FullName -Raw
                if ($otherContent -match "\b$className\b") {
                    $classUsed = $true
                    break
                }
            }
        }
        if ($classUsed) {
            $isUsed = $true
        }
    }
    
    if (-not $isUsed) {
        $unusedScripts += $cs.FullName
    }
}

$unusedScripts | Out-File "unused_scripts.txt"
Write-Host "Done! Found $($unusedScripts.Length) unused scripts."
