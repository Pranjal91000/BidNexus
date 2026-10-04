# ============================================================================
# BidNexus Database & Asset Seeding Script
# Populates Organizations (12+), Vendors (30+), Master Items (24+), Images & Attachments,
# and generates the comprehensive User Credentials Markdown reference.
# ============================================================================

param(
    [string]$DbName = "BidNexus",
    [string]$DbUser = "postgres",
    [string]$DbPassword = "postgres",
    [string]$DbHost = "localhost",
    [string]$DbPort = "5432"
)

$ErrorActionPreference = "Stop"

Write-Host "=================================================" -ForegroundColor Cyan
Write-Host "  BidNexus Database and Asset Seeding Script     " -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan

# 1. Locate directories
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$BackendDir = Resolve-Path (Join-Path $ScriptDir "..\..")
$SolutionDir = Resolve-Path (Join-Path $ScriptDir "..\..\..")
$UploadsDir = Join-Path $BackendDir "BidNexus\wwwroot\uploads"
$SqlScript = Join-Path $ScriptDir "seed_data.sql"
$MarkdownPath = Join-Path $BackendDir "LocalReuseables\UserCredentials_And_SeedDetails.md"

if (-not (Test-Path $UploadsDir)) {
    New-Item -ItemType Directory -Path $UploadsDir -Force | Out-Null
}
Write-Host "Uploads directory: $UploadsDir" -ForegroundColor Green

# 2. Locate psql.exe
$psql = Get-Command "psql.exe" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source
if (-not $psql) {
    $candidatePaths = @(
        "D:\Program Files\PostgreSQL\bin\psql.exe",
        "C:\Program Files\PostgreSQL\17\bin\psql.exe",
        "C:\Program Files\PostgreSQL\16\bin\psql.exe",
        "C:\Program Files\PostgreSQL\15\bin\psql.exe",
        "C:\Program Files\PostgreSQL\bin\psql.exe"
    )
    foreach ($cand in $candidatePaths) {
        if (Test-Path $cand) {
            $psql = $cand
            break
        }
    }
}

if (-not $psql) {
    Write-Error "Could not locate psql.exe. Please ensure PostgreSQL is installed."
    exit 1
}
Write-Host "Using psql: $psql" -ForegroundColor Green

$GlobalSql = Join-Path $ScriptDir "globaldata.sql"
$env:PGPASSWORD = $DbPassword

# 2b. Execute globaldata.sql if present
if (Test-Path $GlobalSql) {
    Write-Host "`nRunning global data seed ($GlobalSql)..." -ForegroundColor Yellow
    & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -f $GlobalSql
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Execution of $GlobalSql failed with code $LASTEXITCODE."
        exit $LASTEXITCODE
    }
    Write-Host "Global data successfully seeded." -ForegroundColor Green
}

# 3. Execute seed_data.sql
Write-Host "`nRunning database schema and records seed ($SqlScript)..." -ForegroundColor Yellow
& $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -f $SqlScript
if ($LASTEXITCODE -ne 0) {
    Write-Error "Execution of $SqlScript failed with code $LASTEXITCODE."
    exit $LASTEXITCODE
}
Write-Host "Base records, organizations, vendors, and auctions successfully seeded." -ForegroundColor Green

# 4. Download Real Authentic Industrial Photos for Master Items from the Internet
Write-Host "`nDownloading authentic industrial photographs for Master Items..." -ForegroundColor Yellow

