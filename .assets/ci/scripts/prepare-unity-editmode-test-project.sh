#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'EOF'
Usage: prepare-unity-editmode-test-project.sh \
  --project-path <path> \
  --test-family <base|fixedmathsharp> \
  --dependency-flavor <standard|lean> \
  --package-name <name> \
  --package-path <path> \
  --runtime-assembly <assembly-name> \
  [--base-package-name <name>] \
  [--base-package-path <path>] \
  [--fixedmathsharp-package-name <name>] \
  [--fixedmathsharp-package-url <url>] \
  [--fixedmathsharp-runtime-assembly <assembly-name>]
EOF
}

project_path=""
test_family=""
dependency_flavor=""
package_name=""
package_path=""
runtime_assembly=""
base_package_name=""
base_package_path=""
fixedmathsharp_package_name=""
fixedmathsharp_package_url=""
fixedmathsharp_runtime_assembly=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --project-path)
      project_path="$2"
      shift 2
      ;;
    --test-family)
      test_family="$2"
      shift 2
      ;;
    --dependency-flavor)
      dependency_flavor="$2"
      shift 2
      ;;
    --package-name)
      package_name="$2"
      shift 2
      ;;
    --package-path)
      package_path="$2"
      shift 2
      ;;
    --runtime-assembly)
      runtime_assembly="$2"
      shift 2
      ;;
    --base-package-name)
      base_package_name="$2"
      shift 2
      ;;
    --base-package-path)
      base_package_path="$2"
      shift 2
      ;;
    --fixedmathsharp-package-name)
      fixedmathsharp_package_name="$2"
      shift 2
      ;;
    --fixedmathsharp-package-url)
      fixedmathsharp_package_url="$2"
      shift 2
      ;;
    --fixedmathsharp-runtime-assembly)
      fixedmathsharp_runtime_assembly="$2"
      shift 2
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Unknown argument: $1" >&2
      usage >&2
      exit 2
      ;;
  esac
done

if [[ -z "$project_path" || -z "$test_family" || -z "$dependency_flavor" || -z "$package_name" || -z "$package_path" || -z "$runtime_assembly" ]]; then
  usage >&2
  exit 2
fi

case "$dependency_flavor" in
  standard|lean) ;;
  *)
    echo "Unknown dependency flavor: $dependency_flavor" >&2
    exit 2
    ;;
esac

if [[ "$test_family" == "fixedmathsharp" ]]; then
  if [[ -z "$base_package_name" || -z "$base_package_path" || -z "$fixedmathsharp_package_name" || -z "$fixedmathsharp_package_url" || -z "$fixedmathsharp_runtime_assembly" ]]; then
    usage >&2
    exit 2
  fi
elif [[ "$test_family" != "base" ]]; then
  echo "Unknown test family: $test_family" >&2
  exit 2
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../../.." && pwd)"
template_root="$repo_root/.assets/ci/unity-editmode-tests"

if [[ ! -d "$template_root" ]]; then
  echo "Unity EditMode test template was not found: $template_root" >&2
  exit 1
fi

rm -rf "$project_path"
mkdir -p "$project_path"
cp -R "$template_root/." "$project_path/"

mkdir -p "$project_path/Assets/Tests/EditMode"
if [[ "$test_family" == "fixedmathsharp" ]]; then
  cp "$repo_root"/Tests/EditMode.FixedMathSharp/*.cs "$project_path/Assets/Tests/EditMode/"
  asmdef_template="$project_path/Assets/Tests/EditMode/SwiftCollections.Unity.Tests.EditMode.fixedmathsharp-${dependency_flavor}.asmdef.template"
  manifest_template="$project_path/Packages/manifest.fixedmathsharp.json.template"
else
  cp "$repo_root"/Tests/EditMode/*.cs "$project_path/Assets/Tests/EditMode/"
  asmdef_template="$project_path/Assets/Tests/EditMode/SwiftCollections.Unity.Tests.EditMode.base-${dependency_flavor}.asmdef.template"
  manifest_template="$project_path/Packages/manifest.base.json.template"
fi

escape_sed_replacement() {
  printf '%s' "$1" | sed -e 's/[\/&|]/\\&/g'
}

render_template() {
  local template_path="$1"
  local output_path="$2"

  sed \
    -e "s|__PACKAGE_NAME__|$(escape_sed_replacement "$package_name")|g" \
    -e "s|__PACKAGE_PATH__|$(escape_sed_replacement "$package_path")|g" \
    -e "s|__RUNTIME_ASSEMBLY__|$(escape_sed_replacement "$runtime_assembly")|g" \
    -e "s|__BASE_PACKAGE_NAME__|$(escape_sed_replacement "$base_package_name")|g" \
    -e "s|__BASE_PACKAGE_PATH__|$(escape_sed_replacement "$base_package_path")|g" \
    -e "s|__FIXEDMATHSHARP_PACKAGE_NAME__|$(escape_sed_replacement "$fixedmathsharp_package_name")|g" \
    -e "s|__FIXEDMATHSHARP_PACKAGE_URL__|$(escape_sed_replacement "$fixedmathsharp_package_url")|g" \
    -e "s|__FIXEDMATHSHARP_RUNTIME_ASSEMBLY__|$(escape_sed_replacement "$fixedmathsharp_runtime_assembly")|g" \
    "$template_path" > "$output_path"
}

render_template "$asmdef_template" "$project_path/Assets/Tests/EditMode/SwiftCollections.Unity.Tests.EditMode.asmdef"
render_template "$manifest_template" "$project_path/Packages/manifest.json"

find "$project_path" -name '*.template' -delete
