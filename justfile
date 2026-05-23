solution_root := justfile_directory()

_default:
  @just --list
  
run:
  dotnet run --project {{ solution_root }}/Api --launch-profile https

watch:
  dotnet watch run --quiet --project {{ solution_root }}/Api --launch-profile https

fmt:
  dotnet csharpier format . 