$realItemImages = @{
    "STL-PLT-10"   = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=500&h=500&fit=crop&q=80" # Structural Steel Heavy Plate / Fabrication
    "STL-PIP-150"  = "https://images.unsplash.com/photo-1578328819058-b69f3a3b0f6b?w=500&h=500&fit=crop&q=80" # Seamless Carbon Steel Pipes
    "VLV-BAL-100"  = "https://images.unsplash.com/photo-1581092160607-ee22621dd758?w=500&h=500&fit=crop&q=80" # Industrial Trunnion Ball Valve
    "PMP-HYD-50"   = "https://images.unsplash.com/photo-1581092334651-ddf26d9a09d0?w=500&h=500&fit=crop&q=80" # Variable Displacement Hydraulic Axial Pump
    "GEN-DSL-500"  = "https://images.unsplash.com/photo-1581092162384-8987c1d64718?w=500&h=500&fit=crop&q=80" # 500 kVA Diesel Generator Set
    "TRF-OIL-1000" = "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?w=500&h=500&fit=crop&q=80" # 1000 kVA Substation Power Transformer
    "SFT-HLM-CE"   = "https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=500&h=500&fit=crop&q=80" # Industrial Safety Hard Hat Helmet
    "FLG-WNR-200"  = "https://images.unsplash.com/photo-1581092580497-e0d23cbdf1dc?w=500&h=500&fit=crop&q=80" # Weld Neck Flange 8 Inch RF
    "CBL-COP-400"  = "https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=500&h=500&fit=crop&q=80" # Heavy Industrial Power Cables Spool
    "SOL-PNL-550"  = "https://images.unsplash.com/photo-1559302504-64aae6ca6b6d?w=500&h=500&fit=crop&q=80" # 550W Mono PERC Solar PV Panels
    "FST-BLT-M24"  = "https://images.unsplash.com/photo-1586864387967-d02ef85d93e8?w=500&h=500&fit=crop&q=80" # High Tensile Hex Bolts and Nuts
    "BRG-ROL-222"  = "https://images.unsplash.com/photo-1581092162384-8987c1d64718?w=500&h=500&fit=crop&q=80" # Spherical Roller Bearing
    "ACT-PNE-160"  = "https://images.unsplash.com/photo-1581091226825-a6a2a5aee158?w=500&h=500&fit=crop&q=80" # Double Acting Pneumatic Actuator
    "VLV-DEL-150"  = "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=500&h=500&fit=crop&q=80" # Automatic Deluge Fire Valve
    "STL-IBM-400"  = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=500&h=500&fit=crop&q=80" # Universal Heavy Structural Steel Beams
    "BLT-CNV-120"  = "https://images.unsplash.com/photo-1581092580497-e0d23cbdf1dc?w=500&h=500&fit=crop&q=80" # Steel Cord Conveyor Belting
    "INS-CER-142"  = "https://images.unsplash.com/photo-1581094794329-c8112a89af12?w=500&h=500&fit=crop&q=80" # Ceramic Fiber High Temp Insulation Blanket
    "LGT-EXP-150"  = "https://images.unsplash.com/photo-1517524008697-84bbe3c3fd98?w=500&h=500&fit=crop&q=80" # Explosion Proof Flameproof LED Light
    "PMP-CNT-200"  = "https://images.unsplash.com/photo-1581092160607-ee22621dd758?w=500&h=500&fit=crop&q=80" # End Suction Centrifugal Process Pump
    "MOT-IND-110"  = "https://images.unsplash.com/photo-1581092162384-8987c1d64718?w=500&h=500&fit=crop&q=80" # 110 kW Induction Motor IE4
    "CRN-EOT-20T"  = "https://images.unsplash.com/photo-1581093588401-fbb62a02f120?w=500&h=500&fit=crop&q=80" # 20 Ton Double Girder Overhead Crane
    "VLV-CHK-250"  = "https://images.unsplash.com/photo-1581092334651-ddf26d9a09d0?w=500&h=500&fit=crop&q=80" # Dual Plate Wafer Check Valve
    "GSK-SPW-300"  = "https://images.unsplash.com/photo-1581092580497-e0d23cbdf1dc?w=500&h=500&fit=crop&q=80" # Spiral Wound Metallic Gasket
    "CHM-CRN-500"  = "https://images.unsplash.com/photo-1584467735871-8e85353a8413?w=500&h=500&fit=crop&q=80" # Scale Inhibitor Chemical Storage Drums
}

$wc = New-Object System.Net.WebClient
$wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)")

$itemsQuery = 'SELECT i."Id", i."Code", i."Name", i."TenantId", COALESCE(a."StoredFileName", ''''), COALESCE(a."ContentType", '''') FROM "Master"."Item" i LEFT JOIN "Utilities"."Attachment" a ON i."DocAttachmentId" = a."Id";'
$rawItems = $itemsQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

