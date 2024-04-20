::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: Bootloader batch file for new workstation setup
::
:: 1. Warns user if they run the script as admin
:: 2. Elevates to run some things as admin including UAC prompt
:: 3. Then runs some things as user
::  
:: To add things to be run as admin (Step 1) go to the :userRunAsAdminBatch function
::
:: To add things to be run as the user after that, go to the :finalSteps function
::
::      - David Sikes (PockyBum522)
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

setlocal EnableDelayedExpansion
@echo off

: For checking for internet

ping -n 2 -w 700 8.8.8.8 | find "TTL="

IF %ERRORLEVEL% EQU 0 (
    SET internet=internet_connected
) ELSE (
    SET internet=internet_not_connected
)

: For detecting if we're running as admin for warning to user
cacls "%systemroot%\system32\config\system" 1>nul 2>&1

: ELEV argument present means we're calling this script again but with admin privs now
if "%1" neq "ELEV" (
	
		call :warnUserIfRunningAsAdministrator
		
		call :warnUserIfRunningWithoutInternet
		
		call :createElevatedActionsMutexFile
		
		call :initElevatedActions

		call :pauseUntilElevatedActionsFinish
		
		call :finalSteps

) else (

    call :gotPrivileges
    
)


:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: ADD YOUR BATCH COMMANDS HERE, AFTER THIS BANNER. ANYTHING IN THIS FUNCTION RUNS AS ADMIN, NOT AS THE USER
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:userSuppliedBatchRunAsAdmin
	
	echo.
    echo Running user supplied batch file stuff to be run as admin!
    echo.
	
	set SETTINGSEXE="%~dp0WindowsCommandLineSettings.exe"
	
	:: Set the taskbar search bar to be completely hidden
	%SETTINGSEXE% -Taskbar-Searchbar-SetHidden	
	
    exit /B
	
	
