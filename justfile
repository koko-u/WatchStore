solution_root := justfile_directory()

_default:
  @just --list

run:
  dotnet run --project {{ solution_root }}/Api --launch-profile https

watch:
  DOTNET_WATCH_SUPPRESS_STATIC_FILE_HANDLING=1 \
  dotnet watch run --quiet --project {{ solution_root }}/Api --launch-profile https

fmt:
  dotnet csharpier format .