$updatedPhotosCount = 0
foreach ($line in ($rawItems -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $parts = $line.Split('|')
    if ($parts.Count -lt 6) { continue }
    $itemId = [int]$parts[0]
    $itemCode = $parts[1].Trim()
    $itemName = $parts[2].Trim()
    $tenantId = [int]$parts[3]
    $oldStoredFileName = $parts[4].Trim()
    $oldContentType = $parts[5].Trim()

    # If it is already a real JPEG, skip
    if ($oldContentType -eq "image/jpeg" -and (Test-Path (Join-Path $UploadsDir $oldStoredFileName))) {
        continue
    }

    $photoUrl = $realItemImages[$itemCode]
    $downloadSuccess = $false
    $guid = [guid]::NewGuid().ToString("N")
    $storedFileName = "$guid.jpg"
    $fullPath = Join-Path $UploadsDir $storedFileName

    if ($photoUrl) {
        try {
            $bytes = $wc.DownloadData($photoUrl)
            [System.IO.File]::WriteAllBytes($fullPath, $bytes)
            $fileSize = $bytes.Length
            $downloadSuccess = $true
        } catch {
            Write-Warning "Could not download photo for $itemCode : $($_.Exception.Message)"
        }
    }

    if ($downloadSuccess) {
        # Delete old file to free disk space
        if ($oldStoredFileName -and (Test-Path (Join-Path $UploadsDir $oldStoredFileName))) {
            Remove-Item (Join-Path $UploadsDir $oldStoredFileName) -Force -ErrorAction SilentlyContinue
        }

        $relPath = "uploads/$storedFileName"
        $attachSql = @"
DO `$`$
DECLARE
    v_att_id integer;
BEGIN
    INSERT INTO "Utilities"."Attachment" ("TenantId", "OriginalFileName", "StoredFileName", "ContentType", "FileSize", "RelativePath", "CreatedDateTime")
    VALUES ($tenantId, '$itemCode.jpg', '$storedFileName', 'image/jpeg', $fileSize, '$relPath', NOW())
    RETURNING "Id" INTO v_att_id;

    UPDATE "Master"."Item" SET "DocAttachmentId" = v_att_id WHERE "Id" = $itemId;
END `$`$;
"@
        $attachSql | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName | Out-Null
        $updatedPhotosCount++
        Write-Host "  -> Downloaded real photo for Item [$itemCode] $itemName ($([math]::Round($fileSize/1024, 1)) KB)" -ForegroundColor Green
    }
}
Write-Host "Real item photos updated: $updatedPhotosCount items linked." -ForegroundColor Green

# 4b. Generate Modern Foreground Banners for Organizations lacking ForegroundImageId
Write-Host "`nGenerating foreground branding banners for Organizations..." -ForegroundColor Yellow
$orgsBannerQuery = 'SELECT o."Id", o."Name", o."TenantId" FROM "TenantRel"."Organization" o WHERE o."ForegroundImageId" IS NULL;'
$rawOrgsBanner = $orgsBannerQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

$orgColors = @("#1e3a8a", "#065f46", "#831843", "#701a75", "#14532d", "#1e293b", "#312e81", "#7c2d12", "#0c4a6e", "#374151")
$orgCount = 0
foreach ($line in ($rawOrgsBanner -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $parts = $line.Split('|')
    if ($parts.Count -lt 3) { continue }
    $oId = [int]$parts[0]
    $oName = $parts[1].Trim()
    $tId = [int]$parts[2]

    $themeColor = $orgColors[$orgCount % $orgColors.Count]
    $initials = ($oName.Split(' ') | ForEach-Object { if ($_.Length -gt 0) { $_[0] } }) -join ''
    if ($initials.Length -gt 4) { $initials = $initials.Substring(0, 4) }
    $guid = [guid]::NewGuid().ToString("N")
    $storedFileName = "$guid.svg"
    $fullPath = Join-Path $UploadsDir $storedFileName

    $cleanName = [System.Security.SecurityElement]::Escape($oName)

    $bannerSvg = @"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 360" width="1200" height="360">
  <defs>
    <linearGradient id="bg_$guid" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="$themeColor" />
      <stop offset="45%" stop-color="#0f172a" />
      <stop offset="100%" stop-color="#020617" />
    </linearGradient>
    <linearGradient id="glow_$guid" x1="0%" y1="100%" x2="100%" y2="0%">
      <stop offset="0%" stop-color="#38bdf8" stop-opacity="0.25" />
      <stop offset="100%" stop-color="#818cf8" stop-opacity="0" />
    </linearGradient>
  </defs>
  <rect width="1200" height="360" rx="16" fill="url(#bg_$guid)" />
  <rect width="1200" height="360" rx="16" fill="url(#glow_$guid)" />
  <!-- Grid accents -->
  <circle cx="1050" cy="180" r="160" fill="none" stroke="#ffffff" stroke-opacity="0.04" stroke-width="2" />
  <circle cx="1050" cy="180" r="240" fill="none" stroke="#ffffff" stroke-opacity="0.03" stroke-width="2" />
  <circle cx="1050" cy="180" r="320" fill="none" stroke="#ffffff" stroke-opacity="0.02" stroke-width="2" />
  
  <g transform="translate(64, 110)">
    <!-- Monogram badge -->
    <rect x="0" y="0" width="140" height="140" rx="28" fill="#ffffff" fill-opacity="0.08" stroke="#ffffff" stroke-opacity="0.2" stroke-width="2" />
    <text x="70" y="80" font-family="system-ui, -apple-system, sans-serif" font-size="44" font-weight="900" fill="#ffffff" text-anchor="middle">$initials</text>
    
    <!-- Title & Tag -->
    <g transform="translate(180, 20)">
      <rect x="0" y="0" width="160" height="26" rx="6" fill="#38bdf8" fill-opacity="0.2" stroke="#38bdf8" stroke-opacity="0.3" stroke-width="1" />
      <text x="80" y="17" font-family="system-ui, -apple-system, sans-serif" font-size="11" font-weight="800" fill="#38bdf8" text-anchor="middle" letter-spacing="1">VERIFIED BUYER</text>
      <text x="0" y="64" font-family="system-ui, -apple-system, sans-serif" font-size="36" font-weight="800" fill="#ffffff">$cleanName</text>
      <text x="0" y="94" font-family="system-ui, -apple-system, sans-serif" font-size="15" font-weight="500" fill="#94a3b8">Enterprise Procurement &amp; Auction Sponsor</text>
    </g>
  </g>
</svg>
"@
    [System.IO.File]::WriteAllText($fullPath, $bannerSvg, [System.Text.Encoding]::UTF8)
    $fileSize = (Get-Item $fullPath).Length
    $relPath = "uploads/$storedFileName"

    $attachSql = @"
DO `$`$
DECLARE
    v_att_id integer;
BEGIN
    INSERT INTO "Utilities"."Attachment" ("TenantId", "OriginalFileName", "StoredFileName", "ContentType", "FileSize", "RelativePath", "CreatedDateTime")
    VALUES ($tId, '$cleanName.svg', '$storedFileName', 'image/svg+xml', $fileSize, '$relPath', NOW())
    RETURNING "Id" INTO v_att_id;

    UPDATE "TenantRel"."Organization" SET "ForegroundImageId" = v_att_id WHERE "Id" = $oId;
END `$`$;
"@
    $attachSql | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName | Out-Null
    $orgCount++
    Write-Host "  -> Generated banner for Organization: $oName" -ForegroundColor Gray
}
Write-Host "Organization banners generated: $orgCount linked." -ForegroundColor Green

# 4c. Generate Modern Foreground Banners for Vendors lacking ForegroundImageId
Write-Host "`nGenerating foreground branding banners for Vendors..." -ForegroundColor Yellow
$vendorsBannerQuery = 'SELECT v."Id", v."Name", v."TenantId" FROM "TenantRel"."Vendor" v WHERE v."ForegroundImageId" IS NULL;'
$rawVendorsBanner = $vendorsBannerQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

$vendorColors = @("#047857", "#1d4ed8", "#b45309", "#4338ca", "#0e7490", "#6d28d9", "#be123c", "#15803d", "#0369a1", "#334155")
$vendorCount = 0
foreach ($line in ($rawVendorsBanner -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $parts = $line.Split('|')
    if ($parts.Count -lt 3) { continue }
    $vId = [int]$parts[0]
    $vName = $parts[1].Trim()
    $tId = [int]$parts[2]

    $themeColor = $vendorColors[$vendorCount % $vendorColors.Count]
    $initials = ($vName.Split(' ') | ForEach-Object { if ($_.Length -gt 0) { $_[0] } }) -join ''
    if ($initials.Length -gt 4) { $initials = $initials.Substring(0, 4) }
    $guid = [guid]::NewGuid().ToString("N")
    $storedFileName = "$guid.svg"
    $fullPath = Join-Path $UploadsDir $storedFileName

    $cleanName = [System.Security.SecurityElement]::Escape($vName)

    $bannerSvg = @"
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1200 360" width="1200" height="360">
  <defs>
    <linearGradient id="vbg_$guid" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="$themeColor" />
      <stop offset="45%" stop-color="#111827" />
      <stop offset="100%" stop-color="#030712" />
    </linearGradient>
    <linearGradient id="vglow_$guid" x1="0%" y1="0%" x2="100%" y2="100%">
      <stop offset="0%" stop-color="#34d399" stop-opacity="0.2" />
      <stop offset="100%" stop-color="#60a5fa" stop-opacity="0" />
    </linearGradient>
  </defs>
  <rect width="1200" height="360" rx="16" fill="url(#vbg_$guid)" />
  <rect width="1200" height="360" rx="16" fill="url(#vglow_$guid)" />
  <!-- Hexagon / grid accents -->
  <circle cx="1080" cy="180" r="140" fill="none" stroke="#ffffff" stroke-opacity="0.05" stroke-width="2" />
  <circle cx="1080" cy="180" r="220" fill="none" stroke="#ffffff" stroke-opacity="0.03" stroke-width="2" />
  
  <g transform="translate(64, 110)">
    <!-- Monogram badge -->
    <rect x="0" y="0" width="140" height="140" rx="28" fill="#ffffff" fill-opacity="0.08" stroke="#ffffff" stroke-opacity="0.2" stroke-width="2" />
    <text x="70" y="80" font-family="system-ui, -apple-system, sans-serif" font-size="44" font-weight="900" fill="#ffffff" text-anchor="middle">$initials</text>
    
    <!-- Title & Tag -->
    <g transform="translate(180, 20)">
      <rect x="0" y="0" width="170" height="26" rx="6" fill="#10b981" fill-opacity="0.2" stroke="#10b981" stroke-opacity="0.3" stroke-width="1" />
      <text x="85" y="17" font-family="system-ui, -apple-system, sans-serif" font-size="11" font-weight="800" fill="#34d399" text-anchor="middle" letter-spacing="1">QUALIFIED VENDOR</text>
      <text x="0" y="64" font-family="system-ui, -apple-system, sans-serif" font-size="34" font-weight="800" fill="#ffffff">$cleanName</text>
      <text x="0" y="94" font-family="system-ui, -apple-system, sans-serif" font-size="15" font-weight="500" fill="#9ca3af">Industrial Supplier &amp; Bidding Partner</text>
    </g>
  </g>
</svg>
"@
    [System.IO.File]::WriteAllText($fullPath, $bannerSvg, [System.Text.Encoding]::UTF8)
    $fileSize = (Get-Item $fullPath).Length
    $relPath = "uploads/$storedFileName"

    $attachSql = @"
DO `$`$
DECLARE
    v_att_id integer;
BEGIN
    INSERT INTO "Utilities"."Attachment" ("TenantId", "OriginalFileName", "StoredFileName", "ContentType", "FileSize", "RelativePath", "CreatedDateTime")
    VALUES ($tId, '$cleanName.svg', '$storedFileName', 'image/svg+xml', $fileSize, '$relPath', NOW())
    RETURNING "Id" INTO v_att_id;

    UPDATE "TenantRel"."Vendor" SET "ForegroundImageId" = v_att_id WHERE "Id" = $vId;
END `$`$;
"@
    $attachSql | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName | Out-Null
    $vendorCount++
    Write-Host "  -> Generated banner for Vendor: $vName" -ForegroundColor Gray
}
Write-Host "Vendor banners generated: $vendorCount linked." -ForegroundColor Green

# 5. Extract all Organizations and Vendors and generate the Markdown Credentials Document
Write-Host "`nGenerating User Credentials Reference Markdown ($MarkdownPath)..." -ForegroundColor Yellow

$orgsQuery = 'SELECT o."Id", o."Name", t."UserName", t."Id", t."EmailAddress", t."ContactNumber", COALESCE(o."ForegroundImageId"::text, ''--''), COALESCE(o."OfficialAddress", ''--'') FROM "TenantRel"."Organization" o JOIN "TenantRel"."Tenant" t ON o."TenantId" = t."Id" ORDER BY o."Id" ASC;'
$rawOrgs = $orgsQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

$vendorsQuery = 'SELECT v."Id", v."Name", t."UserName", t."Id", t."EmailAddress", t."ContactNumber", COALESCE(v."ForegroundImageId"::text, ''--''), COALESCE(v."About", ''--'') FROM "TenantRel"."Vendor" v JOIN "TenantRel"."Tenant" t ON v."TenantId" = t."Id" ORDER BY v."Id" ASC;'
$rawVendors = $vendorsQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

$itemsCatalogQuery = 'SELECT i."Id", i."Code", i."Name", COALESCE(c."Name", ''General''), COALESCE(i."DocAttachmentId"::text, ''--'') FROM "Master"."Item" i LEFT JOIN "GlobalData"."Category" c ON i."CategoryId" = c."Id" ORDER BY i."Id" ASC;'
$rawCatalog = $itemsCatalogQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

$b = [char]96

$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine("# BidNexus - Seeded Credentials & Catalog Master Reference")
[void]$sb.AppendLine()
[void]$sb.AppendLine("> **Generated Date:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') UTC  ")
[void]$sb.AppendLine("> **Universal Password:** ${b}Password@123${b} for all seeded Organizations and Vendors.  ")
[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 1. Quick Authentication & Login Guide")
[void]$sb.AppendLine()
[void]$sb.AppendLine("You can log in as any Organization or Vendor using the credentials below:")
[void]$sb.AppendLine("1. Send a POST request to `/api/auth/login`:")
[void]$sb.AppendLine('```json')
[void]$sb.AppendLine('{')
[void]$sb.AppendLine('  "userName": "<Username>",')
[void]$sb.AppendLine('  "password": "Password@123"')
[void]$sb.AppendLine('}')
[void]$sb.AppendLine('```')
[void]$sb.AppendLine("2. In the Frontend Web Application (http://localhost:5173), paste the generated authToken JWT token into the connection card.")
[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 2. Seeded Organizations (Enterprise Buyers)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| # | Legal / Organization Name | Username | Password | Role | Tenant ID | Email Address | Phone | Image ID | Official Address |")
[void]$sb.AppendLine("|---|---------------------------|----------|----------|------|-----------|---------------|-------|----------|------------------|")

$oIdx = 1
foreach ($line in ($rawOrgs -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $p = $line.Split('|')
    if ($p.Count -lt 8) { continue }
    $col1 = $p[1].Trim()
    $col2 = $p[2].Trim()
    $col3 = $p[3].Trim()
    $col4 = $p[4].Trim()
    $col5 = $p[5].Trim()
    $col6 = $p[6].Trim()
    $col7 = $p[7].Trim()
    [void]$sb.AppendLine("| $oIdx | **$col1** | $b$col2$b | ${b}Password@123${b} | Organization | $col3 | $b$col4$b | $col5 | $b$col6$b | $col7 |")
    $oIdx++
}

[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 3. Seeded Vendors (Qualified Suppliers)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| # | Vendor / Supplier Business Name | Username | Password | Role | Tenant ID | Email Address | Phone | Image ID | Specialization / Scope |")
[void]$sb.AppendLine("|---|---------------------------------|----------|----------|------|-----------|---------------|-------|----------|------------------------|")

$vIdx = 1
foreach ($line in ($rawVendors -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $p = $line.Split('|')
    if ($p.Count -lt 8) { continue }
    $col1 = $p[1].Trim()
    $col2 = $p[2].Trim()
    $col3 = $p[3].Trim()
    $col4 = $p[4].Trim()
    $col5 = $p[5].Trim()
    $col6 = $p[6].Trim()
    $aboutShort = $p[7].Trim()
    if ($aboutShort.Length -gt 70) { $aboutShort = $aboutShort.Substring(0, 70) + "..." }
    [void]$sb.AppendLine("| $vIdx | **$col1** | $b$col2$b | ${b}Password@123${b} | Vendor | $col3 | $b$col4$b | $col5 | $b$col6$b | $aboutShort |")
    $vIdx++
}

[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 4. Master Items Catalog (with Square Preview Images)")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| # | Item Code | Item Name | Category | Attachment ID | Image Endpoint |")
[void]$sb.AppendLine("|---|-----------|-----------|----------|---------------|----------------|")

$iIdx = 1
foreach ($line in ($rawCatalog -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $p = $line.Split('|')
    if ($p.Count -lt 5) { continue }
    $col1 = $p[1].Trim()
    $col2 = $p[2].Trim()
    $col3 = $p[3].Trim()
    $attId = $p[4].Trim()
    $ep = if ($attId -ne "--") { "$b/api/attachments/$attId$b" } else { "None" }
    [void]$sb.AppendLine("| $iIdx | $b$col1$b | **$col2** | $col3 | $b$attId$b | $ep |")
    $iIdx++
}

$auctionsQuery = 'SELECT a."Id", a."DocNoYearly", a."AuctionName", s."Name", o."Name", CASE WHEN a."IsForwardAuction" THEN ''Forward'' ELSE ''Reverse'' END, (SELECT count(*) FROM "AuctionRel"."Bid" WHERE "AuctionId" = a."Id" AND "IsCurrent" = true) FROM "AuctionRel"."Auction" a JOIN "GlobalData"."Status" s ON a."StatusId" = s."Id" JOIN "TenantRel"."Organization" o ON a."OrganizationId" = o."Id" ORDER BY a."Id" ASC;'
$rawAuctions = $auctionsQuery | & $psql -U $DbUser -h $DbHost -p $DbPort -d $DbName -t -A -F "|"

[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 5. Seeded Auctions & Live Bidding Previews")
[void]$sb.AppendLine()
[void]$sb.AppendLine("A random visitor browsing the application will see these live, scheduled, and completed auctions:")
[void]$sb.AppendLine()
[void]$sb.AppendLine("| # | Doc Number | Auction Name | Status | Type | Buyer Organization | Active Bids | Detail Endpoint | Leaderboard Endpoint |")
[void]$sb.AppendLine("|---|------------|--------------|--------|------|--------------------|-------------|-----------------|----------------------|")

$aIdx = 1
foreach ($line in ($rawAuctions -split "`r?`n")) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }
    $p = $line.Split('|')
    if ($p.Count -lt 7) { continue }
    $aId = $p[0].Trim()
    $aDoc = $p[1].Trim()
    $aName = $p[2].Trim()
    $aStat = $p[3].Trim()
    $aOrg = $p[4].Trim()
    $aType = $p[5].Trim()
    $aBids = $p[6].Trim()
    [void]$sb.AppendLine("| $aIdx | $b$aDoc$b | **$aName** | $b$aStat$b | $aType | $aOrg | **$aBids** | $b/api/auctions/$aId$b | $b/api/bids/auction/$aId/leaderboard$b |")
    $aIdx++
}

[void]$sb.AppendLine()
[void]$sb.AppendLine("---")
[void]$sb.AppendLine()
[void]$sb.AppendLine("## 6. Storage Space Clearing Verification")
[void]$sb.AppendLine()
[void]$sb.AppendLine("- **Tenant Profiles:** When updating the Legal Name, About story, or Foreground Branding Image via `PUT /api/profile`, any replaced or removed image is automatically deleted from `wwwroot/uploads/` on disk to free up storage space.")
[void]$sb.AppendLine("- **Item Master:** When updating an item via `PUT /api/masters/items/{id}` or deleting an item via `DELETE /api/masters/items/{id}`, any replaced or removed item preview image is immediately purged from disk.")
[void]$sb.AppendLine("- **Name Uniqueness:** If a tenant attempts to change their legal name to a name that already belongs to another tenant, the API rejects the request with HTTP `409 Conflict`.")

[System.IO.File]::WriteAllText($MarkdownPath, $sb.ToString(), [System.Text.Encoding]::UTF8)

Write-Host "`nMarkdown reference generated successfully at:" -ForegroundColor Green
Write-Host "  $MarkdownPath" -ForegroundColor Cyan
Write-Host "`nSummary of Seeded Data:" -ForegroundColor Green
Write-Host "  - Organizations: $($oIdx - 1)" -ForegroundColor White
Write-Host "  - Vendors:       $($vIdx - 1)" -ForegroundColor White
Write-Host "  - Master Items:  $($iIdx - 1)" -ForegroundColor White
Write-Host "  - Auctions:      $($aIdx - 1)" -ForegroundColor White
Write-Host "=================================================" -ForegroundColor Cyan