:userSuppliedBatchRunAsUser

	echo.
    echo Running user admin batch file stuff!
    echo.
	
	set SETTINGSEXE="%~dp0WindowsCommandLineSettings.exe"
	
	:: Set the taskbar search bar to be completely hidden
	%SETTINGSEXE% -Taskbar-Searchbar-SetHidden	
	
    exit /B
	
	
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: THIS SECTION RUNS FIRST. EVERYTHING IN HERE RUNS AS ADMINISTRATOR.
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:gotPrivileges
    
    echo.
    echo Running admin stuff!
    echo.

	:: Ensure that even if script terminates unexpectedly there's no hostname by default
	call :saveHostnameBlank

	echo.
	echo.
	echo.
	echo.
	echo.
	echo.
	echo.
	echo.
	echo.
	echo.
	echo ---------------------------------------------------
	echo NOTICE: TEMPORARY MEASURES FOR RUNNING APPLICATION
	echo ---------------------------------------------------
	echo.
	echo In order to ensure that no user interaction is needed, we need to do a few things:
	echo.
	echo 1. UAC Needs to be set to "Never Notify"
	echo 2. If your batch file needs to reboot, automatic logon to your user account needs to be set up so that reboots may take place with no interaction
	echo.
	REM echo Once the application has finished, you will be prompted to ask if 
	REM echo you want to set UAC and automatic logon back to their default settings
	REM echo.
	echo Once you have read this, this script will assist you with setting these settings
	echo.

	echo ONLY AFTER READING THE ABOVE THOROUGHLY should you press any key to continue...
	
	:: Pause without the prompt
	pause>nul
	
	setlocal EnableDelayedExpansion
	
	echo ---------------------------------------------------
	echo SCRIPT CONFIGURATION QUESTIONS
	echo ---------------------------------------------------
	echo.
	echo In order to ensure that no user interaction is needed, we need to do a few things:
	echo.
	echo 1. UAC Needs to be set to "Never Notify"
	echo 2. If your batch file needs to reboot, automatic logon to your user account needs to be set up so that reboots may take place with no interaction
	echo.
	
	echo.
	echo.
	set /P userResponseUacAndAutologon=Do you want to change UAC prompt level and set up automatic logon so that script can run completely unattended even if reboots are necessary [Y]/N?
	
	echo.
	echo.
	set /P userResponseInstallDotnetSdk=Do you want to install the latest dotnet sdk [Y]/N?
	
	echo.
	echo.
	set /P userResponseInstallNotepadPlusPlus=Do you want this script to install Notepad++ and powershell core [Y]/N?
	
	echo.
	echo.
	set /P userResponseSetNewHostname=Do you want this script to set a new hostname? [Blank] for do not set new hostname, any other will be set as new hostname:
	
	call :saveHostnameAsRegistryEntry
	
    echo.
    echo Installing Chocolatey
    echo.

    powershell -NoProfile -InputFormat None -ExecutionPolicy Bypass -Command "[System.Net.ServicePointManager]::SecurityProtocol = 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"

    echo.
    echo Setting Chocolatey confirmation prompts to not show
    echo.

    choco feature enable -n=allowGlobalConfirmation

    echo.
    echo If Chocolatey was previously installed, checking for updates
    echo.

    choco upgrade chocolatey

    choco config set --name cacheLocation --value "%~dp0WindowsCommandLineSettings\Resources\ChocolateyCache"
	
    choco upgrade autologon

	if /I "%userResponseSetNewHostname%" EQU "" goto :skipSettingHostname
	
	WMIC computersystem where Name='%ComputerName%' Call rename "%userResponseSetNewHostname%"
	
	:skipSettingHostname
	if /I "%userResponseSetNewHostname%" EQU "" goto :skipSettingHostname
	
	WMIC computersystem where Name='%ComputerName%' Call rename "%userResponseSetNewHostname%"
	
	:skipSettingHostname

	if /I "%userResponseUacAndAutologon%" EQU "N" goto :skipUacPromptsAndAutologon
	if /I "%userResponseUacAndAutologon%" EQU "n" goto :skipUacPromptsAndAutologon

    echo.
    echo -------------------------------------
    echo SET UAC TO NEVER NOTIFY
    echo -------------------------------------
    echo.
    echo In the window that pops up, please set UAC to "Never Notify" by 
    echo dragging the slider all the way to the bottom. When you are finished, press ok.
    echo.

    useraccountcontrolsettings

    autologon64

    echo.
    echo -----------------------------------------------------
    echo SET USER ACCOUNT TO AUTOMATICALLY LOGON
    echo -----------------------------------------------------
    echo.
    echo In the window that pops up, please enter the password for this 
    echo user account and click "Enable" to save and activate automatic logon. 
    echo.
    echo After this, script becomes unattended and you can walk away.
    echo.
    echo Once finished, press any key to continue . . .

    pause>nul
	
	echo.
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
	echo YOU CAN NOW WALK AWAY, REST OF SCRIPT WILL RUN UNATTENDED
		

	:skipUacPromptsAndAutologon
	
	if /I "%userResponseInstallDotnetSdk%" EQU "N" goto :skipInstallDotNetSdk
	if /I "%userResponseInstallDotnetSdk%" EQU "n" goto :skipInstallDotNetSdk
	
    echo.
    echo Installing latest Dot NET SDK
    echo.

    choco upgrade dotnet-sdk

    :: Bugfix that takes care of certain observed instances where 
    :: dot net dependencies were potentially not installed properly
    choco install dotnet-sdk --force

	:skipInstallDotNetSdk

	if /I "%userResponseInstallNotepadPlusPlus%" EQU "N" goto :skipInstallNotepad
	if /I "%userResponseInstallNotepadPlusPlus%" EQU "n" goto :skipInstallNotepad
	
    echo.
    echo Installing powershell core
    echo.

    :: Ensures we can run any necessary powershell commands with the latest version if necessary
    choco upgrade powershell-core --install-arguments='"ADD_EXPLORER_CONTEXT_MENU_OPENPOWERSHELL=1 ADD_FILE_CONTEXT_MENU_RUNPOWERSHELL=1 USE_MU=1 ENABLE_MU=1"'

    echo.
    echo Installing NotePad++
    echo.

    choco upgrade NotePadPlusPlus

	:skipInstallNotepad

    echo.
    echo Refreshing environment variables for this shell instance
    echo.
    ::echo "RefreshEnv.cmd only works from cmd.exe, please install the Chocolatey Profile to take advantage of refreshenv from PowerShell"
    call :RefreshEnvironmentVariables

	:: Run user supplied commands in the function at the top of this script
	call :userSuppliedBatchRunAsAdmin

    echo.
    echo Deleting lockfile that represents admin stuff is running
    echo.

    del "%PUBLIC%\Documents\elevatedActionsScriptV01.lockfile"
	
    exit 0

