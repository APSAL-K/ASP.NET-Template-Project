#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Usage: ./scripts/scaffold-module.sh <ModuleName>" >&2
    exit 1
fi

module_name="$1"

if [[ ! "$module_name" =~ ^[A-Za-z][A-Za-z0-9_]*$ ]]; then
    echo "Invalid module name: $module_name" >&2
    echo "Use only letters, numbers, and underscores, and start with a letter." >&2
    exit 1
fi

root="Modules/$module_name"

mkdir -p "$root"/{Application/DTOs,Application/Interfaces,Controllers,Domain/Entities,Persistence/Migrations,Services}

files=(
    "$root/${module_name}ModuleExtensions.cs"
    "$root/Application/DTOs/${module_name}Dtos.cs"
    "$root/Application/Interfaces/I${module_name}Service.cs"
    "$root/Controllers/${module_name}Controller.cs"
    "$root/Domain/Entities/${module_name}Entity.cs"
    "$root/Persistence/${module_name}DbContext.cs"
    "$root/Persistence/${module_name}DatabaseConfiguration.cs"
    "$root/Persistence/${module_name}DbContextFactory.cs"
    "$root/Persistence/${module_name}SeedData.cs"
    "$root/Services/${module_name}Service.cs"
)

for file_path in "${files[@]}"; do
    if [[ ! -e "$file_path" ]]; then
        touch "$file_path"
    fi
done

echo "Module scaffold created or verified at $root"