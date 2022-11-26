$json = Get-Content "../manifest.json" | ConvertFrom-Json
$version = $json.version_number
(Get-Content MaxDrawDistance.cs) -Replace "0.0.0.0", "$version.0" | Set-Content MaxDrawDistance.cs
(Get-Content MaxDrawDistancePlugin.csproj) -Replace "<Version>0.0.0</Version>", "<Version>$version</Version>" | Set-Content MaxDrawDistancePlugin.csproj