::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:: FINAL STEPS ONLY. THIS RUNS AFTER THE ADMIN PRIVS PART. THIS PART RUNS AS USER.
::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
:finalSteps
    echo.
    echo Running final steps!
    echo.
    
	:: Remove desktop cache folder only if it exists (This only happens if we run the .exe from the desktop.)
	if exist "%USERPROFILE%\Desktop\WindowsCommandLineSettings" (
		rd /S /Q "%USERPROFILE%\Desktop\WindowsCommandLineSettings"
	)
	
	del "%AppData%\NuGet\NuGet.Config"
	
	:: Run user supplied commands in the function at the top of this script
	:: This particular setting works fine either run as admin, or run as the user. Not all of them are like that. Some require running as admin, some require running as user.	
	call :userSuppliedBatchRunAsUser
	
	echo.
    echo Finished configuring this computer.
	echo.
		
	FOR /F "tokens=2* skip=2" %%a in ('reg query "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v "SCRIPT_NEW_HOSTNAME"') do SET FETCHEDHOSTNAME=%%b
		
	echo Fetched hostname:
	echo %FETCHEDHOSTNAME%
	
	if /I "%FETCHEDHOSTNAME%" EQU "" (
		
		pause
	
	) else (
	
		echo. 
		echo Computer needs to reboot to set new hostname.
		echo.
		echo Press any key to reboot...
				
		pause>nul
		
		shutdown -r -t 01 -f
	)
    
    exit 0

::::::::::::::::::::::::::::::::::::::::::::::
:: SCRIPT UTILITY LOGIC ONLY BELOW HERE
::::::::::::::::::::::::::::::::::::::::::::::

:warnUserIfRunningAsAdministrator

    if "%errorlevel%" equ "0" (
    
        echo -------------------------------------------------------------
        echo ERROR: YOU ARE RUNNING THIS WITH ADMINISTRATOR PRIVILEGES
        echo -------------------------------------------------------------
        echo. 
        echo If you're seeing this, it means you are running this as admin user!
        echo.
        echo You will need to restart this program WITHOUT Administrator 
        echo privileges.
        echo. 
        echo Make sure to NOT Run As Administrator next time!
        echo. 
        echo Press any key to exit . . .

        pause> nul

        exit 1
    )

    exit /B
	
:warnUserIfRunningWithoutInternet

    if "%internet%" equ "internet_not_connected" (
    
        echo -------------------------------------------------------------
        echo ERROR: YOU ARE RUNNING THIS WITHOUT AN INTERNET CONNECTION
        echo -------------------------------------------------------------
        echo. 
        echo If you're seeing this, it means you are running this with no internet!
        echo.
        echo You will need to restart this program after connecting.
        echo. 
        echo Make sure to connect to the internet BEFORE re-running.
        echo. 
        echo Press any key to exit . . .

        pause> nul

        exit 1
    )

    exit /B

:createElevatedActionsMutexFile

    echo.
    echo Creating lockfile for waiting until elevated actions finish
    echo.

    copy /y NUL %PUBLIC%\Documents\elevatedActionsScriptV01.lockfile >NUL
    
    exit /B

:pauseUntilElevatedActionsFinish

    echo.
    echo Waiting for elevated actions portion of script to finish.
    echo.
    echo This may take some time...
    echo.

    timeout /t 10

    IF EXIST %PUBLIC%\Documents\elevatedActionsScriptV01.lockfile goto pauseUntilElevatedActionsFinish

    echo.
    echo Refreshing environment variables for this shell instance
    echo.
    ::echo "RefreshEnv.cmd only works from cmd.exe, please install the Chocolatey Profile to take advantage of refreshenv from PowerShell"
    call :RefreshEnvironmentVariables

    exit /B


:saveHostnameBlank

	:: Set registry entry for hostname
	del %TEMP%\newHostname.reg

	echo Windows Registry Editor Version 5.00 >> %TEMP%\newHostname.reg
	echo: >> %TEMP%\newHostname.reg
	echo [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment] >> %TEMP%\newHostname.reg
	echo: "SCRIPT_NEW_HOSTNAME"="" >> %TEMP%\newHostname.reg
	echo: >> %TEMP%\newHostname.reg

	REG IMPORT %TEMP%\newHostname.reg

	exit /B
	

:saveHostnameAsRegistryEntry
	
	:: Set registry entry for hostname
	del %TEMP%\newHostname.reg

	echo Windows Registry Editor Version 5.00 >> %TEMP%\newHostname.reg
	echo: >> %TEMP%\newHostname.reg
	echo [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment] >> %TEMP%\newHostname.reg
	echo: "SCRIPT_NEW_HOSTNAME"="%userResponseSetNewHostname%" >> %TEMP%\newHostname.reg
	echo: >> %TEMP%\newHostname.reg

	REG IMPORT %TEMP%\newHostname.reg

	exit /B

