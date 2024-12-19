@echo off

rem Script to prepare a new release using the dotnet tool nbgv.
rem Ensure nbgv is installed before running this script. Install it using `dotnet tool install -g nbgv`

echo Running nbgv prepare-release...
nbgv prepare-release --format json --commit-message-pattern "chore(release): bump version to {0}" || exit /b 1

echo Release preparation is complete. IMPORTANT: Don't forget to push the newly created release branch.
