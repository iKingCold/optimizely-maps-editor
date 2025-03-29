@echo off

echo Starting the installation of EPiServer CLI tool...
echo ============================================

dotnet tool install EPiServer.Net.Cli --global --add-source https://nuget.optimizely.com/feed/packages.svc
IF %ERRORLEVEL% NEQ 0 (
    echo Failed to install EPiServer CLI tool. Please check your connection or permissions.
    exit /b 1
)
echo ============================================

echo Creating the CMS database...
dotnet-episerver create-cms-database "DemoSite\DemoSite.csproj" -S "(LocalDb)\MSSQLLocalDB" -E -dn "OpenMapsEditorDB" -du "OpenMapsEditorUser" -dp "Qwerty123!"
IF %ERRORLEVEL% NEQ 0 (
    echo Failed to create the CMS database. Please check your parameters or database setup.
    exit /b 1
)
echo CMS database created successfully.
echo ============================================
pause