::::::::::::::::::::::::::::::::::::::::::::
:: Elevate.cmd - Version 4
:: Automatically check & get admin rights
:: see "https://stackoverflow.com/a/12264592/1016343" for description
:: Modified by David Sikes
::::::::::::::::::::::::::::::::::::::::::::

:initElevatedActions
    	
    setlocal DisableDelayedExpansion
    set cmdInvoke=1
    set winSysFolder=System32
    set "batchPath=%~dpnx0"
    rem this works also from cmd shell, other than %~0
    for %%k in (%0) do set batchName=%%~nk
    set "vbsGetPrivileges=%temp%\OEgetPriv_batchScriptV01.vbs"
    setlocal EnableDelayedExpansion

    ECHO.
    ECHO **************************************
    ECHO Invoking UAC for Privilege Escalation
    ECHO **************************************
    ECHO.

    ECHO Set UAC = CreateObject^("Shell.Application"^) > "%temp%\OEgetPriv_batchScriptV01.vbs"
    ECHO args = "ELEV " >> "%temp%\OEgetPriv_batchScriptV01.vbs"
    ECHO For Each strArg in WScript.Arguments >> "%temp%\OEgetPriv_batchScriptV01.vbs"
    ECHO args = args ^& strArg ^& " "  >> "%temp%\OEgetPriv_batchScriptV01.vbs"
    ECHO Next >> "%temp%\OEgetPriv_batchScriptV01.vbs"
    ECHO UAC.ShellExecute "!batchPath!", args, "", "runas", 1 >> "%temp%\OEgetPriv_batchScriptV01.vbs"

    : Now run the file we just made
    "%SystemRoot%\%winSysFolder%\CScript.exe" "%temp%\OEgetPriv_batchScriptV01.vbs" ELEV

    exit /B

:: -------------------------------------------------------
:: BELOW HERE IS ALL CODE FROM CHOCOLATEY'S RefreshEnv.cmd
:: -------------------------------------------------------

:: Set one environment variable from registry key
:SetFromReg
    "%WinDir%\System32\Reg" QUERY "%~1" /v "%~2" > "%TEMP%\_envset.tmp" 2>NUL
    for /f "usebackq skip=2 tokens=2,*" %%A IN ("%TEMP%\_envset.tmp") do (
        echo/set "%~3=%%B"
    )
    goto :EOF

:: Get a list of environment variables from registry
:GetRegEnv
    "%WinDir%\System32\Reg" QUERY "%~1" > "%TEMP%\_envget.tmp"
    for /f "usebackq skip=2" %%A IN ("%TEMP%\_envget.tmp") do (
        if /I not "%%~A"=="Path" (
            call :SetFromReg "%~1" "%%~A" "%%~A"
        )
    )
    goto :EOF

:RefreshEnvironmentVariables
    echo/@echo off >"%TEMP%\_env.cmd"

    :: Slowly generating final file
    call :GetRegEnv "HKLM\System\CurrentControlSet\Control\Session Manager\Environment" >> "%TEMP%\_env.cmd"
    call :GetRegEnv "HKCU\Environment">>"%TEMP%\_env.cmd" >> "%TEMP%\_env.cmd"

    :: Special handling for PATH - mix both User and System
    call :SetFromReg "HKLM\System\CurrentControlSet\Control\Session Manager\Environment" Path Path_HKLM >> "%TEMP%\_env.cmd"
    call :SetFromReg "HKCU\Environment" Path Path_HKCU >> "%TEMP%\_env.cmd"

    :: Caution: do not insert space-chars before >> redirection sign
    echo/set "Path=%%Path_HKLM%%;%%Path_HKCU%%" >> "%TEMP%\_env.cmd"

    :: Cleanup
    del /f /q "%TEMP%\_envset.tmp" 2>nul
    del /f /q "%TEMP%\_envget.tmp" 2>nul

    :: capture user / architecture
    SET "OriginalUserName=%USERNAME%"
    SET "OriginalArchitecture=%PROCESSOR_ARCHITECTURE%"

    :: Set these variables
    call "%TEMP%\_env.cmd"

    :: Cleanup
    del /f /q "%TEMP%\_env.cmd" 2>nul

    :: reset user / architecture
    SET "USERNAME=%OriginalUserName%"
    SET "PROCESSOR_ARCHITECTURE=%OriginalArchitecture%"

    echo | set /p dummy="Finished refreshing environtment variables."
    echo.
    